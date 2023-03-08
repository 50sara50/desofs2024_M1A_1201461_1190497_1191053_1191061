'use strict'

const ADMIN = 'admin'
const USER = 'user'
const SUBSCRIBER = 'subscriber'
const MARKETING_DIRECTOR = 'marketing-director'
const subscriptionService =
    require('../application/SubscriptionController').SubscriptionService()

const Hyper = require('./hyper-response').Hyper
const responseModule = require('./response')
const responseHelper = responseModule.ResponseHelper

const _ = require('underscore')
const Response = responseModule.Response
const etagHeader = responseModule.etagHeader
const port = process.env.PORT || process.argv[2] || 3004
const SERVER_ROOT = 'http://localhost:' + port
const API_SERVER = 'http://localhost:3000'

// !Utils functions
// inspired in code in the A Board For You repository from Paulo Gandra de Sousa.
function buildHref(start, n, createdOn, canceledOn, planID) {
    const href = API_SERVER + '/subscriptions?start=' + start + '&n=' + n
    if (createdOn) {
        return href + '&createdOn=' + createdOn
    }
    if (canceledOn) {
        return href + '&canceledOn=' + canceledOn
    }
    if (planID) {
        return href + '&planID=' + planID
    }
    return href
}

// inspired in code in the A Board For You repository from Paulo Gandra de Sousa.
function createLinksOfCollection(
    start,
    n,
    isLast,
    createdOn,
    canceledOn,
    planID
) {
    const links = []
    links.push({
        rel: 'self',
        href: buildHref(start, n, createdOn, canceledOn, planID)
    })
    links.push({
        rel: 'start',
        href: buildHref(1, n, createdOn, canceledOn, planID)
    })
    if (start > 1) {
        links.push({
            rel: 'prev',
            href: buildHref(start - n, n, createdOn, canceledOn, planID)
        })
    }
    if (!isLast) {
        links.push({
            rel: 'next',
            href: buildHref(start + n, n, createdOn, canceledOn, planID)
        })
    }
    if (planID) {
        links.push({
            rel: 'switch-plan',
            href: API_SERVER + '/subscriptions/' + '&planID=' + planID
        })
    }
    links.push({
        rel: 'create',
        href: API_SERVER + '/subscriptions'
    })
    return links
}

// inspired in code in the A Board For You repository from Paulo Gandra de Sousa.
function createLinkHypermediaOfItem(subscription, userRole) {
    const links = []
    const self = API_SERVER + '/subscriptions/' + subscription.id
    links.push({
        rel: 'self',
        href: self
    })
    links.push({
        rel: 'plan',
        href: self + '/plan/' + subscription.plan
    })

    links.push({
        rel: 'user',
        href: self + '/user/' + subscription.user
    })

    links.push({
        rel: 'devices',
        href: self + '/devices'
    })
    if (userRole === 'admin') {
        links.push({
            rel: 'collection',
            href: API_SERVER + '/subscriptions'
        })
    }
    // FIXME: change the url to the correct one
    if (subscription.status === 'active' && userRole === 'subscriber') {
        links.push({
            rel: 'deactivate',
            href: self + '/deactivate'
        })
        links.push({
            rel: 'renew',
            href: self + '/renew'
        })
        links.push({
            rel: 'upgrade-downgrade-plan',
            href: self + '/plan'
        })
    }
    return links
}

// inspired in code in the A Board For You repository from Paulo Gandra de Sousa.
function createQueriesOfCollection() {
    var queries = []
    queries.push({
        rel: 'filter',
        href: API_SERVER + '/subscriptions/',
        prompt: 'Filter by createdOn or canceledOn',
        data: [
            {
                name: 'createdOn',
                value: '',
                prompt: ' Month of First Susbcription'
            },
            {
                name: 'canceledOn',
                value: '',
                prompt: ' Month of Cancellation'
            }
        ]
    })
    queries.push({
        rel: 'filter',
        href: API_SERVER + '/subscriptions/',
        prompt: 'Filter by plan',
        data: [
            {
                name: 'planID',
                value: '',
                prompt: 'Plan'
            }
        ]
    })

    return queries
}

// inspired in code in the A Board For You repository from Paulo Gandra de Sousa.
function createSubscriptionTemplate() {
    return {
        data: [
            {
                name: 'user',
                value: '',
                prompt: 'User'
            },
            {
                name: 'plan',
                value: '',
                prompt: 'Plan'
            },
            {
                name: 'type',
                value: 'montly|annual',
                prompt: 'Type'
            }
        ]
    }
}

