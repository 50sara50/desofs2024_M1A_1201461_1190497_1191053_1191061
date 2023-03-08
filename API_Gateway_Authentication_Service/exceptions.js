/* eslint-disable no-unused-vars */
'use strict';

// The object does not exist
function NotFoundException (message, requestId) {
    this.name = 'Not Found Exception';
    this.message = message || 'Object not found.';
    this.requestId = requestId;
    this.stack = new Error().stack;
}
NotFoundException.prototype = Object.create(Error.prototype);
NotFoundException.prototype.constructor = NotFoundException;

// Authentication is required && user is not logged in
function UnauthenticatedException (message) {
    this.name = 'Unauthenticated Exception';
    this.message =
    message || 'You need to be authenticated to perform this action.';
    this.stack = new Error().stack;
}
UnauthenticatedException.prototype = Object.create(Error.prototype);
UnauthenticatedException.prototype.constructor = UnauthenticatedException;

// The user does not have permission to perform action
function UnauthorizedException (message) {
    this.name = 'Unauthorized Exception';
    this.message = message || "You don't have permission to perform this action.";
    this.stack = new Error().stack;
}
UnauthorizedException.prototype = Object.create(Error.prototype);
UnauthorizedException.prototype.constructor = UnauthorizedException;

// The user does not have permission to perform action
function NotAllowedException (message) {
    this.name = 'Not allowed Exception';
    this.message = message || "You don't are not allowed to perform this action.";
    this.stack = new Error().stack;
}
NotAllowedException.prototype = Object.create(Error.prototype);
NotAllowedException.prototype.constructor = NotAllowedException;

// Checks for changes made in another server instances
function ConcurrencyException (message) {
    this.name = 'Concurrency Exception';
    this.message = message || 'Someone is making alterations to this object at the same time as you!';
    this.stack = new Error().stack;
}
ConcurrencyException.prototype = Object.create(Error.prototype);
ConcurrencyException.prototype.constructor = ConcurrencyException;

// -----------------------MODULES EXPORTS-------------------------------
exports.UnauthorizedException = UnauthorizedException;
exports.NotFoundException = NotFoundException;
exports.UnauthenticatedException = UnauthenticatedException;
exports.ConcurrencyException = ConcurrencyException;
exports.NotAllowedException = NotAllowedException;
