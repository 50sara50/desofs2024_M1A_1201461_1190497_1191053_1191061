/* eslint-disable camelcase */
/* eslint-disable no-undef */
/* eslint-disable no-unused-vars */
'use strict'
// Application Layer API

// EXCEPTIONS
const Exceptions = require('../exceptions')
const NotFoundError = Exceptions.NotFoundError
// const UnauthorizedException = Exceptions.UnauthenticatedException
// const ConcurrencyException = Exceptions.ConcurrencyException

const PlanController =
    require('../application/plan-controller').PlanController()

const responseModule = require('./response')
const Response = responseModule.Response
const etagHeader = responseModule.etagHeader
const _ = require('underscore')
const receiving_port = 3000
const SERVER_ROOT = 'http://localhost:' + receiving_port
const API_PATH = '/api'
const Hyper = require('./hyper-response').Hyper
const API_ROOT = SERVER_ROOT + API_PATH
const responseHelper = responseModule.ResponseHelper
// !Utils functions
function checkSomeInArray(array, allowed) {
    // Explicit casting to avoid errors
    const arr = Array.from(array.split(','))
    const allow = Array.from(allowed)
    for (let i = 0; i < arr.length; i++) {
        if (_.contains(allow, arr[i])) return true
    }
    return false
}

function buildHref(start, n) {
    let href = `${SERVER_ROOT}/Plans?start=${start}&n=${n}`
    return href;
}

function createLinksOfCollection(start, n, isLast) {
    const links = [];
    links.push(
        {
            rel: 'self',
            href: buildHref(start, n)
        }
    );
    links.push(
        {
            rel: 'start',
            href: buildHref(1, n)
        }
    );
    if (start > 1) {
        links.push(
            {
                rel: 'prev',
                href: buildHref(start - n, n)
            }
        );
    }
    if (!isLast) {
        links.push(
            {
                rel: 'next',
                href: buildHref(start + n, n)
            }
        );
    }
    return links;
}

function createLinkHypermediaOfAPlan(plan, userRole) {
    const links = [];
    links.push(
        {
            rel: 'self',
            href: `${SERVER_ROOT}/Plans/${plan.id}`
        }
    );
    if ((userRole === 'admin' || userRole === 'marketing-director') && plan.status === 'active') {
        links.push(
            {
                rel: 'edit',
                href: `${SERVER_ROOT}/Plans/${plan.id}`
            }
        );
        links.push(
            {
                rel: 'deactivate',
                href: `${SERVER_ROOT}/Plans/${plan.id}`
            }
        );
        links.push(
            {
                rel: 'delete',
                href: `${SERVER_ROOT}/Plans/${plan.id}`
            }
        );
    }
    if (userRole === 'marketing-director' || userRole === 'admin') {
        links.push(
            {
                rel: 'collection',
                href: `${SERVER_ROOT}/Plans`
            }
        );
    }
    
    return links;
}

function createQueriesOfCollection() {
    const queries = [];
    return queries;
}

function createPlanTemplate() {
    const template = {
        data: [
            {
                name: 'montlyFee',
                value: '',
                prompt: 'Montly Fee'
            },
            {
                name: 'annualFee',
                value: '',
                prompt: 'Annual Fee'
            },
            {
                name: 'numberMinutes',
                value: '',
                prompt: 'Number Minutes'
            },
            {
                name: 'devices',
                value: '',
                prompt: ' Number of Devices'
            },
            {
                name: 'musicCollections',
                value: '',
                prompt: 'Music Collections'
            },
            {
                name: 'musicSuggestions',
                value: '',
                prompt: 'Music Suggestions'
            }
        ]
        }
    return template;
    }
function formatResponseCollection(response, out, req) {
    return response.format({
        'application/json': () => {
            return response.ok(out.plans, responseHelper.linkHeader(createLinksOfCollection(+req.query.start, +req.query.n, out.isLast)));
        },
        'application/vnd.pagsousa.hyper+json': () => {
            return response.ok(Hyper.collection(out.plans, createLinkHypermediaOfAPlan, createLinksOfCollection(+req.query.start, +req.query.n, out.isLast), createPlanTemplate(), createQueriesOfCollection(), req.headers.roles));
        }
    })
}

function formatResponse(response, entry, userRole) {
    return response.format({
        'application/json': () => {
            if (entry.created) {
                    return response.created(entry.data, SERVER_ROOT + '/Plans/' + entry.data.id, responseHelper.linkHeader(createLinkHypermediaOfAPlan(entry.data, userRole)));
            } else {
                return response.ok(entry.data, responseHelper.linkHeader(createLinkHypermediaOfAPlan(entry.data, userRole)));
            }
        },
        'application/vnd.pagsousa.hyper+json': () => {
            if (entry.created) {
                    return response.created(Hyper.item(entry.data, createLinkHypermediaOfAPlan, null, userRole), SERVER_ROOT + '/Plans/' + entry.data.id);
            } else {
                return response.ok(Hyper.item(entry.data, createLinkHypermediaOfAPlan));
            }
        }
    })
}


//!Plans functions
async function handleGetPlans(req, res) {
    const response = new Response(res)
    if (!req.query.start) {
        req.query.start = 1
    }
    if (!req.query.n) {
        req.query.n = 5
    }
    let out = [];
    try {
        if (req.headers.username) {
            // Logged in user
            const roles = req.headers.roles
            const allowed = ['admin', 'marketing-director']
            if (checkSomeInArray(roles, allowed)) {
                out = await PlanController.getPlans(true, req.query.start, req.query.n)
            } else {
                out = await PlanController.getPlans(false, req.query.start, req.query.n)
            }
        } else {
            out = await PlanController.getPlans(false, req.query.start, req.query.n)
        }
        return formatResponseCollection(response, out, req);
    } catch (err) {
        return response.error(err)
    }
}

