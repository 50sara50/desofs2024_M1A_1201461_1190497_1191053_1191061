/* eslint-disable no-unused-vars */
'use strict';

// The object does not exist
function NotFoundException (message) {
    this.name = 'Not Found Exception';
    this.message = message || 'Object not found.';
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

// Checks for changes made in another server instances
function ConcurrencyException (message) {
    this.name = 'Concurrency Exception';
    this.message = message || 'Someone is making alterations to this object at the same time as you!';
    this.stack = new Error().stack;
}
ConcurrencyException.prototype = Object.create(Error.prototype);
ConcurrencyException.prototype.constructor = ConcurrencyException;

/*
 * state inconsistencies due to concurrency issues - will generate a 409
 */
function ConflictError (message, currentState) {
    this.name = 'ConflictError';
    this.message = message || 'There is a conflict between the server resource state and the cliente state';
    this.currentState = currentState;
    this.stack = (new Error()).stack;
}
ConflictError.prototype = Object.create(Error.prototype);
ConflictError.prototype.constructor = ConflictError;

function BadRequestError (message, currentState) {
    this.name = 'Bad Request Exception';
    this.message = message || 'Bad Request';
    this.currentState = currentState;
    this.stack = (new Error()).stack;
}

BadRequestError.prototype = Object.create(Error.prototype);
BadRequestError.prototype.constructor = BadRequestError;

function ForbiddenError (message) {
   this.name = 'Forbidden Exception';
   this.message = message || 'You are not authorized to perform this operation';
   this.stack = (new Error()).stack;
}
ForbiddenError.prototype = Object.create(Error.prototype);
ForbiddenError.prototype.constructor = ForbiddenError;

// -----------------------MODULES EXPORTS-------------------------------
exports.UnauthorizedError = UnauthorizedException;
exports.NotFoundError = NotFoundException;
exports.UnauthenticatedError = UnauthenticatedException;
exports.ConcurrencyError = ConcurrencyException;
exports.ConflictError = ConflictError;
exports.BadRequestError = BadRequestError;
exports.ForbiddenError = ForbiddenError;
