///////////////////////////////////////////////////////
//
// Paulo Gandra de Sousa
// (c) 2016 - 2019
//
///////////////////////////////////////////////////////

/*jslint node: true */
/*jshint esversion:6 */

'use strict'

//
// a documentation file to hold the common definitions of models used in the API
//

/**
 * @swagger
 * components:
 *   securitySchemes:
 *     basicAuth:
 *       type: http
 *       scheme: basic
 *     digestAuth:
 *       type: http
 *       scheme: digest
 *
 */

/**
 * @swagger
 * components:
 *   schemas:
 *     plans:
 *       description: a plan
 *       properties:
 *         ID:
 *           type: string
 *         monthlyFee:
 *           type: string
 *         annualFee:
 *           type: string
 *         numberMinutes:
 *           type: string
 *         devices:
 *           type: string
 *         musicCollections:
 *           type: string
 *         musicSuggestions:
 *           type: string
 */

/**
 * @swagger
 * components:
 *   schemas:
 *     plans:
 *       description: A key-value object map of plans. Each key will correspond to the "id" property of the corresponding value object
 *       properties:
 *         id:
 *           $ref: '#/components/schemas/plan'
 */
