using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NPI.Client.DTOs.Auth;
using NPI.Client.Providers;
using NPI.Client.Services;
using NPI.Shared.DTOs;

namespace NPI.Client.ViewModels
{
    public class HeaderViewModel : ViewModelBase
    {
        private readonly INpiService _service;
        private readonly IAuthService _authService;
        private readonly AuthStateProvider _authStateProvider;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime JS;
        public HeaderViewModel(INpiService service, AuthStateProvider authStateProvider, IJSRuntime js, IAuthService authService, NavigationManager navigationManager)
        {
            _service = service;
            _authStateProvider = authStateProvider;
            _navigationManager = navigationManager;
            JS = js;
            _authService = authService;
        }

        private UserData _userData = new();
        public UserData UserData
        {
            get => _userData;
            set => SetProperty(ref _userData, value);
        }

        public async Task LoadUserDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                UserData = await _authService.GetCurrentUserAsync() ?? new UserData(); 
            });
            var currentAuthState = await _authStateProvider.GetAuthenticationStateAsync();
            if (currentAuthState.User.Identity?.IsAuthenticated != true)
            {
                _navigationManager.NavigateTo("/login");
            }
        }

        public async Task Logout()
        {
            await _authStateProvider.MarkUserAsLoggedOut();
        }

    }
}
