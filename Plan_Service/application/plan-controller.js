/* eslint-disable no-undef */
/* eslint-disable no-unused-vars */
'use strict'

const {
    ConflictError,
    NotFoundError,
    ConcurrencyError
} = require('../exceptions')
const _ = require('underscore')
const { SubscriptionService } = require('../service/subscription_service')

// Repositories
const plansRepo = require('../persistence/plan-repository').PlanRepository()

// ! Utils Functions

function mapToPlansDTO(plansData) {
    let plansDTO = []
    plansData.forEach((plan) => {
        plansDTO.push(new PlanDTO(plan))
    })
    return plansDTO
}

function mapToPlanDTO(plan) {
    let dto = new PlanDTO(plan)
    return dto
}
// !=====================================

async function getPlan(id, admin = false) {
    if (id) {
        if (admin) {
            return await plansRepo.getPlan(id)
        } else {
            const result = await plansRepo.getPlan(id);
            if (!result) {
                return Promise.resolve(undefined);
            }
          
            if (result.status !== 'active') {
                return undefined;
            }
            const plan = mapToPlanDTO(result);
            return plan;
        }
    } else {
        return Promise.reject(
            new RangeError('Empty parameters are not allowed.')
        )
    }
}

async function getPlans(admin = false, start, n) {
    if (admin) {
        const result = await plansRepo.getPlans(admin, start, n)
        return {
            plans: result.plans,
            isLast: result.isLast
        }
    } else {
        const result = await plansRepo.getPlans(admin, start, n)
        const plans = mapToPlansDTO(result.plans)
        return {
            plans,
            isLast: result.isLast
        }
    }
}

async function addPlan(
    newId,
    monthlyFee,
    aFee,
    nMinutes,
    nDevices,
    mCollections,
    mSuggestions
) {
    if (
        !_.every([
            newId,
            monthlyFee,
            aFee,
            nMinutes,
            nDevices,
            mCollections,
            mSuggestions
        ])
    ) {
        return Promise.reject(
            new RangeError('Empty parameters are not allowed.')
        )
    } else {
        const entry = await plansRepo.getPlan(newId)
        if (entry) {
            return Promise.reject(
                new ConflictError(
                    'The plan ' + newId + ' already exists.',
                    entry
                )
            )
        } else {
            return plansRepo.addPlan(
                newId,
                monthlyFee,
                aFee,
                nMinutes,
                nDevices,
                mCollections,
                mSuggestions
            )
        }
    }
}

async function partialUpdatePlan(req, rev) {
    if (!req.planID) {
        return Promise.reject(new RangeError('The id field cannot be empty'))
    } else {
        const plan = await plansRepo.getPlan(req.planID)
        if (!plan) {
            return Promise.reject(
                new NotFoundError('The plan ' + req.planID + ' does not exist.')
            )
        }
        if (rev !== plan.__v.toString()) {
            // Same revision
            return Promise.reject(new ConcurrencyError())
        }
        // The body only contains the necessary changes in patch method
        const changes = req.body
        return plansRepo.partialUpdatePlan(plan, changes)
    }
}

async function deletePlan(id, rev, username, roles) {
    if (!id) {
        return Promise.reject(
            new RangeError('Empty parameters are not allowed.')
        )
    }

    return getPlan(id, true).then(async (plan) => {
        if (!plan) {
            return Promise.reject(
                new NotFoundError('The plan ' + id + ' does not exist.')
            )
        }
        if (rev === plan.__v.toString()) {
            // checks if there are any subscriptions of the plan
            const subscriptions =
                await SubscriptionService.getSubscriptionsByPlan(
                    id,
                    username,
                    roles
                )
            if (subscriptions.length === 0) {
                plansRepo.deletePlan(id, rev).then((deleted) => {
                    if (!deleted) {
                        return Promise.reject(
                            new ConflictError(
                                'Something went wrong while deleting this item.'
                            )
                        )
                    }
                })
            } else {
                return Promise.reject(
                    new ConflictError(
                        'You cannot cease a plan with subscribers.'
                    )
                )
            }
        } else {
            return Promise.reject(
                new ConcurrencyError(
                    'Item has been updated by other user already. Please refresh and retry.',
                    plan
                )
            )
        }
    })
}

async function updatePlan(planId, body, revision) {
    if (!planId) {
        return Promise.reject(
            new RangeError('You need to provide an Id to update a plan')
        )
    }
    let plan = await plansRepo.getPlan(planId)

    if (revision === plan.__v.toString()) {
        try {
            const updated = await plansRepo.updatePlan(planId, body)
            return Promise.resolve(updated)
        } catch (error) {
            return Promise.reject(
                new RangeError('Wrong parameters while trying to update a plan')
            )
        }
    } else {
        return Promise.reject(
            new ConcurrencyError(
                'Item has been updated by other user already. Please refresh and retry.',
                plan
            )
        )
    }
}

// EXPORTS
exports.PlanController = function () {
    return {
        getPlan,
        getPlans,
        addPlan,
        partialUpdatePlan,
        deletePlan,
        updatePlan
    }
}

class PlanDTO {
    id
    status
    monthlyFee
    annualFee
    numberMinutes
    devices
    musicCollections
    musicSuggestions

    constructor(data) {
        this.id = data.id
        this.status = data.status
        this.monthlyFee = data.monthlyFee
        this.annualFee = data.annualFee
        this.numberMinutes = data.numberMinutes
        this.devices = data.devices
        this.musicCollections = data.musicCollections
        this.musicSuggestions = data.musicSuggestions
    }
}
