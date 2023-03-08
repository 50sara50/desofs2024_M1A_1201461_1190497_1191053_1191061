const responseModule = require('./response')
const Response = responseModule.Response
const API_SERVER = 'http://localhost:3000'
const API_VERSION = require(process.cwd() + '/package.json').version;
/**
 * @swagger
 * /:
 *   get:
 *     tags:
 *       - Billboard
 *     summary: API's Table of contents
 *     description: API's table of contents, as a large road-side billboard announcing the API capabilities
 *     security: []
 *     responses:
 *       200:
 *         description: the table of contents in the form of links to navigate and explore the API
 *         content:
 *           text/html:
 *             schema:
 *               type: string
 *               format: html
 *           application/json:
 *             schema:
 *               type: object
 *               additionalProperties: false
 *               properties:
 *                 links:
 *                   $ref: '#/components/schemas/links'
 *         links:
 *           GetSubscriptions:
 *             operationId: getSubscriptions
 *             description: follow the `http://localhost:3000/rel/subscriptions` relation to GET all subscriptions
 *           GetUsers:
 *             operationId: getUsers
 *             description: follow the `http://localhost:3000/rel/users` relation to GET all users
 *          GetPlans:
 *             operationId: getPlans
 *             description: follow the `http://localhost:3000/rel/plans` relation to GET all plans
 *         GetDashboard:
 *             operationId: getDashboard
 *             description: follow the `http://localhost:3000/rel/dashboard` relation to GET the dashboard
 *         Login:
 *             operationId: login
 *             description: follow the `http://localhost:3000/rel/login` relation to login to the API
 *         GetVersion:
 *             operationId: getVersion
 *             description: follow the `http://localhost:3000/version` relation to GET the API version
 *          
 *       404:
 *         $ref: '#/components/responses/NotFound'
 * 
 */
function handleGetTOC(req, res) {
    // content
    const tocJson = {
        links: [{
            rel: 'help',
            href: API_SERVER + '/api-docs'
        },
        // TODO: add this back in when the openapi.json is ready
        // {
        //     rel: 'describedBy',
        //     href: API_SERVER + '/openapi.json'
        // },
        {
            rel: 'license',
            href: 'https://spdx.org/licenses/MIT.html'
        },
        {
            rel: 'self',
            href: API_SERVER
        },
        {
            rel: 'http://schema.org/softwareVersion',
            href: API_SERVER + '/version'
        },

        // TODO: check what should be here and what should be the rel value
        { 
            rel: 'http://localhost:3000/rel/login',
            href: API_SERVER + '/login'
        },
        {
            rel: 'http://localhost:3000/rel/users',
            href: API_SERVER + '/users'
        },
        {
            rel: 'http://localhost:3000/rel/subscriptions',
            href: API_SERVER + '/subscriptions'
        },
        {
            rel: 'http://localhost:3000/rel/plans',
            href: API_SERVER + '/plans'
        },
        {
            rel: 'http://localhost:3000/rel/dashboard',
            href: API_SERVER + '/dashboard'
        }
        ]
    };
    // 1 day cache - it could be even bigger...
    res.set('Cache-Control', 'public, max-age=432000');
    const response = new Response(res);
    response.format({
        'application/json': function () {
            return response.ok(tocJson);
        },
        'application/vnd.pagsousa.hyper+json': function () {
            return response.ok(tocJson);
        }
    }
    )
}

/**
 * @swagger
 * /version:
 *   get:
 *     tags:
 *       - Billboard
 *     summary: returns information on the API version
 *     description: returns information on the API version
 *     security: []
 *     responses:
 *       s200:
 *         description: information on the API version
 *         content:
 *           text/html:
 *             schema:
 *               type: string
 *               format: html
 *           application/json:
 *             schema:
 *               type: object
 *               additionalProperties: false
 *               properties:
 *                 version:
 *                   description: semantic versioning
 *                   type: string
 *                   maxLength: 50
 *                   pattern: '^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$'
 *                 title:
 *                   type: string
 *                   maxLength: 250
 *                   pattern: '^\w+$'
 *                 authores:
 *                   type: string
 *                   maxLength: 250
 *                   pattern: '^[\w ,]+$'
 *                 copyright:
 *                   type: string
 *                   maxLength: 250
 *                   pattern: '^[\w ,]+$'
 *       404:
 *         $ref: '#/components/responses/NotFound'
 */
function handleGetVersion(req, res) {
    // caching for 5 days
    res.set('Cache-Control', 'public, max-age=432000');
    console.log('GET /version');
    console.log(API_VERSION);
    const response = new Response(res);
    response.format({
        'text/html': function () {
            return response.html(
                `
                <body>
                <h1>Subscription Management API</h1>
                <p>v<span class='http://schema.org/softwareVersion'>${API_VERSION}</span></p>
                <p>Copyright (c) 2022-2023 <span class='authores'>Davide Clemente, Sara Borges, Simão Gomes </span></p>
                </body>
                `
            );
        },
        'application/json': function () {
            return response.ok({
                version: API_VERSION,
                title: 'Subscription Management API',
                authores: 'Davide Clemente, Sara Borges, Simão Gomes',
                copyright: '2022-2023'
            });
        }
    });
}

exports.BillboardHandler = function () {
    return {
        handleGetTOC,
        handleGetVersion
    }
}
