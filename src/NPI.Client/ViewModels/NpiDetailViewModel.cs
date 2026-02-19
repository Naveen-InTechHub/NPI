using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using NPI.Client.Services;
using NPI.Shared.DTOs;
using NPI.Shared.Enums;
using NPI.Client.Providers;

namespace NPI.Client.ViewModels;

/// <summary>
/// MVVM ViewModel for the NPI Detail page.
/// Manages all tab data, active tab state, theme, and user interactions.
/// </summary>
public class NpiDetailViewModel : ViewModelBase
{
    private readonly INpiService _service;
    private readonly AuthStateProvider _authStateProvider;
    private readonly IAuthService _authService;
    private readonly NavigationManager _navigationManager;
    private readonly ToastService _toastService;

    public NpiDetailViewModel(INpiService service, AuthStateProvider authStateProvider, IAuthService authService, NavigationManager navigationManager, ToastService toastService)
    {
        _service = service;
        _authStateProvider = authStateProvider;
        _authService = authService;
        _navigationManager = navigationManager;
        _toastService = toastService;

    }

    // ── Core State ──

    private NpiRecordDto? _record;
    public NpiRecordDto? Record
    {
        get => _record;
        set => SetProperty(ref _record, value);
    }

    private string _activeTab = "setup";
    public string ActiveTab
    {
        get => _activeTab;
        set => SetProperty(ref _activeTab, value);
    }

