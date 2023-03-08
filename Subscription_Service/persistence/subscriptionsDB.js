// FIXME: This is a temporary solution to get the subscriptions.At least the id should be different.
var subscriptions = {}
// Importing data
subscriptions.Simao = {
    id: 'Simao',
    user: 'Simao',
    plan: 'Gold',
    createdOn: new Date(),
    type: 'monthly',
    renewDate: new Date(new Date().setMonth(new Date().getMonth() + 1)),
    status: 'active',
    subscriptionFee: '5.99',
    cancelationDate: null,
    __v: '0'
}

subscriptions.Davide = {
    id: 'Davide',
    user: 'Davide',
    plan: 'Silver',
    createdOn: new Date(2021, 5, 23),
    type: 'annual',
    renewDate: new Date(2022, 5, 21),
    status: 'active',
    subscriptionFee: '49.99',
    cancelationDate: new Date(2022, 8, 3),
    __v: '0'
}

subscriptions.Cecilia = {
    id: 'Cecilia',
    user: 'Cecilia',
    plan: 'Gold',
    createdOn: new Date(2022, 3, 19),
    type: 'annual',
    renewDate: new Date(2023, 3, 19),
    status: 'active',
    subscriptionFee: '59.99',
    cancelationDate: null,
    __v: '0'
}

subscriptions.Narciso = {
    id: 'Narciso',
    user: 'Narciso',
    plan: 'Gold',
    createdOn: new Date(2022, 4, 5),
    type: 'annual',
    renewDate: new Date(2023, 4, 5),
    status: 'active',
    subscriptionFee: '59.99',
    cancelationDate: null,
    __v: '0'
}

subscriptions.Arnaldo = {
    id: 'Arnaldo',
    user: 'Arnaldo',
    plan: 'Gold',
    createdOn: new Date(2022, 5, 15),
    type: 'annual',
    renewDate: new Date(2023, 5, 15),
    status: 'active',
    subscriptionFee: '59.99',
    cancelationDate: null,
    __v: '0'
}

subscriptions.Sandrine = {
    id: 'Sandrine',
    user: 'Sandrine',
    plan: 'Silver',
    createdOn: new Date(2022, 2, 3),
    type: 'annual',
    renewDate: new Date(2023, 2, 3),
    status: 'active',
    subscriptionFee: '49.99',
    cancelationDate: null,
    __v: '0'
}

subscriptions.Dora = {
    id: 'Dora',
    user: 'Dora',
    plan: 'Silver',
    createdOn: new Date(2022, 3, 7),
    type: 'annual',
    renewDate: new Date(2023, 3, 7),
    status: 'active',
    subscriptionFee: '49.99',
    cancelationDate: null,
    __v: '0'
}

subscriptions.Bernardino = {
    id: 'Bernardino',
    user: 'Bernardino',
    plan: 'Silver',
    createdOn: new Date(2022, 12, 30),
    type: 'monthly',
    renewDate: new Date(2023, 1, 30),
    status: 'active',
    subscriptionFee: '4.99',
    cancelationDate: null,
    __v: '0'
}

exports.data = {
    subscriptions
}
