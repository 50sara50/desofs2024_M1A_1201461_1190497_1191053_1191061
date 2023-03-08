/**
 * @swagger
 * components:
 *  securitySchemes:
 *    bearerAuth:
 *      type: http
 *      scheme: bearer
 *      bearerFormat: JWT
 *  schemas:
 *    Plan:
 *      type: object
 *      required:
 *        - ID
 *        - monthlyFee
 *        - annualFee
 *        - numberMinutes
 *        - devices
 *        - musicCollections
 *        - musicSuggestions
 *      properties:
 *        ID:
 *          type: string
 *          description: The Plan's ID
 *        monthlyFee:
 *          type: string
 *          description: The value to pay every month
 *        annualFee:
 *          type: string
 *          description: The value to pay every year
 *        numberMinutes:
 *          type: string
 *          description: The number of minutes allowed to listen in this plan
 *        devices:
 *          type: string
 *          description: Number of devices allowed
 *        musicCollections:
 *          type: string
 *          description: The number of music collections allowed in this plan
 *        musicSuggestions:
 *          type: string
 *          description: The way the system gives suggestions
 *      example:
 *          ID: Premium
 *          monthlyFee: '5.99'
 *          annualFee: '69.99'
 *          numberMinutes: '60'
 *          devices: 5
 *          musicCollections: 10
 *          musicSuggestions: 'personalized'
 */

/**
 * @swagger
 * tags:
 *   name: Auth
 */

/**
 * @swagger
 * /login:
 *   post:
 *     tags:
 *       - Auth
 *     summary: Logs in the user and issues him a JWT token
 *     requestBody:
 *       description: User credentials
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               username:
 *                 type: string
 *               password:
 *                 type: string
 *     responses:
 *       200:
 *         description: Successfull login
 *       404:
 *         description: User not found
 *       401:
 *         description: Wrong password
 */

// !! PLANS

/**
 * @swagger
 * tags:
 *   name: Plans
 */

/**
 * @swagger
 * /plans:
 *   get:
 *     security:
 *       - bearerAuth: []
 *     tags:
 *       - Plans
 *     summary: returns all the plans - for logged users
 *     responses:
 *       200:
 *         description: The list of plans
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Plan'
 *       400:
 *         description: Bad Request
 *       404:
 *         description: Not found - there are no existing plans
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /plans:
 *   get:
 *     tags:
 *       - Plans
 *     summary: returns all the plans
 *     responses:
 *       200:
 *         description: The list of plans
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Plan'
 *       400:
 *         description: Bad Request
 *       404:
 *         description: Not found - there are no existing plans
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /plans/{planId}:
 *   get:
 *     tags: [Plans]
 *     summary: Returns a single plan by its ID
 *     parameters:
 *       - in: path
 *         name: planId
 *         schema:
 *           type: string
 *         required: true
 *         description: The plan ID
 *     responses:
 *       200:
 *         description: The details of the plan
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Plan'
 *       400:
 *         description: Bad Request
 *       404:
 *         description: The plan was not found
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /plans/{planId}:
 *   get:
 *     security:
 *       - bearerAuth: []
 *     tags: [Plans]
 *     summary: Returns a single plan by its ID
 *     parameters:
 *       - in: path
 *         name: planId
 *         schema:
 *           type: string
 *         required: true
 *         description: The plan ID
 *     responses:
 *       200:
 *         description: The details of the plan
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Plan'
 *       400:
 *         description: Bad Request
 *       404:
 *         description: The plan was not found
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /Plans/{planId}:
 *   delete:
 *     security:
 *       - bearerAuth: []
 *     tags: [Plans]
 *     summary: Ceases a plan
 *     parameters:
 *       - in: path
 *         name: planId
 *         schema:
 *           type: string
 *         required: true
 *         description: The plan ID
 *       - in: header
 *         name: if-match
 *         description: Revision control
 *     responses:
 *       200:
 *         description: The plan was successfuly deleted
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Plan'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only an administrator can perform this action
 *       404:
 *         description: Plan not found
 *       409:
 *         description: You cannot delete a plan with subscribers
 *       412:
 *         description: Pre condition failed - If-match header is not correct
 *       500:
 *         description: Internal server error
 */

