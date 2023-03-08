const express = require('express')
const helmet = require('helmet')
const logger = require('morgan')
const bodyParser = require('body-parser')
const proxy = require('express-http-proxy')
require('dotenv').config()
const API_VERSION = require(process.cwd() + '/package.json').version
const port = process.env.PORT || process.argv[2] || 3000
const API_SERVER = `http://localhost:${port}`

const authz = require('./application/authz-controller').AuthzController()
const billboard = require('./billboard-handler').BillboardHandler()

const passport = require('passport')
// HTTP authentication
const passportHtpp = require('passport-http')
const BasicStrategy = passportHtpp.BasicStrategy
const DigestStrategy = passportHtpp.DigestStrategy

// JWT
const JWTStrategy = require('passport-jwt').Strategy
const ExtractJWT = require('passport-jwt').ExtractJwt
const opts = {}
opts.jwtFromRequest = ExtractJWT.fromAuthHeaderAsBearerToken()
opts.secretOrKey = process.env.JWT_SECRET

// OPEN API
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
                url: 'http://localhost:3000'
            }
        ]
    },
    apis: ['./open-api-definitions.js']
}

const specs = swaggerJSDoc(options)

const SUBSCRIPTION_SERVICE = process.env.SUBSCRIPTION_SERVICE
const USER_SERVICE = process.env.USER_SERVICE
const PLANS_SERVICE = process.env.PLANS_SERVICE

const app = express()
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(specs))

app.use(passport.initialize())

const AUTH_STRATEGY = ['jwt']

passport.use(
    new JWTStrategy(opts, function (jwtPayload, done) {
        authz
            .findUser(jwtPayload.obj.id)
            .then((user) => {
                if (user) {
                    return done(null, user)
                } else {
                    done(null, false)
                }
            })
            .catch((err) => {
                done(err, null)
            })
    })
)

// passport.use(
//     new BasicStrategy(function (username, password, done) {
//         authz
//             .login(username, password)
//             .then((user) => {
//                 console.log(user)
//                 done(null, !user ? false : user)
//             })
//             .catch((err) => {
//                 return done(null, false)
//             })
//     })
// )

app.use(helmet())

app.use(logger('dev'))

// eslint-disable-next-line n/handle-callback-err
function err1(err, res, next) {
    res.status(200).send('Something went wrong')
    next()
}

function myCustomHeaderInjection(proxyReqOpts, srcReq) {
    // you can update headers
    if (srcReq.user) {
        proxyReqOpts.headers.userName = srcReq.user.id
        proxyReqOpts.headers.roles = srcReq.user.roles
    }
    return proxyReqOpts
}

app.route('/login').post(
    proxy(USER_SERVICE, {
        proxyErrorHandler: err1
    })
)

// !USERS

