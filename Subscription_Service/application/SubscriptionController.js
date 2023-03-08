// EXCEPTIONS
const e = require('express')
const Exceptions = require('../exceptions')
const subscriptionRepository =
    require('../persistence/subscriptionRepository').SubscriptionRepository
const deviceRepository =
    require('../persistence/deviceRepository').DevicesRepository

const planService = require('../service/PlanService').PlanService
const userService = require('../service/UserService').UserService

const NotFoundException = Exceptions.NotFoundException
const BadRequestException = Exceptions.BadRequestException
const ConflictException = Exceptions.ConflictException
const ConcurrencyException = Exceptions.ConcurrencyException
const utils = require('../utils/utils')
const createID = utils.createID
const formatDate = utils.formatDate
const UnauthenticatedException = Exceptions.UnauthenticatedException

//! Utils
function hasNextLink(links) {
    let nextLink = null;
    links.split(',').forEach((link) => {
        if (link.includes('next')) {
            nextLink = link;
        }
    })
    return nextLink;
}


async function getSubscriptions(
    username,
    roles,
    createdOn,
    canceledOn,
    year,
    planID,
    start,
    n
) {
    const subscriptionsArray = await subscriptionRepository.getSubscriptions(
        createdOn,
        canceledOn,
        year,
        planID,
        start,
        n
    )
    const subscriptionsFormatted = []
    const data = subscriptionsArray.data
    const users = await userService.getUsers(1, 1)
    let nextUsers = hasNextLink(users.links);
    while (nextUsers != null) {
        const start = nextUsers.split('start=')[1].split('&')[0];
        const n = nextUsers.split('n=')[1].split('>')[0];
        const otherUsers = await userService.getUsers(parseInt(start), parseInt(n));
        nextUsers = hasNextLink(otherUsers.links);
        users.data = users.data.concat(otherUsers.data);
    }
    const plans = await planService.getPlans(username, roles, 1, 5);
    let next = hasNextLink(plans.headers.link);

    // TODO: We should consider use the getById method from the plan service
    while (next != null) {
        const start = next.split('start=')[1].split('&')[0];
        const n = next.split('n=')[1].split('>')[0];
        const otherPlans = await planService.getPlans(username, roles, start, n);
        next = hasNextLink(otherPlans.headers.link);
        plans.data = plans.data.concat(otherPlans.data);
    }
    for (const i in data) {
        const plan = plans.data.find((p) => p.id === data[i].plan)
        const user = users.data.find((u) => u.id === data[i].user)
        const subscriptionsWithPlan = buildSubscription(data[i], plan, user)
        subscriptionsFormatted.push(subscriptionsWithPlan)
    }
    return Promise.resolve({
        isLast: subscriptionsArray.isLast,
        data: subscriptionsFormatted
    })
}

async function getDashboard(month) {
    if (!month) {
        return Promise.reject(
            new BadRequestException(
                "You need to provide a valid 'month' parameter "
            )
        )
    }
    // TODO: Maybe we can do on a unique query but we have to consider some things like if the data is gonna be used (in calculations for example)
    // TODO: We will have to consider plans and users too but we can do it when we really implement the dashboard. This is just the fix for the last sprint implementation
    const subscriptionOnSpecificMonth =
        await subscriptionRepository.searchSubscriptions({ createdOn: month })
    const subscriptionCanceledOnSpecificMonth =
        await subscriptionRepository.searchSubscriptions({ canceledOn: month })
    const dashboard = {
        totalSubscriptions: subscriptionOnSpecificMonth.length,
        totalSubscriptionsCanceled: subscriptionCanceledOnSpecificMonth.length
    }
    return Promise.resolve(dashboard)
}

function buildSubscription(subscription, plan, user) {
    return {
        id: subscription.id,
        user: user.id,
        plan: plan.id,
        status: subscription.status,
        __v: subscription.__v,
        subscriptionFee: subscription.subscriptionFee,
        type: subscription.type,
        createdOn: new Date(subscription.createdOn).toLocaleDateString('en-GB'),
        renewDate: new Date(subscription.renewDate).toLocaleDateString('en-GB'),
        cancelationDate:
            subscription.cancelationDate == undefined
                ? undefined
                : new Date(subscription.cancelationDate).toLocaleDateString(
                    'en-GB'
                )
    }
}

