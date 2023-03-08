const express = require('express')
const bodyParser = require('body-parser')
const FileSystem = require('fs')
var callbackApp = express()
callbackApp.use(bodyParser.json())
callbackApp.use(bodyParser.urlencoded({ extended: true }))
const callbackPort = process.env.PORT || 3010
const CALLBACK_ROOT = 'http://localhost:' + callbackPort
// STARTING callback
callbackApp.listen(callbackPort, function () {
    console.log('Listening on ' + callbackPort)
})

const request = require('request')
const serverUrl = 'http://localhost:3000'

// callback routing
callbackApp.route('/callback').post(function (req, res) {
    // process the response to our initial request
    console.log('Got callback response! ')

    // reply back
    res.status(204).send('No Content')

    FileSystem.writeFile(
        `${__dirname}\\dashboard.html`,
        req.body.dashboard,
        (err, data) => {
            if (err) {
                console.log(err)
            }
        }
    )
})

let arg = process.argv[2]

if (arg == '1') {
    getDashboard()
} else {
    getCashFlowDashBoard()
}

function getDashboard() {
    // Auto login and request
    console.log('Logging in...')
    request(
        {
            uri: `${serverUrl}/login`,
            method: 'POST',
            json: { username: 'Admin', password: 'admin' }
        },
        function (err, res, body) {
            if (!err) {
                //console.log(`Login Response status => ${res.statusCode}`)
                console.log('Logged in...')
                const token = body.token
                console.log('Making request to build dashboard...')

                request(
                    {
                        uri: serverUrl + '/Subscriptions/Dashboard',
                        method: 'POST',
                        auth: {
                            bearer: token
                        },
                        json: {
                            callback: CALLBACK_ROOT + '/callback'
                        }
                    },
                    function (err, res, body) {
                        if (!err) {
                            console.log(
                                'Dashboard request got directly status code ' +
                                    res.statusCode
                            )
                        } else {
                            console.log(err)
                        }
                    }
                )
            } else {
                console.log(err)
            }
        }
    )
}

function getCashFlowDashBoard() {
    // Auto login and request
    console.log('Logging in...')
    request(
        {
            uri: `${serverUrl}/login`,
            method: 'POST',
            json: { username: 'Admin', password: 'admin' }
        },
        function (err, res, body) {
            if (!err) {
                //console.log(`Login Response status => ${res.statusCode}`)
                console.log('Logged in...')
                const token = body.token
                console.log('Making request to build dashboard...')

                request(
                    {
                        uri: serverUrl + '/Subscriptions/DashboardCashflow',
                        method: 'POST',
                        auth: {
                            bearer: token
                        },
                        json: {
                            callback: CALLBACK_ROOT + '/callback'
                        },
                        qs: {
                            months: 20,
                            mode: 'future'
                        }
                    },
                    function (err, res, body) {
                        if (!err) {
                            console.log(
                                'Dashboard request got directly status code ' +
                                    res.statusCode
                            )
                        } else {
                            console.log(err)
                        }
                    }
                )
            } else {
                console.log(err)
            }
        }
    )
}
