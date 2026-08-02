using Backend.Application.Abstractions;
using Backend.Application.Features.Authentication.Commands.AuthenticateUser;
using Backend.Application.Features.Authentication.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController(
    IMediator mediator,
    IAccessTokenGenerator accessTokenGenerator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AccessTokenResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new AuthenticateUserCommand(request), cancellationToken);

        return Ok(accessTokenGenerator.Generate(user.Username));
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<AuthenticatedUserResponse> Me()
    {
        return Ok(new AuthenticatedUserResponse(User.Identity!.Name!));
    }
}