function buildDevice(device, subscription) {
    return {
        id: device.id,
        name: device.name,
        description: device.description,
        subscription: {
            id: subscription.id,
            user: subscription.user
        },
        _v: device._v
    }
}

async function getSubscriptionById(id) {
    if (!id) {
        return Promise.reject(
            new BadRequestException(
                "You need to provide a valid 'id' of a subcription."
            )
        )
    }
    const subscription = await subscriptionRepository.getSubscription(id)

    if (!subscription) {
        return Promise.reject(
            new NotFoundException(`Subscription with id ${id} not found`, id)
        )
    }
    const plan = await planService.getPlanById(subscription.plan)
    const user = await userService.getUserById(subscription.user);
    const subscriptionWithPlan = buildSubscription(subscription, plan.data, user.data)
    return Promise.resolve(subscriptionWithPlan)
}
async function getDevicesOfSubscription(subscriptionId, username) {
    if (!subscriptionId) {
        return Promise.reject(
            new BadRequestException(
                "You need to provide a valid 'id' of a subscription."
            )
        )
    }
    if (!username) {
        return Promise.reject(
            new UnauthenticatedException(
                'You need to be authenticated to perform this action.'
            )
        )
    }
    const subscription = await subscriptionRepository.getSubscription(
        subscriptionId
    )
    if (!subscription) {
        return Promise.reject(
            new ConflictException(
                `Subscription with id ${subscriptionId} not found`,
                subscriptionId
            )
        )
    }
    if (subscription.user !== username) {
        return Promise.reject(
            new Exceptions.ForbiddenException(
                'You are not authorized to perform this action.'
            )
        )
    }
    const devices = await deviceRepository.getDevicesOfSubscription(
        subscriptionId
    )
    return Promise.resolve(devices)
}

async function migrateSubscriptionsOfPlan(oldPlanID, newPlanID, start, n) {
    if (!oldPlanID) {
        return Promise.reject(
            new BadRequestException(
                "You need to provide a valid 'oldPlanID' of a plan."
            )
        )
    }
    if (!newPlanID) {
        return Promise.reject(
            new BadRequestException(
                "You need to provide a valid 'newPlanID' of a plan."
            )
        )
    }
    const oldPlan = await planService.getPlanById(oldPlanID)
    if (!oldPlan || !oldPlan?.data) {
        return Promise.reject(
            new NotFoundException(
                `Plan with id ${oldPlanID} not found`,
                oldPlanID
            )
        )
    }
    const newPlan = await planService.getPlanById(newPlanID)
    if (!newPlan || !newPlan?.data) {
        return Promise.reject(
            new NotFoundException(
                `Plan with id ${newPlanID} not found`,
                newPlanID
            )
        )
    }
    const subscriptions = await subscriptionRepository.getSubscriptionOfPlan(
        oldPlanID
    )
    const updatedSubscriptions = []
    for (const i in subscriptions) {
        const subscription =
            await subscriptionRepository.updateSubscriptionPlan(
                subscriptions[i].id,
                newPlanID
            )
        const subscriptionFormatted = buildSubscription(
            subscription, newPlan.data, { user: { id: subscription.user } }
        )
        updatedSubscriptions.push(subscriptionFormatted)
    }
    return Promise.resolve({
        data: updatedSubscriptions.slice(start - 1, start + n - 1),
        isLast: true
    })
}

