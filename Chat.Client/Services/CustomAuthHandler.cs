using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Chat.Client.Services;

public class CustomAuthHandler(StorageService storageService) : AuthenticationStateProvider
{
    private readonly StorageService _storageService = storageService;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var (userId, username, role) = await ParseJwtToken();

        var claimsPrincipal = await SetClaims(userId, username, role);

        NotifyAuthenticationStateChanged( Task.FromResult(new AuthenticationState(claimsPrincipal)));

        return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    private async Task<ClaimsPrincipal> SetClaims(string? userId, string? username, string? role)
    {
        var check = string.IsNullOrEmpty(userId)
                    || string.IsNullOrEmpty(username)
                    || string.IsNullOrEmpty(role);

        if (check)
            return new ClaimsPrincipal();

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
            new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,userId!),
                new Claim(ClaimTypes.Name,username!),
                new Claim(ClaimTypes.Role,role!)
            },authenticationType:"JwtAuth"
            ));

        return claimsPrincipal;
    }

    private async Task<Tuple<string?, string?, string?>> ParseJwtToken()
    {
        var token = await _storageService.GetToken();

        if(string.IsNullOrEmpty(token))
            return new(null,null,null);

        var jwtSecurity = new JwtSecurityTokenHandler();

        var parsedToken = jwtSecurity.ReadJwtToken(token);

        var userId = parsedToken.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        var username = parsedToken.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;

        var role = parsedToken.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role)!.Value;

        return new(userId, username, role);
    }


}