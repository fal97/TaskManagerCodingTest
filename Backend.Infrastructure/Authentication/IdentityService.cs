using Backend.Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Authentication;

public sealed class IdentityService(UserManager<IdentityUser> userManager)
    : IIdentityService
{
    public async Task<bool> CheckPasswordAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByNameAsync(username);
        return user is not null && await userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IdentityRegistrationResult> RegisterAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = new IdentityUser { UserName = username };
        var result = await userManager.CreateAsync(user, password);
        var errors = result.Errors
            .GroupBy(error => error.Code)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Description).ToArray());

        return new IdentityRegistrationResult(result.Succeeded, errors);
    }
}
