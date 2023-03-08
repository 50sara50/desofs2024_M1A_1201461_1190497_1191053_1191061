const subscriptions = require('./subscriptionsDB').data.subscriptions

function toMonthName(monthNumber) {
    const date = new Date()
    date.setMonth(monthNumber - 1)

    return date.toLocaleString('en-US', {
        month: 'short'
    })
}

function saveSubscription(subscription) {
    try {
        subscription.__v = (parseInt(subscription.__v) + 1).toString()
        subscriptions[subscription.id] = subscription
        return Promise.resolve(subscription)
    } catch (error) {
        return Promise.reject(subscription)
    }
}

function filterInDates(obj, field, value, year) {
    const out = []
    Object.keys(obj).forEach((k) => {
        if (obj[k][field] != null) {
            if (year) {
                if (
                    obj[k][field].getMonth() == value &&
                    obj[k][field].getFullYear() == year
                ) {
                    out[k] = obj[k]
                }
            } else {
                if (obj[k][field].getMonth() == value) out[k] = obj[k]
            }
        }
    })
    return out
}

function partialUpdateSubscription(subscription, changes) {
    Object.assign(subscription, changes)
    return saveSubscription(subscription)
}

function filterInObject(obj, field, value, start = 1, n = 5) {
    const out = {}
    const keys = Object.keys(obj)
    const slicedKeys = keys.slice(start - 1, start + n - 1)
    slicedKeys.forEach((k) => {
        if (obj[k][field].includes(value)) out[k] = obj[k]
    })
    return out
}

function pushObjsToArray(obj, array, start, n) {
    const keys = Object.keys(obj)
    if (start || n) {
        start = parseInt(start)
        n = parseInt(n)
        const slicedKeys = keys.slice(start - 1, start + n - 1)
        slicedKeys.forEach((k) => {
            array.push(obj[k])
        })
    } else {
        keys.forEach((k) => {
            array.push(obj[k])
        })
    }
}

function getSubscription(id) {
    return Promise.resolve(subscriptions[id])
}

function searchSubscriptions({ createdOn, canceledOn }) {
    let data = subscriptions
    if (createdOn) {
        data = filterInDates(data, 'createdOn', createdOn)
    }

    if (canceledOn) {
        data = filterInDates(data, 'cancelationDate', canceledOn)
    }
    return data
}

async function getActiveSubscriptions(month, year, plan) {
    const subscriptionsArray = []
    const data = subscriptions
    const date = new Date(year, month)

    const out = []
    Object.keys(data).forEach((k) => {
        if (data[k].plan == plan) {
            if (data[k].cancelationDate == null && data[k].createdOn < date) {
                subscriptionsArray.push(data[k])
            } else if (
                data[k].cancelationDate > date &&
                data[k].createdOn < date
            ) {
                subscriptionsArray.push(data[k])
            }
        }
    })
    return Promise.resolve(subscriptionsArray)
}

async function getMonthlyRevenue(month, year, plan) {
    const activeSubs = await getActiveSubscriptions(month, year, plan)
    let revenue = 0
    activeSubs.forEach((sub) => {
        if (sub.type == 'monthly') {
            revenue += parseFloat(sub.subscriptionFee)
        } else if (sub.type == 'annual') {
            revenue += parseFloat(sub.subscriptionFee) / 12
        }
    })
    // Round 2 decimals
    revenue = +revenue.toFixed(2)
    return Promise.resolve(revenue)
}

async function getMonthlyRevenueCashflow(month, year, plan) {
    const subs = await getSubsWithRenewDate(month, year, plan)
    let revenue = 0
    subs.forEach((subscription) => {
        revenue += +(parseFloat((subscription.subscriptionFee) / 12).toFixed(2))
    })
    return revenue
}

async function getSubsWithRenewDate(month, year, plan) {
    const subs = await getActiveSubscriptions(month, year, plan)
    const result = []
    const date = new Date(year, month)

    subs.forEach((subscription) => {
        if (subscription.type == 'annual') {
            if (subscription.renewDate > date) {
                result.push(subscription)
            }
        }
    })
    return result
}

function getSubscriptions(
    createdOn,
    canceledOn,
    year,
    planID,
    start,
    n
) {
    const subscriptionsArray = []
    let data = subscriptions
    if (createdOn) {
        if (year) {
            data = filterInDates(data, 'createdOn', createdOn, year)
        } else {
            data = filterInDates(data, 'createdOn', createdOn, null)
        }
    }

    if (canceledOn) {
        if (year) {
            data = filterInDates(data, 'cancelationDate', canceledOn, year)
        } else {
            data = filterInDates(data, 'cancelationDate', canceledOn, null)
        }
    }

    if (planID) {
        data = filterInObject(data, 'plan', planID, start, n)
    }

    pushObjsToArray(data, subscriptionsArray, start, n)
    const totalPages = Math.ceil(Object.keys(data).length / n)
    return Promise.resolve({
        isLast: start >= totalPages,
        data: subscriptionsArray
    })
}

function addSubscriptionToAPlan(
    subscriptionID,
    userID,
    planID,
    type,
    subscriptionFee
) {
    const subscription = {
        id: subscriptionID,
        plan: planID,
        user: userID,
        createdOn: new Date(),
        type,
        renewDate: new Date(new Date().setMonth(new Date().getMonth() + 1)),
        status: 'active',
        subscriptionFee,
        cancelationDate: undefined,
        __v: 0
    }
    subscriptions[subscription.id] = subscription
    return Promise.resolve(subscription)
}

function getSubscriptionOfPlan(planID) {
    const subscriptionArray = []
    Object.keys(subscriptions).forEach((k) => {
        if (subscriptions[k].plan == planID) {
            subscriptionArray.push(subscriptions[k])
        }
    })
    return Promise.resolve(subscriptionArray)
}

// Assuming that a user can have only one subscription at a time
function getSubscriptionOfAnUser(userID) {
    Object.keys(subscriptions).forEach((k) => {
        if (subscriptions[k].user == userID) {
            return Promise.resolve(true)
        }
    })
    return Promise.resolve(false)
}

function updateSubscriptionPlan(subscriptionID, planID) {
    subscriptions[subscriptionID].plan = planID
    saveSubscription(subscriptions[subscriptionID])
    return Promise.resolve(subscriptions[subscriptionID])
}

exports.SubscriptionRepository = {
    saveSubscription,
    getSubscription,
    getSubscriptions,
    getSubscriptionOfAnUser,
    addSubscriptionToAPlan,
    partialUpdateSubscription,
    updateSubscriptionPlan,
    getSubscriptionOfPlan,
    searchSubscriptions,
    getActiveSubscriptions,
    getMonthlyRevenue,
    getMonthlyRevenueCashflow
}
