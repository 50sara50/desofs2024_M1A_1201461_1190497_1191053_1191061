/* eslint-disable object-shorthand */
'use strict'

// Plans data store
const plans = {}

// Sample data
plans.Free = {
    id: 'Free',
    status: 'active',
    monthlyFee: '00.00',
    annualFee: '00.00',
    numberMinutes: '1000',
    devices: 1,
    musicCollections: 0,
    musicSuggestions: 'automatic',
    __v: '0',
    monthly_fee_change: ['00.00'],
    annual_fee_change: ['00.00'],
    promoted: false
}

plans.Silver = {
    id: 'Silver',
    status: 'active',
    monthlyFee: '4.99',
    annualFee: '49.99',
    numberMinutes: '5000',
    devices: 3,
    musicCollections: 10,
    musicSuggestions: 'automatic',
    __v: '0',
    monthly_fee_change: ['4.99'],
    annual_fee_change: ['49.99'],
    promoted: false
}

plans.Gold = {
    id: 'Gold',
    status: 'active',
    monthlyFee: '5.99',
    annualFee: '59.99',
    numberMinutes: 'unlimited',
    devices: 6,
    musicCollections: 25,
    musicSuggestions: 'personalized',
    __v: '0',
    monthly_fee_change: ['5.99'],
    annual_fee_change: ['59.99'],
    promoted: false
}

// Exports
exports.data = {
    plans
}