function deviceHyperMedia(device, userRole, subscriptionID) {
    const links = []
    const self =
        API_SERVER + 'subscriptions/' + subscriptionID + '/devices/' + device.id
    links.push({
        rel: 'self',
        href: self
    })
    links.push({
        rel: 'subscription',
        href: self + '/subscription/' + subscriptionID
    })
    links.push({
        rel: 'delete',
        href: self
    })
    links.push(
        {
            rel: 'edit',
            href: self
        }
    )
    return links;
}

function devicesCollectionHypermedia(subscriptionID) {
    const links = []
    const self = API_SERVER + '/subscriptions/' + subscriptionID + '/devices'
    links.push({
        rel: 'self',
        href: self
    })
    links.push({
        rel: 'subscription',
        href: API_SERVER + '/subscriptions/' + subscriptionID
    })
    links.push(
        {
            rel: 'create',
            href: self
        }
    )
    return links;
}

function createDeviceTemplate() {
    return {
        data: [
            {
                name: 'name',
                value: '',
                prompt: 'Name'
            },
            {
                name: 'description',
                value: '',
                prompt: 'Description'
            }
        ]
    }
}

// TODO: generalize the code to avoid if statements
// inspired in code in the A Board For You repository from Paulo Gandra de Sousa.
function formatResponseCollection(response, out, req, entity) {
    return response.format({
        'application/json': () => {
            if (entity === 'devices') {
                return response.ok(
                    out,
                    responseHelper.linkHeader(
                        devicesCollectionHypermedia(
                            req.params.subscriptionID,
                            out.data
                        )
                    )
                )
            }
            return response.ok(
                out.data,
                responseHelper.linkHeader(
                    createLinksOfCollection(
                        +req.query.start,
                        +req.query.n,
                        out.isLast,
                        req.query.user,
                        req.query.text
                    )
                )
            )
        },
        'application/vnd.pagsousa.hyper+json': () => {
            if (entity === 'devices') {
                return response.ok(
                    Hyper.collection(
                        out,
                        deviceHyperMedia,
                        devicesCollectionHypermedia(req.params.subscriptionID),
                        createDeviceTemplate(),
                        null,
                        req.params.subscriptionID
                    )
                )
            }
            return response.ok(
                Hyper.collection(
                    out.data,
                    createLinkHypermediaOfItem,
                    createLinksOfCollection(
                        +req.query.start,
                        +req.query.n,
                        out.isLast,
                        req.query.user,
                        req.query.text
                    ),
                    createSubscriptionTemplate(),
                    createQueriesOfCollection()
                )
            )
        }
    })
}

function createPlanHypermedia(dataObj, userRole, subscriptionID, links) {
    // split the links through the comma
    var linksArray = links.split(',')
    var arrayLink = []
    for (var i = 0; i < linksArray.length; i++) {
        var link = linksArray[i]
        var linkArray = link.split(';')
        var linkObj = {
            rel: linkArray[1].substring(5, linkArray[1].length - 1),
            href: linkArray[0].substring(1, linkArray[0].length - 1)
        }
        arrayLink.push(linkObj)
    }
    return arrayLink
}

function handleGetSubscriptionUser(req, res) {
    const response = new Response(res)
    // Get loged user roles and check if he has permissions
    const roles = req.headers.roles
    const allowed = [ADMIN, SUBSCRIBER]
    if (checkSomeInArray(roles, allowed)) {
        subscriptionService
            .getSubscriptionUser(req.subscriptionID,req.headers.username)
            .then((user) => {
                return formatResponse(response, {
                    data: user,
                    created: false,
                    entity: 'user'
                },
                req.headers.roles)
            })
            .catch((error) => {
                return response.error(error, (s) => s, etagHeader)
            })
    } else {
        return response.notAllowed(
            'You do not have permissions to perform this action'
        )
    }
}