async function updateDevice(body, deviceID, subscriptionID, username, rev) {
    console.log(rev);
    if (!rev) {
        return Promise.reject(new RangeError("Missing 'if-match' header"))
    }
    if (![body.name, body.description].some((value) => value)) {
        return Promise.reject(
            new BadRequestException(
                "You need to provide a valid 'name' or 'description' of a device."
            )
        )
    }
    if (!username) {
        return Promise.reject(
            new UnauthenticatedException('You need to be authenticated')
        )
    }
    if (!subscriptionID || !deviceID) {
        return Promise.reject(
            new RangeError(
                'You need to provide a valid subscriptionID and deviceID'
            )
        )
    }

    const subscription = await subscriptionRepository.getSubscription(
        subscriptionID
    )
    if (!subscription) {
        return Promise.reject(
            new ConflictException(
                `Subscription with id ${subscriptionID} not found`,
                subscriptionID
            )
        )
    }
    if (subscription.user !== username) {
        return Promise.reject(
            new Exceptions.ForbiddenException(
                `User ${username} is not allowed to update device ${deviceID}`,
                deviceID
            )
        )
    }
    const device = await deviceRepository.getDeviceByID(deviceID)
    if (!device) {
        return Promise.reject(
            new NotFoundException(`Device with id ${deviceID} not found`)
        )
    }
    // Probably not needed as we already checked the user and an user would only have a subscription???
    if (device.subscription !== subscriptionID) {
        return Promise.reject(
            new NotFoundException(
                `Device with id ${deviceID} is not part of subscription ${subscriptionID}`
            )
        )
    }

    if (rev !== device._v.toString()) {
        return Promise.reject(
            new ConcurrencyException(
                'Object has been updated by another user',
                device
            )
        )
    }
    let deviceUpdated = await deviceRepository.updateDevice(
        device,
        body.name,
        body.description
    )
    deviceUpdated = buildDevice(deviceUpdated, subscription)
    return Promise.resolve(deviceUpdated)
}

async function addDeviceToSubscription(body, subscriptionID, username) {
    if (
        ![body.name, body.description, subscriptionID].every((value) => value)
    ) {
        return Promise.reject(
            new RangeError(
                "You need to provide a valid 'id', 'name', 'description' and 'subscription' of a device."
            )
        )
    }
    if (!username) {
        return Promise.reject(
            new UnauthenticatedException(
                'You need to be authenticated to add a device to a subscription.'
            )
        )
    }
    const subscription = await subscriptionRepository.getSubscription(
        subscriptionID
    )
    if (!subscription) {
        return Promise.reject(
            new ConflictException(
                `Subscription with id ${subscriptionID} not found`,
                subscriptionID
            )
        )
    }
    if (subscription.user !== username) {
        return Promise.reject(
            new Exceptions.ForbiddenException(
                'You are not authorized to add a device to this subscription',
                subscriptionID
            )
        )
    }
    const plan = await planService.getPlanById(subscription.plan);
    const devicesInSubscription =
        await deviceRepository.getDevicesOfSubscription(subscriptionID)
    if (devicesInSubscription.length >= plan.data.devices) {
        return Promise.reject(
            new BadRequestException(
                `The subscription with id ${subscriptionID} has reached the maximum number of devices allowed`
            )
        )
    }
    let device = await deviceRepository.addDeviceToSubscription(
        createID(8),
        body.name,
        body.description,
        subscriptionID
    )
    device = buildDevice(device, subscription)
    return Promise.resolve({
        device,
        created: true
    })
}