    private bool _isDarkTheme;
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set => SetProperty(ref _isDarkTheme, value);
    }

    private string _noteText = string.Empty;
    public string NoteText
    {
        get => _noteText;
        set => SetProperty(ref _noteText, value);
    }

    private string _setupNoteText = string.Empty;
    public string SetupNoteText
    {
        get => _setupNoteText;
        set => SetProperty(ref _setupNoteText, value);
    }

    private string _pilotNoteText = string.Empty;
    public string PilotNoteText
    {
        get => _pilotNoteText;
        set => SetProperty(ref _pilotNoteText, value);
    }

    private string _currentUser = string.Empty;
    public string CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value);
    }
    // ── Computed Properties (LINQ over loaded data) ──

    public List<SetupQuestionDto> SetupQuestions
        => Record?.SetupQuestions ?? new();

    public List<PilotRequirementDto> PilotRequirements
        => Record?.PilotRequirements ?? new();

    public List<PlannerQuestionDto> RawMaterialQuestions
        => Record?.PlannerQuestions?.Where(q => q.Category == PlannerCategory.RawMaterials).ToList() ?? new();

    public List<PlannerQuestionDto> ComponentQuestions
        => Record?.PlannerQuestions?.Where(q => q.Category == PlannerCategory.Components).ToList() ?? new();

    public List<PlannerQuestionDto> ComponentAssesmentsQuestion
       => Record?.PlannerQuestions?.Where(q => q.Category == PlannerCategory.Components && (q.QuestionKey == "newcomponents" || q.QuestionKey == "fromnewvendor" || q.QuestionKey == "customersupplied") ).ToList() ?? new();

    public List<PlannerQuestionDto> ArtWorkQuestion
    => Record?.PlannerQuestions?.Where(q => q.Category == PlannerCategory.Components && (q.QuestionKey == "regulatorycerts" || q.QuestionKey == "newartwork")).ToList() ?? new();

    public FormulaSpecDto? FormulaSpec
        => Record?.FormulaSpec;

    public List<LaborItemDto> FormulaLabor
        => Record?.LaborItems?.Where(l => l.Category == LaborCategory.Formula && !l.IsOverhead).OrderBy(l => l.SortOrder).ToList() ?? new();

    public List<LaborItemDto> FormulaOverhead
        => Record?.LaborItems?.Where(l => l.Category == LaborCategory.Formula && l.IsOverhead).OrderBy(l => l.SortOrder).ToList() ?? new();

    public List<LaborItemDto> PackoutLabor
        => Record?.LaborItems?.Where(l => l.Category == LaborCategory.Packout && !l.IsOverhead).OrderBy(l => l.SortOrder).ToList() ?? new();

    public List<LaborItemDto> PackoutOverhead
        => Record?.LaborItems?.Where(l => l.Category == LaborCategory.Packout && l.IsOverhead).OrderBy(l => l.SortOrder).ToList() ?? new();

    public decimal FormulaLaborTotal
        => Record?.LaborItems?.Where(l => l.Category == LaborCategory.Formula).Sum(l => l.Cost) ?? 0m;

    public List<PackagingComponentDto> PackagingComponents
        => Record?.PackagingComponents?.OrderBy(c => c.SortOrder).ToList() ?? new();

    public List<PackagingOptionDto> PackagingOptions
        => Record?.PackagingOptions ?? new();

    public List<NpiDocumentDto> Documents
        => Record?.Documents ?? new();

    public List<NpiNoteDto> Notes
        => Record?.Notes?.Where(x => x.Parent == NotesParent.Notes).OrderByDescending(n => n.CreatedDate).ToList() ?? new();
    public List<NpiNoteDto> SetupNotes
       => Record?.Notes?.Where(x => x.Parent == NotesParent.Setup).OrderByDescending(n => n.CreatedDate).ToList() ?? new();
    public List<NpiNoteDto> PilotNotes
      => Record?.Notes?.Where(x => x.Parent == NotesParent.Pilot).OrderByDescending(n => n.CreatedDate).ToList() ?? new();

    public List<ChangeLogEntryDto> ChangeLog
        => Record?.ChangeLog?.OrderByDescending(c => c.ChangedDate).ToList() ?? new();

    // ── Metric helpers ──

    public string StatusDisplay => Record?.Status switch
    {
        NpiStatus.Draft => "Draft",
        NpiStatus.InProgress => "In Progress",
        NpiStatus.PlannerLaunched => "Planner Launched",
        NpiStatus.Launched => "● Launched",
        NpiStatus.Completed => "✓ Completed",
        NpiStatus.Cancelled => "✕ Cancelled",
        _ => "Unknown"
    };

    public string StatusBadgeClass => Record?.Status switch
    {
        NpiStatus.Launched or NpiStatus.Completed => "badge-success",
        NpiStatus.Cancelled => "badge-warn",
        _ => "badge-info"
    };

    // ═══════════════════════════════════════════
    // Commands
    // ═══════════════════════════════════════════

    /// <summary>Load full NPI record</summary>
    public async Task LoadAsync(int id)
    {
        await ExecuteAsync(async () =>
        {
            Record = await _service.GetByIdAsync(id);
            if (Record == null)
                ErrorMessage = "NPI record not found.";
        });
        
    }

    /// <summary>Switch tab</summary>
    public void SwitchTab(string tab)
    {
        ActiveTab = tab;
    }

    /// <summary>Toggle theme</summary>
    //public void ToggleTheme()
    //{
    //    IsDarkTheme = !IsDarkTheme;
    //}

    /// <summary>Toggle a Setup question Y/N</summary>
    public async Task ToggleSetupQuestionAsync(SetupQuestionDto question, bool? newValue)
    {
       
        question.Value = newValue;
        if (Record == null) return;
        await ExecuteAsync(async () =>
        {
            var result = await _service.ToggleSetupQuestionAsync(
                Record.Id, question.Id,
                new ToggleRequest { Value = newValue, AnsweredBy = CurrentUser ?? "User" });
            if (result != null)
            {
                var idx = Record.SetupQuestions.FindIndex(q => q.Id == question.Id);
                if (idx >= 0) Record.SetupQuestions[idx] = result;
                OnPropertyChanged(nameof(SetupQuestions));
            }
        });
        await LoadAsync(Record.Id); // Refresh all data to reflect any cascading changes
    }

    /// <summary>Toggle a Pilot requirement Y/N</summary>
    public async Task TogglePilotAsync(PilotRequirementDto req, bool? newValue)
    {
        if (Record == null) return;
        await ExecuteAsync(async () =>
        {
            var result = await _service.TogglePilotAsync(
                Record.Id, req.Id,
                new ToggleRequest { Value = newValue, AnsweredBy = CurrentUser });

            if (result != null)
            {
                var idx = Record.PilotRequirements.FindIndex(r => r.Id == req.Id);
                if (idx >= 0) Record.PilotRequirements[idx] = result;
                OnPropertyChanged(nameof(PilotRequirements));
            }
        });
        await LoadAsync(Record.Id); // Refresh all data to reflect any cascading changes
    }

    /// <summary>Toggle a Planner question Y/N</summary>
    public async Task TogglePlannerAsync(PlannerQuestionDto q, bool? newValue)
    {
        if (Record == null) return;
        await ExecuteAsync(async () =>
        {
            var result = await _service.TogglePlannerAsync(
                Record.Id, q.Id,
                new ToggleRequest { Value = newValue, AnsweredBy = CurrentUser });

            if (result != null)
            {
                var idx = Record.PlannerQuestions.FindIndex(x => x.Id == q.Id);
                if (idx >= 0) Record.PlannerQuestions[idx] = result;
                OnPropertyChanged(nameof(RawMaterialQuestions));
                OnPropertyChanged(nameof(ComponentQuestions));
            }
        });
        await LoadAsync(Record.Id); // Refresh all data to reflect any cascading changes
    }

    /// <summary>Save header fields</summary>
    public async Task SaveRecordAsync()
    {
        if (Record == null) return;
        await ExecuteAsync(async () =>
        {
            Record.ModifiedBy = CurrentUser;
            var result = await _service.UpdateAsync(Record.Id, Record);
            if (result != null) Record = result;
        });
        _toastService.ShowSuccess("Record saved successfully.");
    }

    /// <summary>Add a new note</summary>
    public async Task AddNoteAsync(string parent, string noteText)
    {
        if (Record == null || string.IsNullOrWhiteSpace(noteText)) return;
        await ExecuteAsync(async () =>
        {
            var result = await _service.AddNoteAsync(Record.Id, new NpiNoteDto
            {
                Content = noteText,
                Parent = parent,
                CreatedBy = CurrentUser
            });
            if (result != null)
            {
                Record.Notes.Insert(0, result);
                NoteText = string.Empty;
                PilotNoteText = string.Empty;
                SetupNoteText = string.Empty;
                OnPropertyChanged(nameof(Notes));
            }
        });
        _toastService.ShowSuccess("Note added successfully.");
    }

    /// <summary>Delete a packaging component</summary>
    public async Task DeletePackagingComponentAsync(PackagingComponentDto comp)
    {
        if (Record == null) return;
        await ExecuteAsync(async () =>
        {
            var success = await _service.DeletePackagingComponentAsync(Record.Id, comp.Id);
            if (success)
            {
                Record.PackagingComponents.Remove(comp);
                OnPropertyChanged(nameof(PackagingComponents));
            }
        });
    }

    /// <summary>Add new packaging component</summary>
    public async Task AddPackagingComponentAsync(PackagingComponentDto dto)
    {
        if (Record == null) return;
        await ExecuteAsync(async () =>
        {
            var result = await _service.AddPackagingComponentAsync(Record.Id, dto);
            if (result != null)
            {
                Record.PackagingComponents.Add(result);
                OnPropertyChanged(nameof(PackagingComponents));
            }
        });
    }

    private Dictionary<DocumentSlot, IBrowserFile> _selectedFiles = new();

    public async Task Logout()
    {
        await _authStateProvider.MarkUserAsLoggedOut();
    }

    public void NavToBids()
    {
        _navigationManager.NavigateTo("/");
    }

    public async Task UpdateItemCodesAxBom(NpiRecordDto dto)
    {
        if (Record == null) return;
        await ExecuteAsync(async () =>
        {
            var result = await _service.UpdateAsync(Record.Id, dto);
            if (result != null)
            {
                Record.Barcode = result.Barcode;
                Record.ItemCode = result.ItemCode;
                Record.ItemDescription = result.ItemDescription;
                
                OnPropertyChanged(nameof(Record.Barcode));
            }
        });
        _toastService.ShowSuccess("Item codes updated successfully.");
    }


}