// TODO: ver porque não está a retornar o objeto ao criar novo
/**
 * @swagger
 * /plans/{planId}:
 *   put:
 *     security:
 *       - bearerAuth: []
 *     tags: [Plans]
 *     summary: Defines/Updates a new Plan
 *     parameters:
 *       - in: path
 *         name: planId
 *         schema:
 *           type: string
 *         required: true
 *         description: The plan ID
 *       - in: header
 *         name: if-match
 *         description: Revision control
 *     requestBody:
 *       description: The plan
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               ID:
 *                 type: string
 *               monthlyFee:
 *                 type: string
 *               annualFee:
 *                 type: string
 *               numberMinutes:
 *                 type: string
 *               devices:
 *                 type: string
 *               musicCollections:
 *                 type: string
 *               musicSuggestions:
 *                 type: string
 *     responses:
 *       200:
 *         description: The plan was successfuly updated
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Plan'
 *       201:
 *         description: The plan was successfuly created
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Plan'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only an administrator can perform this action
 *       404:
 *         description: Plan not found
 *       412:
 *         description: Pre condition failed - If-match header is not correct
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /plans/{planId}:
 *   patch:
 *     security:
 *       - bearerAuth: []
 *     tags: [Plans]
 *     summary: Changes some plan's details
 *     parameters:
 *       - in: path
 *         name: planId
 *         schema:
 *           type: string
 *         required: true
 *         description: The plan ID
 *       - in: header
 *         name: if-match
 *         description: Revision control
 *     requestBody:
 *       description: plan
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               ID:
 *                 type: string
 *               monthlyFee:
 *                 type: string
 *               annualFee:
 *                 type: string
 *               numberMinutes:
 *                 type: string
 *               devices:
 *                 type: string
 *               musicCollections:
 *                 type: string
 *               musicSuggestions:
 *                 type: string
 *     responses:
 *       200:
 *         description: The plan was successfuly updated
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Plan'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only an administrator can perform this action
 *       404:
 *         description: Plan not found
 *       409:
 *         description: The request could not be processed due to a conflict
 *       412:
 *         description: Pre condition failed - If-match header is not correct
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /plans/{planId}/deactivate:
 *   post:
 *     security:
 *       - bearerAuth: []
 *     tags: [Plans]
 *     summary: Deactivates a Plan
 *     parameters:
 *       - in: path
 *         name: planId
 *         schema:
 *           type: string
 *         required: true
 *         description: The plan ID
 *       - in: header
 *         name: if-match
 *         description: Revision control
 *     responses:
 *       200:
 *         description: The plan was successfuly updated
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Plan'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only an administrator can perform this action
 *       404:
 *         description: Plan not found
 *       409:
 *         description: The request could not be processed due to a conflict
 *       412:
 *         description: Pre condition failed - If-match header is not correct
 *       500:
 *         description: Internal server error
 */

// !! SUBSCRIPTIONS
/**
 * @swagger
 * tags:
 *   name: Subscriptions
 */

// !!VER SE ACEITA DATAS COMO STRING OU NÃO!!
/**
 * @swagger
 * components:
 *  schemas:
 *    Subscription:
 *      type: object
 *      required:
 *        - ID
 *        - user
 *        - plan
 *        - createdOn
 *        - type
 *        - renewDate
 *        - status
 *        - subscriptionFee
 *        - cancelationDate
 *      properties:
 *        ID:
 *          type: string
 *          description: The subscription's ID
 *        user:
 *          type: string
 *          description: The subscriber
 *        plan:
 *          type: string
 *          description: The subscription plan
 *        createdOn:
 *          type: string
 *          description: The date when the subscription was made
 *        type:
 *          type: string
 *          enum: [annual, monthly]
 *          description: Type of payment (annual, monthly)
 *        renewDate:
 *          type: string
 *          description: The subscription's renovation date
 *        status:
 *          type: string
 *          description: The status of the subscription
 *        subscriptionFee:
 *          type: string
 *          enum: [active, deactivated]
 *          description: The total value to pay per year
 *        cancelationDate:
 *          type: date of cancelation
 *      example:
 *          ID: Sara
 *          user: Sara
 *          plan: Silver
 *          createdOn: 23/5/2021
 *          type: annual
 *          renewDate: 26/5/2022
 *          status: active
 *          subscriptionFee: 59.99
 *          cancelationDate: 30/8/2022
 */