async function addSubscriptionToAPlan(body) {
    if (![body.planID, body.type, body.userID].every((value) => value)) {
        return Promise.reject(
            new RangeError(
                "You need to provide a valid 'userID', 'planID' and 'type' of a subscription."
            )
        )
    }

    if (!['monthly', 'annual'].includes(body.type)) {
        return Promise.reject(
            new BadRequestException(
                "You need to provide a valid 'type' of a subscription."
            )
        )
    }
    const plan = await planService.getPlanById(body.planID);
    if (!plan || !plan?.data) {
        return Promise.reject(
            new ConflictException(
                `Plan with id ${body.planID} not found`,
                body.planID
            )
        )
    }
    const data = plan.data;
    const user = await userService.getUserById(body.userID)
    if (!user) {
        return Promise.reject(
            new ConflictException(
                `User with id ${body.userID} not found`,
                body.userID
            )
        )
    }

    const userAsAlreadySubscribed =
        await subscriptionRepository.getSubscriptionOfAnUser(body.userID)
    if (userAsAlreadySubscribed) {
        return Promise.reject(
            new ConflictException(
                `User ${body.userID} already has a subscription`,
                body.userID
            )
        )
    }

    const subscriptionFee =
        body.type === 'monthly' ? data.annualFee : data.monthlyFee

    let promotedUser = {}
    try {
        promotedUser = await userService.promoteUserToSubscriber(
            user.data.id,
            user.headers.etag
        )
    } catch {
        return Promise.reject(new ConflictException('Error promoting user'))
    }

    const subscription = await subscriptionRepository.addSubscriptionToAPlan(
        createID(8),
        body.userID,
        body.planID,
        body.type,
        subscriptionFee
    )

    const subscriptionFormatted = buildSubscription(
        subscription,
        data,
        promotedUser
    )
    return Promise.resolve({
        subscription: subscriptionFormatted,
        created: true
    })
}

async function partialUpdateSubscription(changes, subscriptionId, revision) {
    try {
        const subscription = await subscriptionRepository.getSubscription(
            subscriptionId
        )

        if (!(revision === subscription.__v.toString())) {
            return Promise.reject(
                new ConcurrencyException(
                    'Item has been updated by other user already. Please refresh and retry'
                )
            )
        }
        return subscriptionRepository.partialUpdateSubscription(
            subscription,
            changes
        )
    } catch (error) {
        return Promise.reject(
            new NotFoundException(
                `Subscription with Id: ${subscriptionId} not found!`
            )
        )
    }
}

async function renewSubscription(subscription, revision) {
    try {
        const dateParts = subscription.renewDate.split('/')
        const renewDate = new Date(
            +dateParts[2],
            dateParts[1] - 1,
            +dateParts[0]
        )
        const now = new Date()
        let newRenewal = new Date()
        // If we are renewing sooner than the end of the renew date,
        // the new renew date will be one year past the renew date
        // otherwise, if we already passed the renew date, we add one year starting from today
        if (renewDate > now) {
            newRenewal = new Date(renewDate)
            newRenewal.setFullYear(renewDate.getFullYear() + 1)
        } else {
            newRenewal.setFullYear(now.getFullYear() + 1)
        }
        const changes = { renewDate: formatDate(newRenewal) }
        return updateSubscription(changes, subscription)
    } catch (error) {
        return Promise.reject(
            new NotFoundException(
                `Subscription with Id: ${subscription.id} not found!`
            )
        )
    }
}

function updateSubscription(changes, subscription) {
    Object.assign(subscription, changes)
    return subscriptionRepository.saveSubscription(subscription)
}

async function deleteDeviceFromSubscription(
    subscriptionID,
    deviceID,
    username,
    rev
) {
    if (!deviceID || !subscriptionID) {
        return Promise.reject(
            new RangeError("You need to provide a valid 'id' of a device.")
        )
    }
    if (!username) {
        return Promise.reject(
            new UnauthenticatedException(
                "You need to be logged in to delete a device'."
            )
        )
    }
    if (!rev) {
        return Promise.reject(new RangeError("Missing 'if-match' header"))
    }
    const subscription = await subscriptionRepository.getSubscription(
        subscriptionID
    )
    if (!subscription) {
        return Promise.reject(
            new ConflictException(
                `Subscription with id ${subscriptionID} not found`,
                subscriptionID
            )
        )
    }
    if (subscription.user !== username) {
        return Promise.reject(
            new Exceptions.ForbiddenException(
                "You are not authorized to delete this device'."
            )
        )
    }

    const device = await deviceRepository.getDeviceByID(deviceID)
    if (!device) {
        return Promise.reject(
            new NotFoundException(
                `Device with id ${deviceID} not found`,
                deviceID
            )
        )
    }
    // Probably not needed as we already checked the user and an user would only have a subscription???
    if (device.subscription !== subscriptionID) {
        return Promise.reject(
            new Exceptions.NotFoundException(
                'Device is not part of this subscription'
            )
        )
    }
    if (rev !== device._v.toString()) {
        return Promise.reject(
            new ConcurrencyException(
                'Item has been updated by other user already. Please refresh and retry',
                device
            )
        )
    }
    const deviceDeleted = await deviceRepository.deleteDevice(deviceID)
    return Promise.resolve(deviceDeleted)
}

