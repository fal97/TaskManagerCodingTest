using Backend.Application.Common.Behaviors;
using Backend.Application.Features.Tasks.Commands.CreateUserTask;
using Backend.Application.Features.Tasks.DTOs;
using FluentAssertions;
using FluentValidation;

namespace Backend.Tests.Application.Common.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_InvalidCommand_ThrowsValidationExceptionBeforeHandlerRuns()
    {
        var command = new CreateUserTaskCommand(new CreateUserTaskRequest
        {
            Title = string.Empty,
            Priority = 4
        });
        var behavior = new ValidationBehavior<CreateUserTaskCommand, UserTaskResponse>(
            [new CreateUserTaskValidator()]);
        var handlerWasCalled = false;

        var act = async () => await behavior.Handle(
            command,
            () =>
            {
                handlerWasCalled = true;
                return Task.FromResult(new UserTaskResponse { Title = "Created" });
            },
            CancellationToken.None);

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().Contain(error =>
            error.PropertyName == "Request.Title" && error.ErrorMessage == "Title is required.");
        exception.Which.Errors.Should().Contain(error =>
            error.PropertyName == "Request.Priority");
        handlerWasCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ValidCommand_InvokesHandler()
    {
        var command = new CreateUserTaskCommand(new CreateUserTaskRequest
        {
            Title = "Write tests",
            Priority = 2
        });
        var behavior = new ValidationBehavior<CreateUserTaskCommand, UserTaskResponse>(
            [new CreateUserTaskValidator()]);
        var expectedResponse = new UserTaskResponse { Title = command.Request.Title };

        var response = await behavior.Handle(
            command,
            () => Task.FromResult(expectedResponse),
            CancellationToken.None);

        response.Should().BeSameAs(expectedResponse);
    }
}