/**
 * @swagger
 * tags:
 *   name: Dashboard
 */

/**
 * @swagger
 * /subscriptions/dashboard:
 *   get: 
 *     security:
 *       - bearerAuth: []
 *     tags: [Dashboard]
 *     summary: Shows the dashboard
 *     responses:
 *       200:
 *         description: Dashboard loaded!
 *       400:
 *         description: Bad Request
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /subscription/dashboardcashflows/{numberMonths}:
 *   get:
 *     security:
 *       - bearerAuth: []
 *     tags: [Dashboard]
 *     summary: Shows the cahsflows dashboard
 *     parameters:
 *       - in: path
 *         name: numberMonths
 *         schema:
 *           type: string
 *         required: true
 *         description: The amount of months to consider
 *     responses:
 *       200:
 *         description: Cashflows Dashboard loaded!
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /subscriptions:
 *   get:
 *     security:
 *       - bearerAuth: []
 *     tags: [Subscriptions]
 *     summary: returns all existing subscriptions
 *     responses:
 *       200:
 *         description: The list of the subscription
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Subscription'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only an administrator can perform this action
 *       404:
 *         description: Not found - there are no existing subscriptions
 *       500:
 *         description: Internal server error
 *
 */

/**
 * @swagger
 * /subscriptions/{subscriptionId}:
 *   get:
 *     security:
 *       - bearerAuth: []
 *     tags: [Subscriptions]
 *     summary: Returns a single subscription by its ID
 *     parameters:
 *       - in: path
 *         name: subscriptionId
 *         schema:
 *           type: string
 *         required: true
 *         description: The subscription ID
 *     responses:
 *       200:
 *         description: The details of the subscription
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Subscription'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only an administrator can perform this action
 *       404:
 *         description: Not found - subscription not found
 *       500:
 *         description: Internal server error
 *
 */

/**
 * @swagger
 * /subscriptions/{subscriptionId}:
 *   put:
 *     security:
 *       - bearerAuth: []
 *     tags: [Subscriptions]
 *     summary: Subscribes to a plan
 *     parameters:
 *       - in: path
 *         name: subscriptionId
 *         schema:
 *           type: string
 *         required: true
 *         description: The subscription ID
 *     requestBody:
 *       description: The subscription
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               planID:
 *                 type: string
 *               type:
 *                 type: string
 *                 enum: [annual, monthly]
 *     responses:
 *       200:
 *         description: The details of the subscription
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Subscription'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only the subscription owner can perform this action
 *       412:
 *         description: Pre condition failed - If-match header is not correct
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /subscriptions/{subscriptionId}/deactivate:
 *   post:
 *     security:
 *       - bearerAuth: []
 *     tags: [Subscriptions]
 *     summary: Deactivates a subscription
 *     parameters:
 *       - in: path
 *         name: subscriptionId
 *         schema:
 *           type: string
 *         required: true
 *         description: The subscription ID
 *       - in: header
 *         name: if-match
 *         description: Revision control
 *         required: true
 *     responses:
 *       200:
 *         description: The subscription was successfuly deactivated
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#components/schemas/Subscription'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only the subscription owner can perform this action
 *       404:
 *         description: Subscription not found
 *       412:
 *         description: Pre condition failed - If-match header is not correct
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /subscriptions/{subscriptionId}/renew:
 *   post:
 *     security:
 *       - bearerAuth: []
 *     tags: [Subscriptions]
 *     summary: Renews a subscription for one more year
 *     parameters:
 *       - in: path
 *         name: subscriptionId
 *         schema:
 *           type: string
 *         required: true
 *         description: The subscription ID
 *       - in: header
 *         name: if-match
 *         description: Revision control
 *         required: true
 *     responses:
 *       200:
 *         description: The subscription was successfuly renewed
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#components/schemas/Subscription'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only the subscription owner can perform this action
 *       404:
 *         description: Subscription not found
 *       412:
 *         description: Pre condition failed - If-match header is not correct
 *       500:
 *         description: Internal server error
*/