// TODO: Generalize the hypermedia to avoid the if conditions
// inspired in code in the A Board For You repository from Paulo Gandra de Sousa.
function formatResponse(response, entry, userRole) {
    return response.format({
        'application/json': () => {
            if (entry.created) {
                if (entry.entity === 'subscription') {
                    return response.created(
                        entry.data,
                        SERVER_ROOT +
                            '/subscriptions/' +
                            entry.data.subscription.id,
                        responseHelper.linkHeader(
                            createLinkHypermediaOfItem(
                                entry.data.subscription,
                                userRole
                            )
                        )
                    )
                }
                if (entry.entity === 'device') {
                    return response.created(
                        entry.data,
                        SERVER_ROOT +
                            '/subscriptions/' +
                            entry.data.device.subscription.id +
                            '/devices/' +
                            entry.data.device.id,
                        responseHelper.linkHeader(
                            deviceHyperMedia(
                                entry.data.device.subscription.id,
                                entry.data.device
                            )
                        )
                    )
                }
            } else {
                if (entry.entity === 'device') {
                    return response.ok(
                        entry.data,
                        responseHelper.linkHeader(
                            deviceHyperMedia(
                                entry.data.subscription.id,
                                entry.data
                            )
                        )
                    )
                }
                if (entry.entity === 'plan') {
                    return response.ok(entry.data.data, responseHelper.linkHeader(entry.data.links));
                }
                if (entry.entity === 'user') {
                    return response.ok(entry.data.data, responseHelper.linkHeader(createLinkHypermediaOfItem(entry.data,userRole)));
                }
                return response.ok(entry.data, responseHelper.linkHeader(createLinkHypermediaOfItem(entry.data,userRole)));
            }
        },
        'application/vnd.pagsousa.hyper+json': () => {
            if (entry.created) {
                if (entry.entity === 'subscription') {
                    return response.created(
                        Hyper.item(
                            entry.data.subscription,
                            createLinkHypermediaOfItem,
                            userRole
                        ),
                        SERVER_ROOT +
                            '/subscriptions/' +
                            entry.data.subscription.id
                    )
                }
                if (entry.entity === 'device') {
                    return response.created(
                        Hyper.item(
                            entry.data.device,
                            deviceHyperMedia,
                            null,
                            entry.data.device.subscription.id
                        ),
                        SERVER_ROOT +
                            '/subscriptions/' +
                            entry.data.device.subscription.id +
                            '/devices/' +
                            entry.data.device.id
                    )
                }
            } else {
                if (entry.entity === 'device') {
                    return response.ok(
                        Hyper.item(
                            entry.data,
                            deviceHyperMedia,
                            null,
                            entry.data.subscription.id
                        )
                    )
                }
                if (entry.entity === 'plan') {
                    return response.ok(
                        Hyper.item(
                            entry.data.data,
                            createPlanHypermedia,
                            null,
                            null,
                            undefined,
                            entry.data.links
                        )
                    )
                }
                if (entry.entity === 'user') {
                    return response.ok(Hyper.item(entry.data.data, createPlanHypermedia, null, null,undefined,(entry.data.headers.link)));
                }
                return response.ok(Hyper.item(entry.data, createLinkHypermediaOfItem,userRole));
            }
        }
    })
}


function checkSomeInArray(array, allowed) {
    if (!array || !allowed || array.length === 0 || allowed.length === 0) {
        return false
    }
    // Explicit casting to avoid errors
    const arr = Array.from(array.split(','))
    const allow = Array.from(allowed)
    for (let i = 0; i < arr.length; i++) {
        if (_.contains(allow, arr[i])) return true
    }
    return false
}

// Helper method to check if requesting user is requesting something that is not theirs
function checkIfHasPermsToRequest(username, roles, userID) {
    if (!roles.includes(ADMIN)) {
        if (userID !== username) {
            return false
        }
    }
    return true
}

// !SUBSCRIPTION

