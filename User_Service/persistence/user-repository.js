'use strict'

const Exceptions = require('../exceptions')
const ConcurrencyError = Exceptions.ConcurrencyError

const users = require('./user-bd').data.users

// Helper functions - Credits to Aboard4you by Paulo Gandra

function pushObjsToArray(obj, array, start, n) {
    const keys = Object.keys(obj)
    start = parseInt(start)
    n = parseInt(n)
    const slicedKeys = keys.slice(start - 1, start + n - 1)
    slicedKeys.forEach((k) => {
        array.push(obj[k])
    })
}

// pagination & transformation to array
// returns an array of objects
function pageResults(objs, start, n) {
    start = parseInt(start)
    n = parseInt(n)
    return Object.values(objs).slice(start - 1, start + n - 1)
}

/**
 * filter out a map based on a string field containing a substring
 * @param {*} obj
 * @param {*} field
 * @param {*} value
 */
function filterInObj(obj, field, value) {
    const out = {}
    Object.keys(obj).forEach((k) => {
        if (obj[k][field].includes(value)) out[k] = obj[k]
    })
    return out
}

function getUsers(start, n) {
    const out = []
    pushObjsToArray(users, out, start, n)
    const totalPages = Math.ceil(Object.keys(users).length / (n));
    return Promise.resolve({
        isLast: start >= totalPages,
        data: out
        
    })
}

function searchUsers(name, email, start, n) {
    let data = users
    if (name) data = filterInObj(users, 'name', name)
    if (email) data = filterInObj(users, 'email', email)
    const totalPages = Math.ceil(Object.keys(data).length / (n));
    return Promise.resolve({
        isLast: start >= totalPages,
        data: pageResults(data, start, n)
    })
}

function getByID(id) {
    let user = users[id]
    if (user) return Promise.resolve(user)
    else return Promise.resolve(undefined)
}

function saveUser(user) {
    users[user.id] = user
    return Promise.resolve(user)
}

async function deleteUser(user) {
    try {
        delete users[user.id]
        Promise.resolve(user)
    } catch (error) {
        Promise.reject(user)
    }
}

function updateUser(user) {
    try {
        user._rev = (parseInt(user._rev) + 1).toString()
        users[user.id] = user
        return Promise.resolve(user)
    } catch (error) {
        return Promise.reject(user)
    }
}

exports.UserRepository = function () {
    return {
        getUsers,
        searchUsers,
        getByID,
        deleteUser,
        saveUser,
        updateUser
    }
}
