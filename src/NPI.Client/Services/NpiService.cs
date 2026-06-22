using System.Net.Http.Json;
using NPI.Shared.DTOs;
using NPI.Shared.Enums;

namespace NPI.Client.Services;

// ══════════════════════════════════════════════════════
// NPI Record Service Interface
// ══════════════════════════════════════════════════════
public interface INpiService
{
    // Records
    Task<List<NpiRecordDto>> GetAllAsync(string? search = null, NpiStatus? status = null);
    Task<NpiRecordDto?> GetByIdAsync(int id);
    Task<NpiRecordDto?> GetSetupAsync(int id);
    Task<NpiRecordDto?> GetBomAsync(int id);
    Task<NpiRecordDto?> GetPackagingAsync(int id);
    Task<NpiRecordDto?> GetDocsAsync(int id);
    Task<NpiRecordDto?> CreateAsync(NpiRecordDto dto);
    Task<NpiRecordDto?> UpdateAsync(int id, NpiRecordDto dto);
    Task<string?> UpdateFGGSSetupAsync(int npiId, FGGSSetupDto dto);
    Task<bool> DeleteAsync(int id);
    Task<decimal> GetLaborTotalAsync(int id, LaborCategory category);

    // Setup Questions
    Task<SetupQuestionDto?> ToggleSetupQuestionAsync(int npiId, int questionId, ToggleRequest req);
    Task<QualityPackageQuestionDto?> ToggleQualityPackageQuestionAsync(int npiId, int questionId, ToggleRequest req);

    // Pilot Requirements
    Task<PilotRequirementDto?> TogglePilotAsync(int npiId, int reqId, ToggleRequest req);

    // Planner Questions
    Task<PlannerQuestionDto?> TogglePlannerAsync(int npiId, int qId, ToggleRequest req);

    // Packaging Components
    Task<List<PackagingComponentDto>> GetPackagingComponentsAsync(int npiId);
    Task<PackagingComponentDto?> AddPackagingComponentAsync(int npiId, PackagingComponentDto dto);
    Task<PackagingComponentDto?> UpdatePackagingComponentAsync(int npiId, int id, PackagingComponentDto dto);
    Task<bool> DeletePackagingComponentAsync(int npiId, int id);

    // Notes
    Task<List<NpiNoteDto>> GetNotesAsync(int npiId);
    Task<NpiNoteDto?> AddNoteAsync(int npiId, NpiNoteDto dto);
    Task<bool> DeleteNoteAsync(int npiId, int id);

    // Formula Spec
    Task<bool> UpdateFormulsSpec(int id, FormulaSpecDto dto);

    // Change Log
    Task<List<ChangeLogEntryDto>> GetChangeLogAsync(int npiId);
    // Export
    Task<byte[]?> ExportAsync(string? search = null, NpiStatus? status = null, List<int>? SelectedIds = null);

    Task<bool> DeleteFile(int documentId);
}

// ══════════════════════════════════════════════════════
// Implementation using HttpClient
// ══════════════════════════════════════════════════════
public class NpiService : INpiService
{
    private readonly HttpClient _http;

    public NpiService(HttpClient http)
    {
        _http = http;
    }

