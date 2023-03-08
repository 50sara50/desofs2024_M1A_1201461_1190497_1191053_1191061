'use strict'

const Exceptions = require('../exceptions')
const ConcurrencyError = Exceptions.ConcurrencyError

const bd = require('./user-bd').data
const users = bd.users

//
// authentication
//
// function getAuthz(username) {
//     if (users_pwd[username]) {
//         var user = users[username]
//         user.name = users[username].name
//         const out = {id: user.id, roles:user.roles}
//         return Promise.resolve(out)
//     } else {
//         return Promise.resolve(false)
//     }
// }

exports.AuthzRepository = function () {
    return {}
}
