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

function ConcurrencyException (message, currentState) {
    this.name = 'ConcurrencyError';
    this.message = message || 'Item has been updated after your previous query';
    this.currentState = currentState;
    this.stack = (new Error()).stack;
}
ConcurrencyException.prototype = Object.create(Error.prototype);
ConcurrencyException.prototype.constructor = ConcurrencyException;

function BadRequestException (message, currentState) {
    this.name = 'Bad Request Exception';
    this.message = message || 'Bad Request';
    this.currentState = currentState;
    this.stack = (new Error()).stack;
}

BadRequestException.prototype = Object.create(Error.prototype);
BadRequestException.prototype.constructor = BadRequestException;
/*
 * state inconsistencies due to concurrency issues - will generate a 409
 */
function ConflictException (message, currentState) {
    this.name = 'Conflict Exception';
    this.message = message || 'There is a conflict between the server resource state and the cliente state';
    this.currentState = currentState;
    this.stack = (new Error()).stack;
}
ConflictException.prototype = Object.create(Error.prototype);
ConflictException.prototype.constructor = ConflictException;

/**
 * The currently authenticated user does not have permission (authorization) to execute an operation
 */
function ForbiddenException (message) {
    this.name = 'Forbidden Exception';
    this.message = message || 'You are not authorized to perform this operation';
    this.stack = (new Error()).stack;
}
ForbiddenException.prototype = Object.create(Error.prototype);
ForbiddenException.prototype.constructor = ForbiddenException;


// -----------------------MODULES EXPORTS-------------------------------
exports.BadRequestException = BadRequestException
exports.UnauthorizedException = UnauthorizedException
exports.NotFoundException = NotFoundException
exports.UnauthenticatedException = UnauthenticatedException
exports.ConcurrencyException = ConcurrencyException
exports.ConflictException = ConflictException
exports.ForbiddenException = ForbiddenException
