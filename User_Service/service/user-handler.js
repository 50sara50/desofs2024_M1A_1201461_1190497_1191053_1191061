'use strict'

const { json } = require('body-parser')
const UserController =
    require('../application/user-Controller').UserController()
const authzController =
    require('../application/authz.controller').AuthzController()

const responseModule = require('./response')
const jwt = require('jsonwebtoken')
const Response = responseModule.Response
const etagHeader = responseModule.etagHeader

const receiving_port = process.env.PORT || process.argv[2] || 3000
const SERVER_ROOT = 'http://localhost:' + receiving_port

const sendJson = require('../util').sendJSON
const sendTxt = require('../util').send
const responseHelper = responseModule.ResponseHelper
const Hyper = require('./hyper-response').Hyper

//! Utils
function buildHref(start, n) {
    return SERVER_ROOT + '/users?start=' + start + '&n=' + n
}

function createLinksCollection(start, n, isLast) {
    const links = []
    links.push({
        rel: 'self',
        href: buildHref(start, n)
    })
    links.push({
        rel: 'start',
        href: buildHref(1, n)
    })
    if (start > 1) {
        links.push({
            rel: 'prev',
            href: buildHref(start - n, n)
        })
    }
    if (!isLast) {
        links.push({
            rel: 'next',
            href: buildHref(parseInt(start) + parseInt(n), n)
        })
    }
    //TODO: Should we have the parameter in a different place?
    links.push({
        rel: 'create',
        href: SERVER_ROOT + '/users/:userName'
    })
    return links
}

function createHypermediaUser(user, role) {
    const links = []
    links.push({
        rel: 'self',
        href: SERVER_ROOT + '/users/' + user.id
    })
    links.push({
        rel: 'login',
        href: SERVER_ROOT + '/login'
    })
    if (role === 'admin') {
        links.push({
            rel: 'collection',
            href: SERVER_ROOT + '/users/'
        })
    }
    links.push(
        {
            rel: 'edit',
            href: SERVER_ROOT + '/users/' + user.id
        }
    );
    links.push(
        {
            rel: 'delete',
            href: SERVER_ROOT + '/users/' + user.id
        }
    );

    return links
}

function createQueriesOfCollection() {
    const queries = []
    queries.push({
        rel: 'filter',
        href: SERVER_ROOT + '/users?name={name}',
        prompt: 'Filter by name',
        data: {
            name: 'name',
            value: '',
            prompt: 'Name'
        }
    })
    queries.push({
        rel: 'filter',
        href: SERVER_ROOT + '/users?email={email}',
        prompt: 'Filter by email',
        data: {
            name: 'email',
            value: '',
            prompt: 'Email'
        }
    })
    queries.push({
        rel: 'search',
        href: SERVER_ROOT + '/users?name={name}&email={email}',
        prompt: 'Search by name and email',
        data: [
            {
                name: 'name',
                value: '',
                prompt: 'Name'
            },
            {
                name: 'email',
                value: '',
                prompt: 'Email'
            }
        ]
    })
    //TODO: Check if there are search parameters
    return queries
}

function createUserTemplate() {
    return {
        data: [
            {
                name: 'name',
                value: '',
                prompt: 'Name'
            },
            {
                name: 'email',
                value: '',
                prompt: 'Email'
            },
            {
                name: 'password',
                value: '',
                prompt: 'Password'
            }
        ]
    }
}
function formatResponseCollection(response, out, req) {
    return response.format({
        'application/json': () => {
            return response.ok(
                out.data,
                responseHelper.linkHeader(
                    createLinksCollection(
                        req.query.start,
                        req.query.n,
                        out.isLast
                    )
                )
            )
        },
        'application/vnd.pagsousa.hyper+json': () => {
            return response.ok(
                Hyper.collection(
                    out.data,
                    createHypermediaUser,
                    createLinksCollection(
                        req.query.start,
                        req.query.n,
                        out.isLast
                    ),
                    createUserTemplate(),
                    createQueriesOfCollection(),
                    req.headers.roles
                )
            )
        }
    })
}

function formatResponse(response, entry, userRole) {
    return response.format({
        'application/json': () => {
            if (entry.created) {
                return response.created(
                    entry.data,
                    SERVER_ROOT + '/Plans/' + entry.data.id,
                    responseHelper.linkHeader(
                        createHypermediaUser(entry.data, userRole)
                    )
                )
            } else {
                return response.ok(
                    entry.data,
                    responseHelper.linkHeader(
                        createHypermediaUser(entry.data, userRole)
                    )
                )
            }
        },
        'application/vnd.pagsousa.hyper+json': () => {
            if (entry.created) {
                return response.created(
                    Hyper.item(entry.data.user, createHypermediaUser, userRole),
                    SERVER_ROOT + '/Plans/' + entry.data.id
                )
            } else {
                return response.ok(
                    Hyper.item(entry.data, createHypermediaUser, userRole)
                )
            }
        }
    })
}

async function getUsers(req, res) {
    const response = new Response(res)
    if (!req.query.start) {
        req.query.start = 1
    }
    if (!req.query.n) {
        req.query.n = 6
    }
    // TODO -  In the ocasion we use an external database there is the change we reject the promise
    return UserController.searchUsers(
        req.query.name,
        req.query.email,
        0 + req.query.start,
        0 + req.query.n
    )
        .then((out) => {
            return formatResponseCollection(response, out, req)
        })
        .catch((err) => {
            return response.error(err)
        })
}

function putUsers(req, res) {
    //return Promise.reject(NotAllowedException)
    let response = new Response(res)
    return response.notAllowed('Cannot override the entire collection')
}

function postUsers(req, res) {
    let response = new Response(res)
    return response.notAllowed('Use PUT /user/:id to register a new user')
}