// !!DEVICES
/**
 * @swagger
 * tags:
 *   name: Devices
 */

/**
 * @swagger
 * components:
 *  schemas:
 *    Device:
 *      type: object
 *      required:
 *        - ID
 *        - name
 *        - description
 *        - subscription
 *      properties:
 *        ID:
 *          type: string
 *          description: The device's ID
 *        name:
 *          type: string
 *          description: The device's name
 *        description:
 *          type: string
 *          description: The device's details
 *        subscription:
 *          type: string
 *          description: The subscription connected to the device
 *      example:
 *          ID: SM200
 *          name: Iphone
 *          description: Sara's mobile phone
 *          subscription: Sara
 */

/**
 * @swagger
 * /subscriptions/{subscriptionId}/devices:
 *   get:
 *     security:
 *       - bearerAuth: []
 *     tags: [Devices]
 *     summary: returns all devices related to the subscription
 *     parameters:
 *       - in: path
 *         name: subscriptionId
 *         schema:
 *           type: string
 *         required: true
 *         description: The subscription's ID
 *     responses:
 *       200:
 *         description: The list of devices
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Device'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only the subscription owner can perform this action
 *       404:
 *         description: Not found - there are no existing devices
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /subscriptions/{subscriptionId}/devices:
 *   post:
 *     security:
 *       - bearerAuth: []
 *     tags: [Devices]
 *     summary: Adds a device to subscription
 *     parameters:
 *       - in: path
 *         name: subscriptionId
 *         schema:
 *           type: string
 *         required: true
 *         description: The subscription ID
 *     requestBody:
 *       description: The device
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               name:
 *                 type: string
 *               description:
 *                 type: string
 *     responses:
 *       201:
 *         description: Device added to subscription
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Device'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only the subscription owner can perform this action
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /subscriptions/{subscriptionId}/devices/{deviceId}:
 *   patch:
 *     security:
 *       - bearerAuth: []
 *     tags: [Devices]
 *     summary: Changes some device's details
 *     parameters:
 *       - in: path
 *         name: subscriptionId
 *         schema:
 *           type: string
 *         required: true
 *         description: The subscription ID
 *       - in: path
 *         name: deviceId
 *         schema:
 *           type: string
 *         required: true
 *         description: The device ID
 *       - in: header
 *         name: if-match
 *         description: Revision control
 *     requestBody:
 *       description: device
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               name:
 *                 type: string
 *               description:
 *                 type: string
 *     responses:
 *       201:
 *         description: Device added to subscription
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Device'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only the subscription owner can perform this action
 *       404:
 *         description: Device not found
 *       412:
 *         description: Pre condition failed - If-match header is not correct
 *       500:
 *         description: Internal server error
 */

/**
 * @swagger
 * /subscriptions/{subscriptionId}/devices/{deviceId}:
 *   delete:
 *     security:
 *       - bearerAuth: []
 *     tags: [Devices]
 *     summary: Deletes a device
 *     parameters:
 *       - in: path
 *         name: subscriptionId
 *         schema:
 *           type: string
 *         required: true
 *         description: The subscription ID
 *       - in: path
 *         name: deviceId
 *         schema:
 *           type: string
 *         required: true
 *         description: The device ID
 *       - in: header
 *         name: if-match
 *         description: Revision control
 *     responses:
 *       200:
 *         description: The device was successfuly deleted
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Device'
 *       400:
 *         description: Bad Request
 *       401:
 *         description: You need to be authenticated to peform this action
 *       403:
 *         description: Only the subscription owner can perform this action
 *       404:
 *         description: Device not found
 *       412:
 *         description: Pre condition failed - If-match header is not correct
 *       500:
 *         description: Internal server error
 */
