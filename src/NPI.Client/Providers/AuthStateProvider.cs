using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NPI.Client.DTOs.Auth;
using System.Security.Claims;

namespace NPI.Client.Providers;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager navigationManager;

    public AuthStateProvider(ILocalStorageService localStorage, NavigationManager navigationManager)
    {
        _localStorage = localStorage;
        this.navigationManager = navigationManager;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _localStorage.GetItemAsync<UserData>("currentUser");

        if (user == null)
        {
            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationState(principal);
    }

    public async Task MarkUserAsAuthenticated(UserData user)
    {
        await _localStorage.SetItemAsync("currentUser", user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        var principal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync("currentUser");
        navigationManager.NavigateTo("/login"); // Redirect to login page after logout
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(anonymous)));
    }
}
