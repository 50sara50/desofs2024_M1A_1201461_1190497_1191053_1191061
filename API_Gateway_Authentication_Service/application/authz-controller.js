'use-strict'
require('dotenv').config()
const exceptions = require('../exceptions')
const requester = require('../Http/post-request').PostRequest()
const axios = require('axios')

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

async function login(username, password) {
    if (!username || !password) {
        // TODO Add logger
        console.log('Malformed login')
        return Promise.reject(
            new RangeError('Missing username and/or password parameter')
        )
    } else {
        const url = process.env.USER_SERVICE + '/users/login'
        try {
            const user = await requester.post_request(url, { username })
            if (user) {
                if (user.password === password) {
                    return Promise.resolve(user)
                } else {
                    // Wrong password
                    return Promise.resolve(false)
                }
            } else {
                // INvalid username
                return Promise.resolve(false)
            }
        } catch (error) {
            return Promise.reject(error)
        }
    }
}

async function findUser(username) {
    return new Promise((resolve, reject) => {
        axios
            .get(process.env.USER_SERVICE + '/Users/' + username, {
                retry: 3,
                retryDelay: 3000
            })
            .then((response) => {
                resolve(response.data)
            })
            .catch((error) => {
                reject(error)
            })
    })
}

exports.AuthzController = function () {
    return {
        login,
        findUser
    }
}
