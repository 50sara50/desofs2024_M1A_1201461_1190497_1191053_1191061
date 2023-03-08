/* eslint-disable no-undef */
/* eslint-disable no-unused-vars */
/* eslint-disable camelcase */
'use strict'

const Exceptions = require('../exceptions')
const ConcurrencyException = Exceptions.ConcurrencyException
const NotFoundException = Exceptions.NotFoundException

// Importing data
const sample_data = require('./plans-bd')
const plans = sample_data.data.plans

function pushObjsToArray(obj, array, admin, start, n) {
    const keys = Object.keys(obj)
    start = parseInt(start)
    n = parseInt(n)
    const slicedKeys = keys.slice(start - 1, start + n - 1)
    if (admin) {
        slicedKeys.forEach((k) => {
            array.push(obj[k])
        })
    } else {
       slicedKeys.forEach((k) => {
            if (obj[k].status === 'active') array.push(obj[k])
        })
    }
}

function downgradePlans(obj) {
    Object.keys(obj).forEach((k) => {
        obj[k].promoted = false;
    })
}

// Insert new plan
function addPlan(
    newId,
    mFee,
    aFee,
    nMinutes,
    nDevices,
    mCollections,
    mSuggestions
) {
    const entry = {
        id: newId,
        status: 'active',
        monthlyFee: mFee,
        annualFee: aFee,
        numberMinutes: nMinutes,
        devices: nDevices,
        musicCollections: mCollections,
        musicSuggestions: mSuggestions,
        __v: '0',
        monthly_fee_change: [],
        annual_fee_change: []
    }
    entry.monthly_fee_change.push(mFee)
    entry.annual_fee_change.push(aFee)
    plans[newId] = entry
    return Promise.resolve(plans[newId])
}

// Save a plan
function savePlan(p) {
    p.__v = (parseInt(p.__v) + 1).toString()
    return Promise.resolve(p)
}

// Get all existing plans
function getPlans(admin, start, n) {
    const out = []
    pushObjsToArray(plans, out, admin, start, n);  
    const totalPages = admin ? Math.ceil(Object.keys(plans).length / (n)) : Math.ceil(Object.keys(plans).filter((k) => plans[k].status === 'active').length / (n));
    return Promise.resolve({
        isLast: start >= totalPages,
        plans: out
    })
}

// Get plan by id
function getPlan(id) {
    return Promise.resolve(plans[id])
}

// Delete a plan
function deletePlan(id, rev) {
    const entry = plans[id]

    if (!entry) {
        return Promise.reject(
            new NotFoundException("There is no plan with the id '" + id + "'.")
        )
    }

    if (entry.__v === rev) {
        delete plans[id]
        return Promise.resolve(true)
    } else {
        return Promise.reject(
            new ConcurrencyException(
                'This item has been updated by another user. Please refresh and retry.'
            )
        )
    }
}

// Update a plan
function updatePlan(id, body) {
    const entry = plans[id]

    entry.monthlyFee = body.monthlyFee
    entry.annualFee = body.annualFee
    entry.numberMinutes = body.numberMinutes
    entry.devices = body.devices
    entry.musicCollections = body.musicCollections
    entry.musicSuggestions = body.musicSuggestions
    entry.monthly_fee_change.push(body.monthlyFee)
    entry.annual_fee_change.push(body.annualFee)
    entry.__v = (parseInt(entry.__v) + 1).toString()

    return Promise.resolve(entry)
}

function partialUpdatePlan(plan, changes) {
    if (changes.promoted === 'true') {
        downgradePlans(plans)
    }
    plan.__v = (parseInt(plan.__v) + 1).toString()
    Object.assign(plan, changes)

    if (changes.monthlyFee) {
        plan.monthly_fee_change.push(changes.monthlyFee)
    }

    if (changes.annualFee) {
        plan.annual_fee_change.push(changes.annualFee)
    }

    plan.updatedOn = new Date()
    return Promise.resolve(plan)
}

// Exports
exports.PlanRepository = function () {
    return {
        getPlan,
        addPlan,
        getPlans,
        deletePlan,
        savePlan,
        updatePlan,
        partialUpdatePlan
    }
}
