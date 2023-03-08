'use strict'

//
// a node module with utilities
//

function myOmit(inObj) {
    const ret = {}

    Object.keys(inObj).forEach(function (k) {
        if (k !== '_rev') {
            ret[k] = inObj[k]
        }
    })
    return ret
}

exports.sendJSON = function (res, code, content, headers) {
    res.status(code)

    if (headers) {
        Object.keys(headers).forEach(function (h) {
            res.setHeader(h, headers[h])
        })
    }

    if (content._rev && !res.getHeader('ETag')) {
        res.setHeader('ETag', content._rev.toString())
    }
    res.json(myOmit(content))
}

exports.send = function (res, code, content, headers) {
    res.status(code)

    if (headers) {
        Object.keys(headers).forEach(function (h) {
            res.setHeader(h, headers[h])
        })
    }

    res.send(content)
}

/**
 * returns a string which can be used as id for a message
 * @param {String} [sufix]
 */
exports.createID = function (length) {
    return Math.random()
        .toString(36)
        .substring(2, length + 2)
}

function padTo2Digits(num) {
    return num.toString().padStart(2, '0')
}

exports.formatDate = function (date) {
    return [
        padTo2Digits(date.getDate()),
        padTo2Digits(date.getMonth() + 1),
        date.getFullYear()
    ].join('/')
}