async function upgradeOrDowngradeSubscriptionPlan(
    subscriptionID,
    planID,
    username,
    rev
) {
    if (![subscriptionID, planID].every((value) => value)) {
        return Promise.reject(
            new RangeError(
                "You need to provide a valid 'userID', 'planID' and 'type' of a subscription."
            )
        )
    }
    if (!username) {
        return Promise.reject(
            new UnauthenticatedException(
                "You need to be logged in to update a subscription'."
            )
        )
    }
    if (!rev) {
        return Promise.reject(new RangeError("Missing 'if-match' header"))
    }
    const subscription = await subscriptionRepository.getSubscription(
        subscriptionID
    )
    if (!subscription) {
        return Promise.reject(
            new ConflictException(
                `Subscription with id ${subscriptionID} not found`,
                subscriptionID
            )
        )
    }
    if (subscription.user !== username) {
        return Promise.reject(
            new Exceptions.ForbiddenException(
                "You are not authorized to update this subscription'."
            )
        )
    }
    if (rev !== subscription.__v.toString()) {
        return Promise.reject(
            new ConcurrencyException(
                'Item has been updated by other user already. Please refresh and retry',
                subscription
            )
        )
    }
    const user = await userService.getUserById(username)
    const plan = await planService.getPlanById(planID)
    if (!plan || !plan?.data) {
        return Promise.reject(
            new NotFoundException(`Plan with id ${planID} not found`, planID)
        )
    }
    const subscriptionUpdated =
        await subscriptionRepository.updateSubscriptionPlan(
            subscriptionID,
            planID
        )
    const s = buildSubscription(subscriptionUpdated, plan.data, user.data)
    return Promise.resolve(s)
}

async function getSubscriptionPlan(subscriptionID, username) {
    if (!subscriptionID) {
        return Promise.reject(
            new BadRequestException(
                "You need to provide a valid 'id' of a subcription."
            )
        )
    }
    const subscription = await subscriptionRepository.getSubscription(subscriptionID)

    if (!subscription) {
        return Promise.reject(
            new NotFoundException(`Subscription with id ${subscriptionID} not found`, subscriptionID)
        )
    }
    if (subscription.user !== username) {
        return Promise.reject(
            new Exceptions.UnauthorizedException(
                "You are not authorized to get this subscription'."
            )
        )
    }
    const plan = await planService.getPlanById(subscription.plan)
    return Promise.resolve(plan);
}

async function getSubscriptionUser(subscriptionID, username) {
    if (!subscriptionID) {
        return Promise.reject(
            new BadRequestException(
                "You need to provide a valid 'id' of a subcription."
            )
        )
    }
    const subscription = await subscriptionRepository.getSubscription(subscriptionID)

    if (!subscription) {
        return Promise.reject(
            new NotFoundException(`Subscription with id ${subscriptionID} not found`, subscriptionID)
        )
    }
    if (subscription.user !== username) {
        return Promise.reject(
            new Exceptions.UnauthorizedException(
                "You are not authorized to get this subscription'."
            )
        )
    }
    const user = await userService.getUserById(subscription.user)
    return Promise.resolve(user);
}

exports.SubscriptionService = function (config, dependencies) {
    return {
        getSubscriptions,
        getSubscriptionById,
        partialUpdateSubscription,
        renewSubscription,
        updateDevice,
        addDeviceToSubscription,
        getDevicesOfSubscription,
        addSubscriptionToAPlan,
        deleteDeviceFromSubscription,
        upgradeOrDowngradeSubscriptionPlan,
        migrateSubscriptionsOfPlan,
        getDashboard,
        getSubscriptionPlan,
        getSubscriptionUser
    }
}
