using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NPI.Client.Providers;
using NPI.Client.Services;
using NPI.Shared.DTOs;
using NPI.Shared.Enums;
using static System.Net.WebRequestMethods;

namespace NPI.Client.ViewModels;

public class NpiListViewModel : ViewModelBase
{
    private readonly INpiService _service;
    private readonly AuthStateProvider _authStateProvider;
    private readonly IJSRuntime JS;
    public NpiListViewModel(INpiService service,AuthStateProvider authStateProvider, IJSRuntime js) 
    {
        _service = service;
        _authStateProvider = authStateProvider;
        JS = js;
    }

    private List<NpiRecordDto> _records = new();
    public List<NpiRecordDto> Records
    {
        get => _records;
        set => SetProperty(ref _records, value);
    }

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set => SetProperty(ref _searchTerm, value);
    }

    private NpiStatus? _statusFilter;
    public NpiStatus? StatusFilter
    {
        get => _statusFilter;
        set => SetProperty(ref _statusFilter, value);
    }

    public async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            Records = await _service.GetAllAsync(SearchTerm, StatusFilter);
        });
    }

    public async Task SearchAsync()
    {
        await LoadAsync();
    }

    public async Task Logout()
    {
      await _authStateProvider.MarkUserAsLoggedOut();
    }

    public async Task ExportFile()
    {
        await ExecuteAsync(async () =>
        {
            var bytes = await _service.ExportAsync(SearchTerm, StatusFilter);

            if (bytes == null)
            {
                ErrorMessage = "Failed to export data.";
                return;
            }

            await JS.InvokeVoidAsync("saveAsFile",
                "BidsWon.xlsx",
                Convert.ToBase64String(bytes));
        });
    }


}
