using Backend.Application.Features.Tasks.Commands.CreateUserTask;
using Backend.Application.Features.Tasks.Commands.DeleteUserTask;
using Backend.Application.Features.Tasks.Commands.UpdateUserTask;
using Backend.Application.Features.Tasks.DTOs;
using Backend.Application.Features.Tasks.Queries.GetAllUserTasks;
using Backend.Application.Features.Tasks.Queries.GetUserTask;
using Backend.Application.Features.Tasks.Queries.SearchUserTasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public sealed class TasksController(IMediator mediator) : ControllerBase
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<UserTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<UserTaskResponse>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int? status,
        [FromQuery] int? priority,
        [FromQuery] DateTime? dueFrom,
        [FromQuery] DateTime? dueTo,
        [FromQuery] string sortBy = "createdDate",
        [FromQuery] string sortDirection = "desc",
        CancellationToken cancellationToken = default)
    {
        var tasks = await mediator.Send(
            new SearchUserTasksQuery(
                searchTerm,
                status,
                priority,
                dueFrom,
                dueTo,
                sortBy,
                sortDirection),
            cancellationToken);

        return Ok(tasks);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<UserTaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserTaskResponse>>> GetAll(
        [FromQuery] int? status,
        [FromQuery] int? priority,
        CancellationToken cancellationToken)
    {
        var tasks = await mediator.Send(
            new GetAllUserTasksQuery(status, priority),
            cancellationToken);

        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserTaskResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var task = await mediator.Send(new GetUserTaskQuery(id), cancellationToken);
        return Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserTaskResponse>> Create(
        [FromBody] CreateUserTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = await mediator.Send(new CreateUserTaskCommand(request), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserTaskResponse>> Update(
        int id,
        [FromBody] UpdateUserTaskRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        var task = await mediator.Send(new UpdateUserTaskCommand(request), cancellationToken);

        return Ok(task);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUserTaskCommand(id), cancellationToken);
        return NoContent();
    }
}
