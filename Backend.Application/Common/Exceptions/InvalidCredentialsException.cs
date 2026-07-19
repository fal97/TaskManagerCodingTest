namespace Backend.Application.Common.Exceptions;

public sealed class InvalidCredentialsException()
    : Exception("The username or password is incorrect.");
