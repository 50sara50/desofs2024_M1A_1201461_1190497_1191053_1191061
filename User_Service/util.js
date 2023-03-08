
"use strict";

//
// a node module with utilities
//

function myOmit(inObj) {
    var ret = {};

    Object.keys(inObj).forEach(function (k) {
        if (k !== '_rev') {
            ret[k] = inObj[k];
        }
    });
    return ret;
}

exports.sendJSON = function (res, code, content, headers) {
    res.status(code);

    if (headers) {
        Object.keys(headers).forEach(function (h) {
            res.setHeader(h, headers[h]);
        });
    }

    if (content._rev && !res.getHeader("ETag")) {
        res.setHeader("ETag", content._rev.toString());
    }
    res.json(myOmit(content));
};

exports.send = function (res, code, content, headers) {
    res.status(code);

    if (headers) {
        Object.keys(headers).forEach(function (h) {
            res.setHeader(h, headers[h]);
        });
    }

    res.send(content);
};