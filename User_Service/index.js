const express = require('express')
const helmet = require('helmet')
const logger = require('morgan')
const bodyParser = require('body-parser')
const userHandler = require('./service/user-handler').userHandler()

const app = express()

app.use(bodyParser.json())

app.use(
    bodyParser.urlencoded({
        extended: true
    })
)

app.use(helmet())

app.use(logger('dev'))

// !USERS
app.route('/users')
    .get(userHandler.getUsers)
    .put(userHandler.putUsers)
    .post(userHandler.postUsers)
    .delete(userHandler.deleteUsers)
    .patch(userHandler.patchUsers)

// !SPECIFIC USER
app.param('userID', function (req, res, next, userID) {
    req.userID = userID
    return next()
})
// !AUTHZ
app.post('/login', userHandler.userLogin)

app.route('/users/:userID')
    .get(userHandler.getUserItem)
    .post(userHandler.postUserItem)
    .put(userHandler.putUserItem)
    .delete(userHandler.deleteUserItem)
    .patch(userHandler.patchuserItem)

const receiving_port = process.env.PORT || process.argv[2] || 3001
app.listen(receiving_port, () =>
    console.log('SERVER LISTENING ON PORT ' + receiving_port)
)