function deleteUsers(req, res) {
    let response = new Response(res)
    return response.notAllowed('Cannot delete the entire collection')
}

function patchUsers(req, res) {
    let response = new Response(res)
    return response.notAllowed('Cannot issue a patch to the entire collection')
}

async function getUserItem(req, res) {
    const response = new Response(res)
    const user = await UserController.getUserById(req.userID)

    if (user === undefined) {
        return response.notFound(`User ${req.userID} not found!`)
        //res.status(404).send(`User ${req.userID} not found!`)
    } else {
        const dto = UserController.buildUserDTO(user)
        return formatResponse(
            response,
            {
                data: dto,
                created: false
            },
            dto.roles
        )
        //sendJson(res, 200, user)
    }
}

function updateUser(req, res, user) {
    if (!user) {
        // The user is authenticated but there is no matching id to the requested
        sendTxt(res, 404, 'Not Found!')
    } else {
        const username = req.headers.username
        console.log(`Autheticated user => ${username} <=`)
        if (username === user.id) {
            const revision = req.headers['if-match']
            console.log('If match', revision)
            if (!revision) {
                sendTxt(res, 400, 'Missing "if-match" header')
            } else {
                if (revision === user._rev) {
                    user.name = req.body.name
                    user.email = req.body.email
                    user.updatedOn = new Date()
                    user._rev = (parseInt(user._rev) + 1).toString()
                    formatResponse(
                        res,
                        {
                            data: user,
                            created: false
                        },
                        user.roles
                    )
                } else {
                    sendJson(res, 412, user)
                }
            }
        } else {
            sendTxt(res, 403, 'You can only update your profile!')
        }
    }
}

function postUserItem(req, res) {
    let response = new Response(res)
    return response.notAllowed('HTTP POST Method not supported')
}

async function putUserItem(req, res) {
    let response = new Response(res)
    const user = await UserController.getUserById(req.userID)

    if (!req.headers.username) {
        // User not authenticated, so its a create account (There is no body to update)
        if (user === undefined) {
            if (
                !(
                    req.userID &&
                    req.body.name &&
                    req.body.email &&
                    req.body.password
                )
            ) {
                return response.badRequest()
            }
            const token = await UserController.saveUser(
                req.userID,
                req.body.name,
                req.body.email,
                req.body.password
            )

            // Send response back (JWT)
            return formatResponse(
                response,
                {
                    data: token,
                    created: true
                },
                token.roles
            )
        } else {
            // Username already in use
            return response.conflict('Conflict! Username is already in use!')
        }
    } else {
        // User is already authenticated because there is a username in the headers
        return updateUser(req, res, user)
    }
}

async function deleteUserItem(req, res) {
    const revision = req.headers['if-match']

    console.log('If-match', revision)

    if (!revision) {
        return sendTxt(res, 412, 'Missing if-match header')
    } else {
        const user = await UserController.getUserById(req.userID)
        const username = req.headers.username // Get authenticated username in headers
        if (user === undefined) {
            // User not found in database
            return sendTxt(res, 404, 'User ' + req.userID + ' not found.')
        } else {
            if (username === user.id) {
                if (revision !== user._rev) {
                    return sendTxt(
                        res,
                        412,
                        'Precondition Failed, please refresh and try again'
                    )
                } else {
                    try {
                        await UserController.deleteUser(user)
                        return sendTxt(
                            res,
                            204,
                            'User ' + req.userID + ' deleted.'
                        )
                    } catch (error) {
                        return sendTxt(res, 500, 'Error encountered')
                    }
                }
            } else {
                return sendTxt(res, 403, 'You can only delete your profile.')
            }
        }
    }
}

async function patchuserItem(req, res) {
    const response = new Response(res)
    const revision = req.headers['if-match']

    console.log('If-match', revision)

    if (!revision) {
        return sendTxt(res, 400, 'Missing if-match header')
    } else {
        const user = await UserController.getUserById(req.userID)

        const username = req.headers.username // Get authenticated username in headers
        console.log('Authenticated user: ', username)

        if (user === undefined) {
            // User not found in database
            return sendTxt(res, 404, 'User ' + req.userID + ' not found.')
        } else {
            if (username === user.id) {
                if (revision !== user._rev) {
                    return response.preconditionFailed(
                        'Conflict while updating user. Please refresh'
                    )
                } else {
                    const changes = req.body
                    try {
                        const updatedUser = await UserController.applyChanges(
                            changes,
                            user,
                            req.headers.roles
                        )
                        if (updatedUser === null) {
                            return response.forbiden(
                                'You cant perform the required operation'
                            )
                        } else {
                            const userDTO =
                                UserController.buildUserDTO(updatedUser)
                            return formatResponse(
                                response,
                                {
                                    data: userDTO,
                                    created: false
                                },
                                userDTO.roles
                            )
                        }
                    } catch (err) {
                        return response.badRequest('Bad request')
                    }
                }
            } else {
                return response.forbiden('You can only update your profile.')
                //sendTxt(res, 403, 'You can only update your profile.')
            }
        }
    }
}

// !AUTHENTICATION

async function userLogin(req, res) {
    let response = new Response(res)
    const user = await UserController.getUserById(req.body.username)
    if (!user) {
        response.notFound('User Not Found')
    } else {
        const isValid = authzController.validPassword(
            req.body.password,
            user.password,
            user.salt
        )

        if (isValid) {
            const token = authzController.issueJWT(user)
            response.ok({ token })
        } else {
            response.unauthorized('Wrong Password')
        }
    }
}

exports.userHandler = function () {
    return {
        userLogin,
        getUsers,
        putUsers,
        postUsers,
        deleteUsers,
        patchUsers,
        getUserItem,
        postUserItem,
        putUserItem,
        deleteUserItem,
        patchuserItem
    }
}
