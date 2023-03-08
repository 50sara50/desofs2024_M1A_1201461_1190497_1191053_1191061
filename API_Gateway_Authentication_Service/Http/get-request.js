require('dotenv').config()
const axios_get = require('./Axios/Axios_Get')

function get_request(url) {
    let client = null
    const http_client = process.env.HTTP_CLIENT
    switch (http_client) {
        case 'axios':
            client = new Axios_Get(url)
            return client.get_axios_request().then((res) => {
                return res
            })
    }
}

exports.GetRequest = function () {
    return {
        get_request: get_request
    }
}