// app.route('/users').get(
//     passport.authenticate(AUTH_STRATEGY, {
//         session: false
//     }),
//     proxy(USER_SERVICE, {
//         proxyReqOptDecorator: myCustomHeaderInjection,
//         proxyErrorHandler: err1
//     })
// )
function master() {
    const banner = `
A Board 4 U 

RESTful Hypermedia and GraphQL message service
API version: ${API_VERSION}

* REST
    * Billboard API: ${API_SERVER}/
    * OpenAPI 3.0 specification: ${API_SERVER}/openapi.json
    * Swagger UI: ${API_SERVER}/api-docs


Go to ${API_SERVER} to browse the API
`

    console.log(banner)
}
app.all(
    '/users',
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(USER_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

app.route('/version').get(billboard.handleGetVersion)

const specsJSON = require('./openapi.json')
// TODO: change the other to this one also and add the dashboard
app.use('/openapi.json', swaggerUi.serve, swaggerUi.setup(specsJSON))

const fs = require('fs')
app.get('/', (req, res) => {
    fs.readFile('./index.html', 'utf8', (error, data) => {
        if (error) {
            console.error(error)
            res.sendStatus(500)
        } else {
            res.set('Content-Type', 'text/html')
            res.send(data)
        }
    })
})

app.route('/api-info').get(billboard.handleGetTOC)

app.get('/rel/login', (req, res) => {
    fs.readFile('./assets/login.html', 'utf8', (error, data) => {
        if (error) {
            console.error(error)
            res.sendStatus(500)
        } else {
            res.set('Content-Type', 'text/html')
            res.send(data)
        }
    })
})

app.get('/rel/users', (req, res) => {
    fs.readFile('./assets/users.html', 'utf8', (error, data) => {
        if (error) {
            console.error(error)
            res.sendStatus(500)
        } else {
            res.set('Content-Type', 'text/html')
            res.send(data)
        }
    })
})

app.get('/rel/subscriptions', (req, res) => {
    fs.readFile('./assets/subscriptions.html', 'utf8', (error, data) => {
        if (error) {
            console.error(error)
            res.sendStatus(500)
        } else {
            res.set('Content-Type', 'text/html')
            res.send(data)
        }
    })
})

app.get('/rel/plans', (req, res) => {
    fs.readFile('./assets/plans.html', 'utf8', (error, data) => {
        if (error) {
            console.error(error)
            res.sendStatus(500)
        } else {
            res.set('Content-Type', 'text/html')
            res.send(data)
        }
    })
})

app.post(
    '/users/:userID',
    proxy(USER_SERVICE, {
        proxyErrorHandler: err1
    })
)

app.route('/users/:userID')
    .get(
        proxy(USER_SERVICE, {
            proxyErrorHandler: err1
        })
    )
    .put(
        (req, res, next) => {
            if (req.header('authorization')) {
                console.log('Auth')
                return passport.authenticate(AUTH_STRATEGY, {
                    session: false
                })(req, res, next)
            } else {
                return next()
            }
        },
        proxy(USER_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
    .delete(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(USER_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
    .patch(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(USER_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )

//! Dashboard
app.route('/Subscriptions/Dashboard').post(
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(SUBSCRIPTION_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

app.route('/Subscriptions/DashboardCashflow').post(
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(SUBSCRIPTION_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

// !Subscriptions
app.route('/subscriptions')
    .get(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(SUBSCRIPTION_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
    .post(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(SUBSCRIPTION_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
    .patch(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(SUBSCRIPTION_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
app.post(
    '/subscriptions/:subscriptionID',
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(SUBSCRIPTION_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

app.all(
    '/subscriptions/:subscriptionID',
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(SUBSCRIPTION_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

app.post(
    '/subscriptions/:subscriptionID/deactivate',
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(SUBSCRIPTION_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

app.post(
    '/subscriptions/:subscriptionID/renew',
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(SUBSCRIPTION_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

app.get('/subscriptions/:subscriptionID/user', 
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(SUBSCRIPTION_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

app.route('/subscriptions/:subscriptionID/plan')
    .get(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(SUBSCRIPTION_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
    .patch(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(SUBSCRIPTION_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )

// !DEVICES

app.route('/Subscriptions/:subscriptionID/devices').get(
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(SUBSCRIPTION_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)
app.route('/Subscriptions/:subscriptionID/devices').post(
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(SUBSCRIPTION_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

app.route('/Subscriptions/:subscriptionID/devices/:deviceID')
    .patch(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(SUBSCRIPTION_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
    .delete(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(SUBSCRIPTION_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )

// !Plans

app.route('/Plans').get(
    (req, res, next) => {
        if (req.header('authorization')) {
            console.log('Auth')
            return passport.authenticate(AUTH_STRATEGY, {
                session: false
            })(req, res, next)
        } else {
            return next()
        }
    },
    proxy(PLANS_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

app.route('/Plans/:planID')
    .get(
        (req, res, next) => {
            if (req.header('authorization')) {
                console.log('Auth')
                return passport.authenticate(AUTH_STRATEGY, {
                    session: false
                })(req, res, next)
            } else {
                return next()
            }
        },
        proxy(PLANS_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
    .put(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(PLANS_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
    .post(
        proxy(PLANS_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
    .delete(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(PLANS_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )
    .patch(
        passport.authenticate(AUTH_STRATEGY, {
            session: false
        }),
        proxy(PLANS_SERVICE, {
            proxyReqOptDecorator: myCustomHeaderInjection,
            proxyErrorHandler: err1
        })
    )

app.post(
    '/plans/:planID/deactivate',
    passport.authenticate(AUTH_STRATEGY, {
        session: false
    }),
    proxy(PLANS_SERVICE, {
        proxyReqOptDecorator: myCustomHeaderInjection,
        proxyErrorHandler: err1
    })
)

app.use(bodyParser.json())

app.use(
    bodyParser.urlencoded({
        extended: true
    })
)
app.listen(port, () => console.log('SERVER LISTENING ON PORT ' + port))
console.log(
    'You can cosult our information in: http://localhost:' + port + '/api-docs'
)
master()
