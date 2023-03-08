/// ////////////////////////////////////////////////////
//
// Paulo Gandra de Sousa
// (c) 2016 - 2019
//
/// ////////////////////////////////////////////////////


/* jslint node: true */
/* jshint esversion:9 */

'use strict';


//
// Response manipulation
//

const _ = require('underscore');

const {
    NotFoundError,
    UnauthorizedError,
    ForbiddenError,
    ConcurrencyError,
    ConflictError,
    BadRequestError
} = require('../exceptions');


// these will be passed to the module thru the configure() function
let API_VERSION;

/**
 * list of fields to filter out and do not send back to the client
 */
const FILTER = ['_id', '__v', 'password'];

/**
 *
 * @param {Object} entry
 * @returns an object with the etag header if there is an "__v" property in entry
 */
function etagHeader(entry) {
    if (entry && entry.__v != null) {
        return {
            ETag: entry.__v.toString()
        };
    } else {
        return {};
    }
}

/**
 *
 * @param {String} url
 * @param {String} rel
 */
function linkHeaderElement(url, rel) {
    return `<${url}>;rel="${rel}"`;
}

/**
 *
 * @param {*} data
 */
function linkHeader(data) {
    if (!data) {
        return {};
    }

    let linkstring = '';
    let links;
    if (Array.isArray(data)) {
        links = data.map(l => linkHeaderElement(l.href, l.rel));
    } else {
        links = Object.keys(data).map(rel => linkHeaderElement(data[rel], rel));
    }
    for (let i = 0; i <= links.length - 2; i++) {
        linkstring = linkstring.concat(links[i], ',');
    }
    linkstring = linkstring.concat(links[links.length - 1]);

    return {
        Link: linkstring
    };
}

/**
 *
 * @param {*[]} data - link array
 */
function linkArrayToObject(data) {
    if (!data) {
        return {};
    }

    let links = {};
    if (Array.isArray(data)) {
        data.forEach(l => {
            links[l.rel] = l.href;
        });
    } else {
        links = data;
    }
    return links;
}

//
// Response helper
//

/**
 * creates a copy of the content but removing certain properties. properly handle objects and object arrays.
 *
 * @param {Object | Object[]} content - the content to clenaup
 * @param {String[]} [filter]         - the properties ot remove
 */
function cleanupContent(content, filter) {
    // filter out implementation properties
    if (!filter) {
        filter = FILTER;
    }
    if (Array.isArray(content)) {
        const out = [];
        content.forEach(e => out.push(_.omit(e.toObject ? e.toObject() : e, filter)));
        return out;
    } else {
        if (content.data) {
            if (Array.isArray(content.data)) {
                const out = [];
                content.data.forEach(e => out.push(cleanupContent(e, filter)));
                content.data = out;
            } else {
                content.data = _.omit(content.data.toObject ? content.data.toObject() : content.data, filter);
            }
        } else {
            content = _.omit(content.toObject ? content.toObject() : content, filter);
        }
        return content;
    }
}

/**
 * ensure the response will contain the necessary headers
 * @param {HttpResponse} res - the response object
 * @param {object} content
 * @param {object} [options] an object with boolean properties for each header to include, i.e., link
 */
function ensureHeaders(res, content, options) {
    if (!options) {
        options = {};
    }

    // ensure Etag header, handle "hypermedia" data property
    if (!res.getHeader('ETag')) {
        if (content.__v != null) {
            res.setHeader('ETag', content.__v.toString());
        } else if (content.data && content.data.__v != null) {
            res.setHeader('ETag', content.data.__v.toString());
        }
    }

    // ensure Link header based on hypermedia "links" or "_links" property
    if (!res.getHeader('Link') && options.link) {
        if (content._links) {
            res.setHeader('Link', linkHeader(content._links).Link);
        } else if (content.links) {
            res.setHeader('Link', linkHeader(content.links).Link);
        }
    }
}

function copyHeaders(res, headers) {
    if (headers) {
        Object.keys(headers).forEach(function (h) {
            res.setHeader(h, headers[h]);
        });
    }
}

/**
 * Response helper Constructor. Wrapps around an existing http response object
 *
 * @param {*} resp - the http response object to wrapp
 */
