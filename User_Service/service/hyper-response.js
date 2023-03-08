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
// Hypermedia manipulation
//

// const debug = require("debug")("pag:mserver");
const _ = require('underscore');


/// //////////////////////////
// MODULE EXPORTS


// this is quite dependent on mongodb implementation. who should have the responsibility to filter these fields?
const FILTER = ['_id', '__v'];


//
// hypermedia helper
//
exports.Hyper = {
    /**
     *
     * @param {Object} data
     * @param {function(data:Object)} linksBuilder - function
     * @param {String[]} [filter] -
     */
    item: function (dataObj, linksBuilder, role, filter) {
        if (!filter) {
            filter = FILTER;
        }

        return {
            data: dataObj, // _.omit(data.toObject ? data.toObject() : data, filter),
            links: linksBuilder(dataObj, role)
        };
    },

    /**
     *
     * @param {*} data
     * @param {*} links
     */
    error: function (data, links) {
        return {
            error: data,
            links
        };
    },

    /**
     *
     * @param {[Object]} data array of objects
     * @param {function()} itemLinksBuilder
     * @param {[Object]} colLinks array (according to collection+json)
     * @param {Object} template object (according to collection+json)
     * @param {*} queries
     */
    collection: function (data, itemLinksBuilder, colLinks, template, queries, role) {
        console.log(JSON.stringify(data));
        let self = this;
        let hyperData = _.map(data, (i) => self.item(i, itemLinksBuilder, role));

        return {
            data: hyperData,
            links: colLinks,
            template,
            queries
        };
    },

    /**
     * creates ETag header based on the existence of __v property
     * @param {Object} entry
     */
    etag: function (entry) {
        let etag;
        if (entry) {
            if (entry.data) {
                etag = entry.data.__v ? entry.data.__v.toString() : null;
            } else {
                etag = entry.__v ? entry.__v.toString() : null;
            }
        }
        if (etag) {
            return {
                ETag: etag
            };
        } else {
            return {};
        }
    },


    /**
     * A Link
     * @typedef {Object} Link
     * @property {string} rel      - a relationship type
     * @property {uri}    href     - the target URL of the relation
     * @property {string} [name]   - a unique identifier of the query
     * @property {string} [prompt] - a text to present to a user representing this query's intention
     */


    /**
     * Transforms an array of Link objects (e.g., Collection+JSON) to an object hash (e.g., HAL)
     * for example,
     *      [
     *          {rel:"self", href:"/m/3", name:"me"},
     *          {rel:"next", href="/m/4"}
     *      ]
     *
     * will become
     *      {
     *          self: {href:"/m/3", name:"me"},
     *          next: {href="/m/4"}
     *      }
     *
     * @param {Link[]} linkArray
     */
    transformLinkArrayToHash: function (linkArray) {
        let out = {};
        linkArray.forEach(e => {
            out[e.rel] = e.href;
        });
        return out;
    }
};