    // ── Helpers ──
    private async Task<T?> GetAsync<T>(string url)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<T>>(url);
        return response is { Success: true } ? response.Data : default;
    }

    private async Task<T?> PostAsync<T>(string url, object body)
    {
        var response = await _http.PostAsJsonAsync(url, body);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return result is { Success: true } ? result.Data : default;
    }

    private async Task<T?> PutAsync<T>(string url, object body)
    {
        var response = await _http.PutAsJsonAsync(url, body);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return result is { Success: true } ? result.Data : default;
    }

    private async Task<bool> DeleteAsync(string url)
    {
        var response = await _http.DeleteAsync(url);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        return result?.Success ?? false;
    }

    // ── Records ──
    public async Task<List<NpiRecordDto>> GetAllAsync(string? search = null, NpiStatus? status = null)
    {
        var url = "api/npirecords";
        var @params = new List<string>();
        if (!string.IsNullOrEmpty(search)) @params.Add($"search={Uri.EscapeDataString(search)}");
        if (status.HasValue) @params.Add($"status={(int)status}");
        if (@params.Count > 0) url += "?" + string.Join("&", @params);
        return await GetAsync<List<NpiRecordDto>>(url) ?? new();
    }

    public Task<NpiRecordDto?> GetByIdAsync(int id) => GetAsync<NpiRecordDto>($"api/npirecords/{id}");
    public Task<NpiRecordDto?> GetSetupAsync(int id) => GetAsync<NpiRecordDto>($"api/npirecords/{id}/setup");
    public Task<NpiRecordDto?> GetBomAsync(int id) => GetAsync<NpiRecordDto>($"api/npirecords/{id}/bom");
    public Task<NpiRecordDto?> GetPackagingAsync(int id) => GetAsync<NpiRecordDto>($"api/npirecords/{id}/packaging");
    public Task<NpiRecordDto?> GetDocsAsync(int id) => GetAsync<NpiRecordDto>($"api/npirecords/{id}/docs");
    public Task<NpiRecordDto?> CreateAsync(NpiRecordDto dto) => PostAsync<NpiRecordDto>("api/npirecords", dto);
    public Task<NpiRecordDto?> UpdateAsync(int id, NpiRecordDto dto) => PutAsync<NpiRecordDto>($"api/npirecords/{id}", dto);
    Task<bool> INpiService.DeleteAsync(int id) => DeleteAsync($"api/npirecords/{id}");

    public async Task<decimal> GetLaborTotalAsync(int id, LaborCategory category)
        => await GetAsync<decimal>($"api/npirecords/{id}/labor-total/{category}");

    // ── Toggle endpoints ──
    public Task<SetupQuestionDto?> ToggleSetupQuestionAsync(int npiId, int questionId, ToggleRequest req)
        => PutAsync<SetupQuestionDto>($"api/npirecords/{npiId}/setup-questions/{questionId}/toggle", req);

    public Task<PilotRequirementDto?> TogglePilotAsync(int npiId, int reqId, ToggleRequest req)
        => PutAsync<PilotRequirementDto>($"api/npirecords/{npiId}/pilot-requirements/{reqId}/toggle", req);

    public Task<PlannerQuestionDto?> TogglePlannerAsync(int npiId, int qId, ToggleRequest req)
        => PutAsync<PlannerQuestionDto>($"api/npirecords/{npiId}/planner-questions/{qId}/toggle", req);

    // ── Packaging ──
    public async Task<List<PackagingComponentDto>> GetPackagingComponentsAsync(int npiId)
        => await GetAsync<List<PackagingComponentDto>>($"api/npirecords/{npiId}/packaging-components") ?? new();

    public Task<PackagingComponentDto?> AddPackagingComponentAsync(int npiId, PackagingComponentDto dto)
        => PostAsync<PackagingComponentDto>($"api/npirecords/{npiId}/packaging-components", dto);

    public Task<PackagingComponentDto?> UpdatePackagingComponentAsync(int npiId, int id, PackagingComponentDto dto)
        => PutAsync<PackagingComponentDto>($"api/npirecords/{npiId}/packaging-components/{id}", dto);

    public Task<bool> DeletePackagingComponentAsync(int npiId, int id)
        => DeleteAsync($"api/npirecords/{npiId}/packaging-components/{id}");

    // ── Notes ──
    public async Task<List<NpiNoteDto>> GetNotesAsync(int npiId)
        => await GetAsync<List<NpiNoteDto>>($"api/npirecords/{npiId}/notes") ?? new();

    public Task<NpiNoteDto?> AddNoteAsync(int npiId, NpiNoteDto dto)
        => PostAsync<NpiNoteDto>($"api/npirecords/{npiId}/notes", dto);

    public Task<bool> DeleteNoteAsync(int npiId, int id)
        => DeleteAsync($"api/npirecords/{npiId}/notes/{id}");

    // ── Change Log ──
    public async Task<List<ChangeLogEntryDto>> GetChangeLogAsync(int npiId)
        => await GetAsync<List<ChangeLogEntryDto>>($"api/npirecords/{npiId}/changelog") ?? new();

    public async Task<byte[]?> ExportAsync(string? search = null, NpiStatus? status = null, List<int>? selectedIds = null)
    {
        var request = new ExportRequest
        {
            Search = search,
            Status = status,
            SelectedIds = selectedIds
        };

        var response = await _http.PostAsJsonAsync("api/npirecords/export", request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadAsByteArrayAsync();
    }

    public Task<bool> UpdateFormulsSpec(int id, FormulaSpecDto dto)
     => PutAsync<bool>($"api/formulaspecs/{id}", dto);

    public Task<QualityPackageQuestionDto?> ToggleQualityPackageQuestionAsync(int npiId, int qId, ToggleRequest req)
      => PutAsync<QualityPackageQuestionDto>($"api/qaquestion/toggle?npiId={npiId}&id={qId}", req);
    public Task<string?> UpdateFGGSSetupAsync(int npiId, FGGSSetupDto dto)
   => PostAsync<string?>($"api/fggssetup?npiId={npiId}", dto);

    public Task<bool> DeleteFile(int documentId)
     => DeleteAsync($"api/npidocuments/{documentId}");
}
