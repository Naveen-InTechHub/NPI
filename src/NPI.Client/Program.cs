using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NPI.Client;
using NPI.Client.Providers;
using NPI.Client.Services;
using NPI.Client.ViewModels;
using NPI.Client.ViewModels.Auth;
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazoredLocalStorage();

// API HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7200/")
});

builder.Services.AddSyncfusionBlazor();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzg0MjUyNUAzMjM5MmUzMDJlMzAzYjMyMzkzYkNySG5zVmRlTjlyZExQSzVQUXJHWjVKaWNlWmhSdTJPUUUrcnc0UlA0OVE9");

// Services
builder.Services.AddScoped<INpiService, NpiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<AuthStateProvider>());


// ViewModels (Transient — new instance per component)
builder.Services.AddTransient<NpiDetailViewModel>();
builder.Services.AddTransient<NpiListViewModel>();
builder.Services.AddTransient<HeaderViewModel>();
builder.Services.AddTransient<LoginViewModel>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<ToastService>();


await builder.Build().RunAsync();