async function handleGetSubscriptions(req, res) {
    const response = new Response(res)
    if (!req.query.start) {
        req.query.start = 1
    }
    if (!req.query.n) {
        req.query.n = 5
    }
    // Get loged user roles and check if he has permissions
    const roles = req.headers.roles
    const allowed = [ADMIN]
    if (checkSomeInArray(roles, allowed)) {
        // User has permissions
        try {
            const subscriptions = await subscriptionService.getSubscriptions(
                req.headers.username,
                req.headers.roles,
                req.query.createdOn,
                req.query.canceledOn,
                req.query.year,
                req.query.planID,
                0 + req.query.start,
                0 + req.query.n
            )
            formatResponseCollection(
                response,
                subscriptions,
                req,
                'subscriptions'
            )
        } catch (error) {
            return response.error(error, (s) => s, etagHeader)
        }
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

async function handlePostSubscription(req, res) {
    const response = new Response(res)
    const roles = req.headers.roles
    const allowed = [USER, MARKETING_DIRECTOR]
    if (checkSomeInArray(roles, allowed)) {
        subscriptionService
            .addSubscriptionToAPlan(req.body)
            .then((subscription) => {
                return formatResponse(
                    response,
                    {
                        data: subscription,
                        created: true,
                        entity: 'subscription'
                    },
                    'subscriber'
                )
            })
            .catch((error) => {
                return response.error(error, (s) => s, etagHeader)
            })
    }
}
async function handlePatchSubscriptions(req, res) {
    const response = new Response(res)
    // add pagination
    if (!req.query.start) {
        req.query.start = 1
    }
    if (!req.query.n) {
        req.query.n = 5
    }
    // check if user has permissions admin
    const roles = req.headers.roles
    const allowed = [ADMIN, MARKETING_DIRECTOR, SUBSCRIBER]
    if (checkSomeInArray(roles, allowed)) {
        if (!req.query.planID) {
            return response.badRequest(
                'You must pass a planID in the query params'
            )
        }
        try {
            const subscription =
                await subscriptionService.migrateSubscriptionsOfPlan(
                    req.query.planID,
                    req.body.planID,
                    0 + req.query.start,
                    0 + req.query.n
                )
            return formatResponseCollection(
                response,
                subscription,
                req,
                'subscriptions'
            )
        } catch (error) {
            return response.error(error, (s) => s, etagHeader)
        }
    }
}

async function handleGetSubscription(req, res) {
    const response = new Response(res)
    // Get loged user roles and check if he has permissions
    const roles = req.headers.roles
    const allowed = [ADMIN, SUBSCRIBER]
    if (checkSomeInArray(roles, allowed)) {
        try {
            const subscription = await subscriptionService.getSubscriptionById(
                req.subscriptionID
            )
            return formatResponse(
                response,
                {
                    data: subscription,
                    created: false,
                    entity: 'subscription'
                },
                req.headers.roles
            )
        } catch (error) {
            return response.error(error, (s) => s, etagHeader)
        }
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

async function handleDeactivateSubscription(req, res) {
    req.body = { status: 'deactivated' }
    return handlePatchSubscription(req, res)
}

async function handleRenewSubscription(req, res) {
    const response = new Response(res)
    const revision = req.headers['if-match']
    if (!revision) {
        return response.preconditionFailed('Missing if-match header')
    }
    // Get loged user roles and check if he has permissions
    const roles = req.headers.roles
    const allowed = [ADMIN, SUBSCRIBER]

    const username = req.headers.username
    if (checkSomeInArray(roles, allowed)) {
        // User has permissions
        try {
            // Get subscription with id
            let subscription = await subscriptionService.getSubscriptionById(
                req.subscriptionID
            )
            if (!subscription.user.id === username) {
                return response.forbiden(
                    'You can only request your subscription'
                )
            }
            if (revision !== subscription.__v.toString()) {
                return response.preconditionFailed(
                    'Wrong if-match header. Please refresh'
                )
            }
            subscription = await subscriptionService.renewSubscription(
                subscription
            )
            return formatResponse(
                response,
                {
                    data: subscription,
                    created: false,
                    entity: 'subscription'
                },
                req.headers.roles
            )
        } catch (error) {
            return response.error(error, (s) => s, etagHeader)
        }
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

async function handlePatchSubscription(req, res) {
    const revision = req.headers['if-match']
    const response = new Response(res)
    if (!revision) {
        return response.badRequest('Missing if-match header')
    }
    // Get loged user roles and check if he has permissions
    const roles = req.headers.roles
    const allowed = [ADMIN, SUBSCRIBER]
    if (checkSomeInArray(roles, allowed)) {
        // User has permissions
        try {
            // Get subscription with id
            const subscription = await subscriptionService.getSubscriptionById(
                req.subscriptionID
            )
            // Check if user owns subscription
            if (
                !checkIfHasPermsToRequest(
                    req.headers.username,
                    roles,
                    subscription.user
                )
            ) {
                return response.forbiden(
                    'You can only request your subscription'
                )
            }
            // Update subscription
            const updated = await subscriptionService.partialUpdateSubscription(
                req.body,
                req.subscriptionID,
                revision
            )
            return formatResponse(
                response,
                {
                    data: updated,
                    created: false,
                    entity: 'subscription'
                },
                req.headers.roles
            )
        } catch (error) {
            return response.error(error, (s) => s, etagHeader)
        }
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

function handlePutSubscription(req, res) {
    const response = new Response()
    return response.notAllowed(
        'Use POST /subscriptions to register a new subscription'
    )
}

// !PLAN

function handleGetSubscriptionPlan(req, res) {
    const response = new Response(res)
    // Get loged user roles and check if he has permissions
    const roles = req.headers.roles
    const allowed = [ADMIN, SUBSCRIBER]
    if (checkSomeInArray(roles, allowed)) {
        subscriptionService
            .getSubscriptionPlan(req.subscriptionID, req.headers.username)
            .then((plan) => {
                return formatResponse(
                    response,
                    {
                        data: plan,
                        created: false,
                        entity: 'plan'
                    },
                    req.headers.roles
                )
            })
            .catch((error) => {
                return response.error(error, (s) => s, etagHeader)
            })
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

async function handlePatchSubscriptionPlan(req, res) {
    const response = new Response(res)
    var rev = req.headers['if-match']
    const roles = req.headers.roles
    const allowed = [SUBSCRIBER]
    if (checkSomeInArray(roles, allowed)) {
        try {
            const subscription =
                await subscriptionService.upgradeOrDowngradeSubscriptionPlan(
                    req.subscriptionID,
                    req.body.planID,
                    req.headers.username,
                    rev
                )
            return formatResponse(
                response,
                {
                    data: subscription,
                    created: false,
                    entity: 'subscription'
                },
                req.headers.roles
            )
        } catch (error) {
            return response.error(error, (s) => s, etagHeader)
        }
    }
    return response.notAllowed(
        'You do not have permissions to perform this action'
    )
}

// !DEVICES

function handleGetSubscriptionDevices(req, res) {
    const response = new Response(res)
    // Get loged user roles and check if he has permissions
    const roles = req.headers.roles
    const allowed = [SUBSCRIBER]
    if (checkSomeInArray(roles, allowed)) {
        subscriptionService
            .getDevicesOfSubscription(req.subscriptionID, req.headers.username)
            .then((devices) => {
                return formatResponseCollection(
                    response,
                    devices,
                    req,
                    'devices'
                )
            })
            .catch((error) => {
                return response.error(error, (s) => s, etagHeader)
            })
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

function handleDeleteDeviceFromSubscription(req, res) {
    var rev = req.headers['if-match']
    const response = new Response(res)
    // Get loged user roles and check if he has permissions
    const roles = req.headers.roles
    const allowed = [SUBSCRIBER]
    if (checkSomeInArray(roles, allowed)) {
        subscriptionService
            .deleteDeviceFromSubscription(
                req.subscriptionID,
                req.deviceID,
                req.headers.username,
                rev
            )
            .then(() => {
                return response.ok('Device ' + req.deviceID + ' deleted.')
            })
            .catch((error) => {
                return response.error(error, (s) => s, etagHeader)
            })
    } else {
        return response.notAllowed(
            'You do not have permissions to perform this action'
        )
    }
}

function handlePatchDevice(req, res) {
    var rev = req.headers['if-match']
    const response = new Response(res)
    // Get loged user roles and check if he has permissions
    const roles = req.headers.roles
    const allowed = [SUBSCRIBER]
    if (checkSomeInArray(roles, allowed)) {
        subscriptionService
            .updateDevice(
                req.body,
                req.deviceID,
                req.subscriptionID,
                req.headers.username,
                rev
            )
            .then((device) => {
                return formatResponse(
                    response,
                    {
                        data: device,
                        created: false,
                        entity: 'device'
                    },
                    req.headers.roles
                )
            })
            .catch((error) => {
                return response.error(error, (s) => s, etagHeader)
            })
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

function handlePostDevice(req, res) {
    const response = new Response(res)
    // Get loged user roles and check if he has permissions
    const roles = req.headers.roles
    const allowed = [SUBSCRIBER]
    if (checkSomeInArray(roles, allowed)) {
        subscriptionService
            .addDeviceToSubscription(
                req.body,
                req.subscriptionID,
                req.headers.username
            )
            .then((device) => {
                return formatResponse(
                    response,
                    {
                        data: device,
                        created: true,
                        entity: 'device'
                    },
                    req.headers.roles
                )
            })
            .catch((error) => {
                return response.error(error, (s) => s, etagHeader)
            })
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

// !Dashboard
async function handleGetDashboard(req, res) {
    const response = new Response(res)
    const roles = req.headers.roles
    // TODO: The role should not be admin bust product-manager, but since we do not have a user with this role yet, we use admin for the moment.
    const allowed = [ADMIN]
    if (checkSomeInArray(roles, allowed)) {
        try {
            const dashboard = await subscriptionService.getDashboard(
                req.query.month
            )
            return response.ok(dashboard)
        } catch (error) {
            return response.error(error, (s) => s, etagHeader)
        }
    }
    return response.notAllowed(
        'You do not have permissions to perform this action'
    )
}

exports.SubscriptionsHandler = function () {
    return {
        handleGetSubscriptions,
        handleGetSubscription,
        handlePostSubscription,
        handlePatchSubscription,
        handleGetSubscriptionPlan,
        handlePostDevice,
        handleGetSubscriptionDevices,
        handlePutSubscription,
        handleDeleteDeviceFromSubscription,
        handlePatchDevice,
        handleDeactivateSubscription,
        handleRenewSubscription,
        handlePatchSubscriptionPlan,
        handlePatchSubscriptions,
        handleGetDashboard,
        handleGetSubscriptionUser
    }
}
