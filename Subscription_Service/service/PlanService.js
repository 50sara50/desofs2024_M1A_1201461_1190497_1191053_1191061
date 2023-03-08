const axios = require('axios');

const port = process.env.PORT || process.argv[2] || 3003
const SERVER = 'localhost:' + port
const SERVER_ROOT = 'http://' + SERVER

// interceptor to retry failed requests due to network timeout or network error
// retrieved from https://javascript.plainenglish.io/how-to-retry-requests-using-axios-64c2da8340a7
axios.interceptors.response.use(undefined, (err) => {
    const { config, message } = err;
    if (!config || !config.retry) {
        return Promise.reject(err);
    }
    // retry while Network timeout or Network Error
    if (!(message.includes('timeout') || message.includes('Network Error'))) {
        return Promise.reject(err);
    }
    config.retry -= 1;
    const delayRetryRequest = new Promise((resolve) => {
        setTimeout(() => {
            console.log('retry the request', config.url);
            resolve();
        }, config.retryDelay || 1000);
    });
    return delayRetryRequest.then(() => axios(config));
});

const NodeCache = require('node-cache');

// Caching the plans for two hours to reduce server load ... FIXME ---maybe increase to 24 hours or more
// const cache = new NodeCache({ stdTTL: 1 * 60 * 120 });
const cache = new NodeCache({ stdTTL: 1 * 30 });

// TODO: Alternate Server Root with a load balancer, and maybe implement a timeout
async function getPlanById(id) {
    const key = '/Plans/' + id;
    if (cache.has(key)) {
        return cache.get(key);
    }

    try {
        const response = await axios.get(SERVER_ROOT + '/Plans/' + id, { retry: 3, retryDelay: 3000 })
        cache.set(key, { data: response.data, links: response.headers.link });
        return Promise.resolve({ data: response.data, links: response.headers.link });
    } catch (error) {
        return Promise.resolve(null);
    }
}

// TODO: Consider implement caching in way that allows to reuse this cache in the cache of userID
async function getPlans(username, roles,start,n) {
    const key = '/Plans?start=' + start + '&n=' + n;
    if (cache.has(key)) {
        return cache.get(key);
    }
    return new Promise((resolve, reject) => {
        axios.get(SERVER_ROOT + '/Plans',{
            params: {
                start,
                n
            },
            headers: {
                username,
                roles
            },
            retry: 3,
            retryDelay: 3000
        })
            .then((response) => {
                cache.set(key, { data: response.data, headers: response.headers });
                resolve({ data: response.data, headers: response.headers })
            })
            .catch((error) => {
                reject(error)
            })
    }
    )
}


exports.PlanService = {
    getPlanById,
    getPlans
}
