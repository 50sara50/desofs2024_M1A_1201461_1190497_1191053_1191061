require('dotenv').config()
const request = require('request')

const post_request = (url, body) =>
    new Promise((resolve, reject) => {
        request.post(
            {
                url: url,
                json: body
            },
            (err, res, body) => {
                console.log(err);
                if (!err) {
                    resolve(body)
                } else {
                    reject(err)
                }
            }
        )
    })

exports.PostRequest = function () {
    return {
        post_request: post_request
    }
}
