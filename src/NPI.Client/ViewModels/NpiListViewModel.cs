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
    private readonly NavigationManager _navigationManager;

    private readonly IJSRuntime JS;
    public NpiListViewModel(INpiService service, AuthStateProvider authStateProvider, IJSRuntime js, NavigationManager navigationManager)
    {
        _service = service;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
        JS = js;
    }

    private List<NpiRecordDto> _records = new();
    public List<NpiRecordDto> Records
    {
        get => _records;
        set => SetProperty(ref _records, value);
    }
    private List<int>? _selectedIds = new();
    public List<int>? SelectedIds
    {
        get => _selectedIds;
        set => SetProperty(ref _selectedIds, value);
    }
    private bool _allSelected;
    public bool AllSelected
    {
        get => _allSelected;
        set
        {
            _allSelected = value;
            ToggleSelectAll(value);
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
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
            var bytes = await _service.ExportAsync(SearchTerm, StatusFilter, SelectedIds);

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

    public void NavigateToNPISetup(int npiId)
    {
        _navigationManager.NavigateTo($"/npi/{npiId}");
    }

    // ── Status CSS ──────────────────────────────────────────
    public string StatusClass(NpiStatus status) => status switch
    {
        NpiStatus.Launched => "badge-launched",
        NpiStatus.InProgress => "badge-inprogress",
        NpiStatus.Draft => "badge-draft",
        _ => "badge-draft"
    };
    public string GetStatusText(NpiStatus status) => status switch
    {
        NpiStatus.InProgress => "In Progress",
        NpiStatus.PlannerLaunched => "Planner Launched",
        _ => status.ToString()
    };


    public void ToggleSelectAll(bool isSelected)
    {
        foreach (var r in Records) r.Selected = isSelected;
        if(!isSelected)
            SelectedIds = new();
    }
    public void ToggleRow(NpiRecordDto row, bool isChecked)
    {
        row.Selected = isChecked;

        if (isChecked)
        {
            if (!SelectedIds?.Contains(row.Id) ?? false)
                SelectedIds?.Add(row.Id);
        }
        else
        {
            SelectedIds?.Remove(row.Id);
        }

    }

}
