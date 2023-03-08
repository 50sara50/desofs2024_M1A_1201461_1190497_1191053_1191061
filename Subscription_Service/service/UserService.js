const axios = require('axios')
const port = process.env.PORT || process.argv[2] || 3001
const SERVER = 'localhost:' + port
const SERVER_ROOT = 'http://' + SERVER

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

const NodeCache = require('node-cache')

// Caching the plans for 30 minutos to reduce server load ... FIXME ---maybe increase time to a little more than 30 minutes
const cache = new NodeCache({ stdTTL: 1 * 60 * 30 })

// TODO: Alternate Server Root with a load balancer, and maybe implement a timeout
async function getUserById(id) {
    const key = '/Users/' + id
    if (cache.has(key)) {
        return cache.get(key)
    }
    try {
        const response = await axios.get(SERVER_ROOT + '/Users/' + id, {
            retry: 3,
            retryDelay: 3000
        })
        cache.set(key, { data: response.data, headers: response.headers });
        return { data: response.data, headers: response.headers }
    } catch (error) {
        return null
    }
}

// TODO: Consider implement caching in way that allows to reuse this cache in the cache of userID
async function getUsers(start,n) {
    const key = '/Users?start=' + start + '&n=' + n;
    if (cache.has(key)) {
        return cache.get(key)
    }
    return new Promise((resolve, reject) => {
        axios
            .get(SERVER_ROOT + '/Users', {
                params: {
                    start,
                    n
                },
                retry: 3,
                retryDelay: 3000
            })
            .then((response) => {
                cache.set(key, { data: response.data, links: response.headers.link });
                resolve({ data: response.data, links: response.headers.link });
            })
            .catch((error) => {
                reject(error)
            })
    })
}

async function promoteUserToSubscriber(username, etag) {
    return new Promise((resolve, reject) => {
        axios
            .patch(
                SERVER_ROOT + '/Users/' + username,
                { roles: ['subscriber'] },
                // eslint-disable-next-line quote-props
                {
                    headers: { username, 'if-match': etag, roles: ['admin'] },
                    retry: 3,
                    retryDelay: 3000
                }
            )
            .then((response) => {
                resolve(response.data)
            })
            .catch((error) => {
                reject(error)
            })
    })
}

exports.UserService = {
    getUserById,
    getUsers,
    promoteUserToSubscriber
}
