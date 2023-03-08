'use strict';
const devices = require('./devicesDB').data.devices;
const Exceptions = require('../exceptions');
const NotFoundException = Exceptions.NotFoundException;

function addDeviceToSubscription (id, name, description, subscriptionID) {
    const device = {
        id,
        name,
        description,
        subscription: subscriptionID,
        _v: 0
    }
    devices[id] = device;
    return Promise.resolve(device);
}

function getDeviceByID (id) {
    return Promise.resolve(devices[id]);
}

// TODO: probably should check if both arguments need to be provided
function updateDevice (device, name = undefined, description = undefined) {
    device.name = (name === undefined ? device.name : name);
    device.description = (description === undefined ? device.description : description);
    device._v++;
    return Promise.resolve(device);
}
function getDevicesOfSubscription (subscriptionID) {
    const devicesOfSubscription = [];
    for (const i in devices) {
        if (devices[i].subscription === subscriptionID) {
            devicesOfSubscription.push(devices[i]);
        }
    }
    return Promise.resolve(devicesOfSubscription);
}

function deleteDevice (id) {
    // TODO: probably it is not necessary to check if the device exists here
    if (!id) {
        return Promise.reject(new RangeError("You need to provide a valid 'id' of a device."));
    }
    const device = devices[id];
    if (!device) {
        return Promise.reject(new NotFoundException(`Device with id ${id} not found`, id));
    }
    delete devices[id];
    return Promise.resolve(true);
}

exports.DevicesRepository = {
    addDeviceToSubscription,
    getDevicesOfSubscription,
    updateDevice,
    getDeviceByID,
    deleteDevice
}
