const express = require('express')
const helmet = require('helmet')
const logger = require('morgan')
const bodyParser = require('body-parser')
const subscriptionsHandler =
    require('./service/subscriptionHandler').SubscriptionsHandler()
const dashboard = require('./dashboard/dashboard').Dashboard()
const dashboardCashflows =
    require('./dashboard/dashboard-cashflows').DashboardCashflows()

const app = express()
app.use(express.static('dashboard'))

app.use(bodyParser.json())

app.use(
    bodyParser.urlencoded({
        extended: true
    })
)

app.use(helmet())

app.use(logger('dev'))
const port = process.env.PORT || process.argv[2] || 3004
const SERVER = 'localhost:' + port
const SERVER_ROOT = 'http://' + SERVER
const API_PATH = '/api'
const API_ROOT = SERVER_ROOT + API_PATH

const configuration = {
    apiRoot: API_ROOT,
    apiPath: API_PATH,
    server: SERVER,
    serverRoot: SERVER_ROOT
}

// GET all existing subscriptions
app.route('/Subscriptions')
    .get(subscriptionsHandler.handleGetSubscriptions)
    .post(subscriptionsHandler.handlePostSubscription)
    .patch(subscriptionsHandler.handlePatchSubscriptions)

// Build dashboard
app.post('/Subscriptions/Dashboard', dashboard.handleDashBoard)

// Build dashboard
app.post(
    '/Subscriptions/DashboardCashflow',
    dashboardCashflows.handleCashFlowDashboard
)

// GET subscription by id
app.param('subscriptionID', function (req, res, next, subscriptionID) {
    req.subscriptionID = subscriptionID
    return next()
})

app.route('/Subscriptions/:subscriptionID')
    .get(subscriptionsHandler.handleGetSubscription)
    .put(subscriptionsHandler.handlePutSubscription)
    // .post(subscriptionsHandler.handlePostSubscription)
    .patch(subscriptionsHandler.handlePatchSubscription)

app.post(
    '/Subscriptions/:subscriptionID/deactivate',
    subscriptionsHandler.handleDeactivateSubscription
)

app.get(
    '/Subscriptions/:subscriptionID/user',
    subscriptionsHandler.handleGetSubscriptionUser
)

app.post(
    '/Subscriptions/:subscriptionID/renew',
    subscriptionsHandler.handleRenewSubscription
)

app.route('/Subscriptions/:subscriptionID/Plan')
    .get(subscriptionsHandler.handleGetSubscriptionPlan)
    .patch(subscriptionsHandler.handlePatchSubscriptionPlan)

app.param('deviceID', function (req, res, next, deviceID) {
    req.deviceID = deviceID
    return next()
})

app.route('/Subscriptions/:subscriptionID/devices').get(
    subscriptionsHandler.handleGetSubscriptionDevices
)
app.route('/Subscriptions/:subscriptionID/devices').post(
    subscriptionsHandler.handlePostDevice
)

app.route('/Subscriptions/:subscriptionID/devices/:deviceID')
    .patch(subscriptionsHandler.handlePatchDevice)
    .delete(subscriptionsHandler.handleDeleteDeviceFromSubscription)

/// ////////////////////////// STARTING ...
app.listen(port, function () {
    console.log('Subscriptions Service with:')
    console.log('')
    console.log('\tlayering')
    console.log('\tpagination(filtering in the query string, not with links)')
    console.log('\tpassport authentication')
    console.log('\tconcurrency handling with etag/if-match')
    console.log('\tOpen API specification: %s/openapi.json', API_ROOT)
    console.log('\tSwagger UI: %s/api-docs', API_ROOT)
    console.log('')
    console.log('Listening on %s', SERVER_ROOT)
})
