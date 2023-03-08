const axios = require('axios')

const port = process.env.PORT || process.argv[2] || 3004
const SERVER = 'localhost:' + port
const SERVER_ROOT = 'http://' + SERVER

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

const NodeCache = require('node-cache')

// caching the subscriptions for two hours to reduce server load
const cache = new NodeCache({ stdTTL: 1 * 30 })

async function getSubscriptionsByPlan(plan, username, roles) {
    const key = '/Subscriptions?planID=' + plan
    if (cache.has(key)) {
        return cache.get(key)
    }
    return new Promise((resolve, reject) => {
        axios
            .get(SERVER_ROOT + '/Subscriptions?planID=' + plan, { headers: { username, roles } }, {
                retry: 3,
                retryDelay: 3000
            })
            .then((response) => {
                cache.set(key, response.data)
                resolve(response.data)
            })
            .catch((error) => {
                reject(error)
            })
    })
}

exports.SubscriptionService = {
    getSubscriptionsByPlan
}
