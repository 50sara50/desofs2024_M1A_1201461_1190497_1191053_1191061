const axios = require('axios')
const _ = require('underscore')
const subscriptionService =
    require('../application/SubscriptionController').SubscriptionService()
const subscriptionRepository =
    require('../persistence/subscriptionRepository').SubscriptionRepository
const plansService = require('../service/PlanService').PlanService

// retrieved from https://javascript.plainenglish.io/how-to-retry-requests-using-axios-64c2da8340a7
axios.interceptors.response.use(undefined, (err) => {
    const { config, message } = err
    if (!config || !config.retry) {
        return Promise.reject(err)
    }
    // retry while Network timeout or Network Error
    if (!(message.includes('timeout') || message.includes('Network Error'))) {
        return Promise.reject(err)
    }
    config.retry -= 1
    const delayRetryRequest = new Promise((resolve) => {
        setTimeout(() => {
            console.log('retry the request', config.url)
            resolve()
        }, config.retryDelay || 1000)
    })
    return delayRetryRequest.then(() => axios(config))
})

async function postBack(url, content) {
    return new Promise((resolve, reject) => {
        axios
            .post(url, content)
            .then((response) => {
                resolve(response.data)
            })
            .catch((error) => {
                reject(error)
            })
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

async function getCanceledSubs(month, year, plan) {
    return (
        await subscriptionRepository.getSubscriptions(null, month, year, plan)
    ).data
}

async function getNewSubs(month, year, plan) {
    return (
        await subscriptionRepository.getSubscriptions(month, null, year, plan)
    ).data
}

async function getActiveSubs(month, year, plan) {
    const subs = await subscriptionRepository.getActiveSubscriptions(
        month,
        year,
        plan
    )
    return subs
}

async function getMonthlyRevenue(month, year, plan) {
    const revenue = await subscriptionRepository.getMonthlyRevenue(
        month,
        year,
        plan
    )
    return revenue
}

function calculateChurn(canceled, active, newSubs) {
    if (active == 0) return 0
    return +(((active - (active + newSubs - canceled)) / active) * 100).toFixed(
        2
    )
}

exports.Utils = {
    getActiveSubs,
    getCanceledSubs,
    getMonthlyRevenue,
    getNewSubs,
    calculateChurn,
    postBack,
    checkSomeInArray
}