async function handleGetPlan(req, res) {
    const response = new Response(res)
    const id = req.planID

    try {
        if (req.headers.username) {
            // Logged in user
            const roles = req.headers.roles
            const allowed = ['admin', 'marketing-director']
            if (checkSomeInArray(roles, allowed)) {
                const out = await PlanController.getPlan(id, true)
                return out !== undefined
                    ? formatResponse(response, {
                            data: out,
                            created: false
                    }, req.headers.roles)
                    : response.notFound(
                          `There is no plan with id ${req.planID}!`
                      )
            } else {
                const out = await PlanController.getPlan(id)
                return out !== undefined
                    ? formatResponse(response, {
                            data: out,
                            created: false
                    }, req.headers.roles)
                    : response.notFound(
                          `There is no plan with id ${req.planID}!`
                      )
            }
        } else {
            const out = await PlanController.getPlan(id)
            return out !== undefined
                ? formatResponse(response, {
                        data: out,
                        created: false
                }, req.headers.roles)
                : response.notFound(`There is no plan with id ${req.planID}!`)
        }
    } catch (err) {
        return response.serverError(
            'Something went wrong while trying to process your request... Try again latter'
        )
    }
}

// POST: not allowed
function handlePostPlan(req, res) {
    const response = new Response(res)
    return response.notAllowed('Use PUT /Plans/:id to define a new plan.')
}

// PUT: /Plans/:body
async function handlePutPlan(req, res) {
    const response = new Response(res)
    const roles = req.headers.roles
    const allowed = ['admin', 'marketing-director']
    if (checkSomeInArray(roles, allowed)) {
        const plan = await PlanController.getPlan(req.planID)
        if (!plan) {
            doAddPlan(req, res, req.planID)
        } else {
            doUpdatePlan(req, res)
        }
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

// PUT method to define a new Plan
async function doAddPlan(req, res, id) {
    const response = new Response(res)

    try {
        const entry = await PlanController.addPlan(
            id,
            req.body.monthlyFee,
            req.body.annualFee,
            req.body.numberMinutes,
            req.body.devices,
            req.body.musicCollections,
            req.body.musicSuggestions
        )
        return formatResponse(response, {
            data: entry,
            created: true
        }, req.headers.roles)
    } catch (err) {
        return response.error(err, (s) => s, etagHeader)
    }
}

// PUT method to update a plan
function doUpdatePlan(req, res, entry) {
    const rev = req.headers['if-match']

    const response = new Response(res)
    if (!rev) {
        return response.badRequest("missing 'if-match' header.")
    }
    return PlanController.updatePlan(req.planID, req.body, rev)
        .then(function (entry) {
            return formatResponse(response, {
                data: entry,
                created: false
            }, req.headers.roles)
        })
        .catch(function (err) {
            return response.error(err, (s) => s, etagHeader)
        })
}

// PATCH: /Plans/:id
function handlePatchPlan(req, res) {
    const rev = req.headers['if-match']
    const response = new Response(res)
    const roles = req.headers.roles
    const allowed = ['admin', 'marketing-director']
    if (checkSomeInArray(roles, allowed)) {
        if (!rev) {
            return response.badRequest("Missing 'if-match' header.")
        } else {
            PlanController.partialUpdatePlan(req, rev)
                .then((plan) => {
                    return formatResponse(response, {
                        data: plan,
                        created: false
                    }, req.headers.roles);
                })
                .catch((_err) => {
                    if (_err instanceof NotFoundError) {
                        return response.notFound(
                            `There is no plan with id ${req.planID}!`
                        )
                    }
                    if (_err instanceof Exceptions.ConcurrencyError) {
                        return response.conflict(
                            'Plan was updated by other user. Please refresh to try again'
                        )
                    }
                    return response.serverError(_err)
                })
        }
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

// DELETE: /Plans/:id
function handleDeletePlan(req, res) {
    const rev = req.headers['if-match']
    const response = new Response(res)
    const roles = req.headers.roles
    const allowed = ['admin', 'marketing-director']
    if (checkSomeInArray(roles, allowed)) {
        if (!rev) {
            return response.badRequest("Missing 'if-match' header.")
        } else {
            return PlanController.deletePlan(
                req.planID,
                rev,
                req.headers.username,
                req.headers.roles
            )
                .then(function (deleted) {
                    return response.ok('Plan ' + req.planID + ' deleted.')
                })
                .catch(function (err) {
                    return response.error(err, (s) => s, etagHeader)
                })
        }
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

function handleDeactivatePlan(req, res) {
    const response = new Response(res)
    // Get loged user roles and check if he has permissions
    // const userRoles = req.headers.roles
    // Temporary array
    const roles = req.headers.roles
    const allowed = ['admin', 'marketing-director']
    if (checkSomeInArray(roles, allowed)) {
        req.body = { status: 'deactivated' }
        return handlePatchPlan(req, res)
    } else {
        return response.forbiden(
            'You do not have permissions to perform this action'
        )
    }
}

// EXPORTS
exports.PlanResource = function () {
    return {
        handleGetPlan,
        handleGetPlans,
        handlePostPlan,
        handlePutPlan,
        handleDeletePlan,
        handlePatchPlan,
        handleDeactivatePlan
    }
}
