using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SeguridadToken.Presentacion.MyAuthentication;

public class MyAuthenticationStateProviderService : AuthenticationStateProvider
{
    private readonly ISessionStorageService _sessionStorageService;

    public MyAuthenticationStateProviderService(ISessionStorageService sessionStorageService)
    {
        _sessionStorageService = sessionStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var email = string.Empty;
        try
        {
            email = await _sessionStorageService.GetItemAsync<string>("email");
        }
        catch
        {
            email = null;
        }
        ClaimsIdentity identity;
        if (email != null)
        {
            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email),
            }, "apiauth_type");
        }
        else
        {
            identity = new ClaimsIdentity();
        }

        var user = new ClaimsPrincipal(identity);
        return await Task.FromResult(new AuthenticationState(user));
    }

    public void UserAuthenticated(string email)
    {
        var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email),
            }, "apiauth_type");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void Logout()
    {
        _sessionStorageService.RemoveItemAsync("email");
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}