function Response(resp) {
    const res = resp;

    /**
     * prepares an http response and sends it
     *
     * @param {Number} code http code
     * @param {*} content response body
     * @param {Object} headers http header array
     * @param {string[]} filter
     */
    const send = function (code, content, headers, filter) {
        res.header('X-API-Version', API_VERSION);
        res.status(code);

        copyHeaders(res, headers);

        if (content instanceof Error) {
            res.send(content.toString());
        } else if (content instanceof Object) {
            ensureHeaders(res, content);
            return res.json(cleanupContent(content, filter));
        } else {
            return res.send(content);
        }
    };

    return {
        /**
         * Performs content negotiation
         *
         * @param {*} options
         */
        format: function (options) {
            return res.format(options);
        },

        /**
         * Redirects the client agent to a new url
         *
         * @param {String} url
         */
        redirect: function (url) {
            return res.redirect(url);
        },

        /**
         * sends a text/html response back to the client
         *
         * @param {String} content
         */
        html: function (content) {
            return send(200, content, {
                'content-type': 'text/html'
            });
        },

        badRequest: function (text, headers) {
            return send(400, text, headers);
        },

        serverError: function (text, headers) {
            return send(500, text, headers);
        },

        unauthorized: function (text, headers) {
            return send(401, text, headers);
        },

        forbiden: function (text, headers) {
            return send(403, text, headers);
        },

        notFound: function (text, headers) {
            return send(404, text, headers);
        },

        /**
         * 409 Conflict
         * @param {*} text          - content
         * @param {Object} [headers]  - http headers
         */
        conflict: function (text, headers) {
            return send(409, text, headers);
        },

        /**
         * 412 Precondition Failed
         * @param {*} currentState
         * @param {Object} [headers]  - http headers
         */
        preconditionFailed: function (currentState, headers) {
            return send(412, currentState, headers);
        },

        /**
         * 405 Not Allowed
         * @param {*} text          - content
         * @param {Object} [headers]  - http headers
         */
        notAllowed: function (text, headers) {
            if (!text) {
                text = 'Method is not allowed on this resource.';
            }
            return send(405, text, headers);
        },

        /**
         * 406 Not Acceptable
         * @param {*} text          - content
         * @param {Object} [headers]  - http headers
         */
        notAcceptable: function (text, headers) {
            if (!text) {
                text = "The requested format in the 'accept' header is unacceptable.";
            }
            return send(406, text, headers);
        },

        /**
         * 200 Ok
         * @param {Object | Object[]} data  - object or array to use as response body
         * @param {Object} [headers]        - http headers
         */
        ok: function (data, headers) {
            if (!data) {
                data = {}; // should we return a 204 instead?
            }
            return send(200, data, headers);
        },

        /**
         * 201 Created
         * @param {Object | Object[]} data  - object or array to use as response body
         * @param {String} Location         - location header
         * @param {Object} [headers]        - http headers
         */
        created: function (data, Location, headers) {
            if (!headers) {
                headers = {};
            }
            headers.Location = Location;
            return send(201, data, headers);
        },

        /**
         * 202 Accepted
         * @param {Object | Object[]} data  - object or array to use as response body
         * @param {String} Location         - location header
         * @param {Object} [headers]        - http headers
         */
        accepted: function (data, Location, headers) {
            if (!headers) {
                headers = {};
            }
            headers.Location = Location;
            return send(202, data, headers);
        },

        /**
         * Builds an error response body based on the type of Error that was thrown
         * @param {*} err
         * @param {*} contentBuilder
         * @param {*} headerBuilder
         */
        error: function (err, contentBuilder, headerBuilder) {
            const currentStateContent = () => contentBuilder ? contentBuilder(err.currentState) : err.currentState;
            let content = err.currentState ? currentStateContent() : err.message;
            let headers = headerBuilder ? headerBuilder(err.currentState) : null;

            if (err instanceof RangeError || err instanceof BadRequestError) {
                return this.badRequest(content, headers);
            } else if (err instanceof UnauthorizedError) {
                return this.unauthorized(content, headers);
            } else if (err instanceof ForbiddenError) {
                return this.forbiden(content, headers);
            } else if (err instanceof NotFoundError) {
                return this.notFound(content, headers);
            } else if (err instanceof ConflictError) {
                return this.conflict(content, headers);
            } else if (err instanceof ConcurrencyError) {
                return this.preconditionFailed(content, headers);
            } else {
                // fallback
                return this.serverError(content, headers);
            }
        }
    };
}

/**
 * sends a JSON response
 *
 * @param {*} res
 * @param {*} code
 * @param {*} content
 * @param {*} headers
 */
function sendJSON(res, code, content, headers) {
    res.status(code);
    copyHeaders(res, headers);
    ensureHeaders(res, content);
    res.json(cleanupContent(content));
}

/**
 * sends a text response
 *
 * @param {*} res
 * @param {*} code
 * @param {*} content
 * @param {*} headers
 */
function sendTxt(res, code, content, headers) {
    res.status(code);
    copyHeaders(res, headers);
    return res.send(content);
}


/// //////////////////////////
// MODULE EXPORTS


exports.config = function (config, dependencies) {
    API_VERSION = config.apiVersion;
};

exports.ResponseHelper = {
    sendJSON,
    sendTxt,

    linkHeader,
    etagHeader,

    linkHeaderElement,
    linkArrayToObject,

    cleanupContent
};

exports.PathHandler = {
    handleAsNotAllowed(req, res) {
        let response = new Response(res);
        return response.notAllowed('Method is not allowed on this resource');
    }
};

exports.Response = Response;
