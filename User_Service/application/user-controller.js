// "use-strict";

// exceptions
const { UserRepository } = require('../persistence/user-repository')
const Exceptions = require('./../exceptions')
// const NotFoundError = Exceptions.NotFoundError
// const UnauthenticatedError = Exceptions.UnauthenticatedError;
// const ConflictError = Exceptions.ConflictError
// const ForbidenError = Exceptions.ForbidenError
// const ConcurrencyError = Exceptions.ConcurrencyError

const userRepo = require('../persistence/user-repository').UserRepository()
const authzController =
    require('../application/authz.controller').AuthzController()

function getUsers(start, n) {
    // For pagination purposes, set default values if not requested
    if (!start) {
        start = 1
    }
    if (!n) {
        n = 5
    }
    return userRepo.getUsers(start, n)
}

async function searchUsers(name, email, start, n) {
    // Default values
    if (!start) {
        start = 1
    }
    if (!n) {
        n = 6
    }
    const users = await userRepo.searchUsers(name, email, start, n);
    return {
        data: users.data.map((u) => buildUserDTO(u)),
        isLast: users.isLast
    }
}

async function getUserById(id) {
    return await userRepo.getByID(id)
}

async function saveUser(userId, name, email, password) {
    const saltHash = authzController.genPassword(password)
    const now = new Date()
    const user = {
        id: userId,
        name,
        email,
        password: saltHash.hash,
        salt: saltHash.salt,
        roles: ['user'],
        createdOn: now,
        updatedOn: now,
        _rev: '0'
    }
    const saved = await userRepo.saveUser(user)
    const dto = buildUserDTO(saved)
    const token = authzController.issueJWT(saved)
    return { user: dto, token }
}

async function applyChanges(changes, user, roles) {
    // Check if changes to roles
    if ('roles' in changes) {
        if (['admin'].includes(roles)) {
            return await updateUser(changes, user)
        }
        return null
    }
    if (
        'salt' in changes ||
        'createdOn' in changes ||
        'updatedOn' in changes ||
        '_rev' in changes
    ) {
        return null
        // TODO Not allow to change salt
    }
    if ('password' in changes) {
        const { salt, hash } = authzController.genPassword(changes.password)
        const obj = { salt, password: hash }
        return await updateUser(obj, user)
    }
    if ('email' in changes) {
        return await updateUser(changes, user)
    }
    // Fallback
    return null
}

function updateUser(changes, user) {
    Object.assign(user, changes)
    return userRepo.updateUser(user)
}

function deleteUser(user) {
    return userRepo.deleteUser(user)
}

function buildUserDTO(user) {
    return {
        id: user.id,
        name: user.name,
        email: user.email,
        roles: user.roles,
        __v: user._rev
    }
}

exports.UserController = function () {
    return {
        getUsers,
        searchUsers,
        getUserById,
        saveUser,
        deleteUser,
        buildUserDTO,
        applyChanges
    }
}
