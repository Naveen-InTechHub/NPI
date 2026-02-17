using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using NPI.Client.DTOs;
using NPI.Client.DTOs.Auth;
using NPI.Client.Providers;
using System.Net.Http.Json;
using System.Text.Json;
using static NPI.Client.Components.Pages.Login;

namespace NPI.Client.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<UserData?>> LoginAsync(LoginModel request);
        Task LogoutAsync();
        Task<UserData?> GetCurrentUserAsync();
    }
    public class AuthService : IAuthService
    {
        private HttpClient Http { get; set; }
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthStateProvider _authStateProvider;
        public AuthService(HttpClient Http, NavigationManager navigationManager, ILocalStorageService localStorageService, AuthStateProvider authStateProvider)
        {
            this.Http = Http;
            _navigationManager = navigationManager;
            _localStorage = localStorageService;
            _authStateProvider = authStateProvider;

        }

        public async Task<ApiResponse<UserData?>> LoginAsync(LoginModel request)
        {
            var response = await Http.PostAsJsonAsync("api/auth/login", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserData?>>();
            UserData? loginData = apiResponse?.Data;
          
            if (loginData != null)
            {
                await _localStorage.SetItemAsync("authDetails", JsonSerializer.Serialize(loginData));
            }
            return apiResponse ?? new ApiResponse<UserData?> { Success = false, Message = "No response data." };

        }

        public async Task LogoutAsync()
        {
            await _authStateProvider.MarkUserAsLoggedOut();
        }

        public async Task<UserData?> GetCurrentUserAsync()
        {
            try
            {
                var authDetails = await _localStorage.GetItemAsync<UserData?>("currentUser");
                    return authDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving auth details: {ex.Message}");
                return null;
            }
        }
    }
}
