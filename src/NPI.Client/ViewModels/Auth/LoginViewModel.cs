using Microsoft.AspNetCore.Components;
using NPI.Client.DTOs.Auth;
using NPI.Client.Services;

namespace NPI.Client.ViewModels.Auth
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly NavigationManager _navigationManager;

        private LoginModel _loginModel = new();
        public LoginModel LoginModel
        {
            get => _loginModel;
            set => SetProperty(ref _loginModel, value);
        }

        private string? _userRole;
        public string? UserRole
        {
            get => _userRole;
            set => SetProperty(ref _userRole, value);
        }

        private bool _isSubmiting;
        public bool IsSubmiting
        {
            get => _isSubmiting;
            set => SetProperty(ref _isSubmiting, value);
        }
        public LoginViewModel(IAuthService service, NavigationManager navigationManager)
        {
            _authService = service;
            _navigationManager = navigationManager; 
        }

        public async Task LoginAsync()
        {
            await ExecuteAsync(async () =>
            {
                ErrorMessage = null;
                var response = await _authService.LoginAsync(LoginModel);
                if (response.Success)
                {
                    if (response.Data != null)
                    {
                        UserRole = response.Data.Role;
                        _navigationManager.NavigateTo("/"); // Redirect to home/dashboard after successful login    
                    }
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            });
        }

       
    }
}
