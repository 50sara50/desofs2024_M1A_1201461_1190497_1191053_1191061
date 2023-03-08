//const authzRepo = require('../persistence/authz-repository').AuthzRepository()
const crypto = require('crypto')
const jwt = require('jsonwebtoken')

function validPassword(password, hash, salt) {
    let hashVerify = crypto
        .pbkdf2Sync(password, salt, 10000, 64, 'sha512')
        .toString('hex')
    return hash === hashVerify
}

function genPassword(Rawpassword) {
    let salt = crypto.randomBytes(32).toString('hex')
    let genHash = crypto
        .pbkdf2Sync(Rawpassword, salt, 10000, 64, 'sha512')
        .toString('hex')

    return {
        salt,
        hash: genHash
    }
}

function issueJWT(user) {
    const obj = { id: user.id, roles: user.roles }
    const token = jwt.sign({ obj }, 'TOP_SECRET', { expiresIn: '1h' })
    return token;
}

exports.AuthzController = function () {
    return {
        genPassword,
        validPassword,
        issueJWT
    }
}
