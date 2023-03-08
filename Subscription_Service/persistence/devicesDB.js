// FIXME: This is a temporary solution to get the devices.At least the id should be different.
var devices = {};
const subscriptions = require('./subscriptionsDB').data.subscriptions;

devices.SM200 = {
    id: 'SM200',
    name: 'Android Phone',
    description: 'mobile',
    subscription: subscriptions.Simao.id,
    _v: 0
}

devices.SM300 = {
    id: 'SM300',
    name: 'Samsung Smart TV',
    description: 'tv',
    subscription: subscriptions.Simao.id,
    _v: 0

}

devices.SM400 = {
    id: 'SM400',
    name: 'Chrome PC (Cadmium)',
    description: 'tv',
    subscription: subscriptions.Davide.id,
    _v: 0
}

devices.E400 = {
    id: 'SM400',
    name: 'Iphone',
    description: 'mobile',
    subscription: subscriptions.Davide.id,
    _v: 0
}

devices.E300 = {
    id: 'E300',
    name: 'LG Smart TV',
    description: 'tv',
    subscription: subscriptions.Davide.id,
    _v: 0
}

exports.data = {
    devices
}
