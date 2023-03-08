/* eslint-disable no-var */
/* eslint-disable no-unused-vars */
const express = require('express')
const helmet = require('helmet')
const logger = require('morgan')
const bodyParser = require('body-parser')
const { json } = require('body-parser')
const swaggerUi = require('swagger-ui-express')
const swaggerJSDoc = require('swagger-jsdoc')


const options = {
    definition: {
        openapi: '3.0.0',
        info: {
            title: 'Subscription Management API',
            version: '1.0.0',
            description: 'Simple express Subscription Management API'
        },
        servers: [
            {
                url: 'http://localhost:3003'
            }
        ]
    },
    apis: ['./service/plan-handler.js']
}

const specs = swaggerJSDoc(options);

const app = express()

app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(specs));
app.use(bodyParser.json())

app.use(
    bodyParser.urlencoded({
        extended: true
    })
)

app.use(helmet())

app.use(logger('dev'))

// version numbers
const VERSION = require('./package.json').version
const API_VERSION = VERSION
const port = process.env.PORT || process.argv[2] || 3003
const SERVER = 'localhost:' + port
const SERVER_ROOT = 'http://' + SERVER
const API_PATH = '/api'
const API_ROOT = SERVER_ROOT 

const configuration = {
    apiRoot: API_ROOT,
    apiPath: API_PATH,
    server: SERVER,
    serverRoot: SERVER_ROOT
}


const planResource = require('./service/plan-handler').PlanResource()

// GET all existing plans

app.route('/Plans').get(planResource.handleGetPlans)

app.param('planID', function (req, res, next, planID) {
    req.planID = planID
    return next()
})

app.route('/Plans/:planID')
    .get(planResource.handleGetPlan)
    .put(planResource.handlePutPlan)
    .post(planResource.handlePostPlan)
    .delete(planResource.handleDeletePlan)
    .patch(planResource.handlePatchPlan)


app.post('/Plans/:planID/deactivate', planResource.handleDeactivatePlan)


/// ////////////////////////// STARTING ...
app.listen(port, function () {
    console.log('Plan Service with:')
    console.log('')
    console.log('\tlayering')
    console.log('\tpagination(filtering in the query string, not with links)')
    console.log('\tpassport authentication')
    console.log('\tconcurrency handling with etag/if-match')
    console.log('\tPDF representation')
    console.log('\tphoto upload')
    console.log(
        '\tasynchronous creation of messages with call to 3rd party service'
    )
    console.log('\tOpen API specification: %s/openapi.json', API_ROOT)
    console.log('\tSwagger UI: %s/api-docs', API_ROOT)
    console.log('')
    console.log('Listening on %s', SERVER_ROOT)
})
