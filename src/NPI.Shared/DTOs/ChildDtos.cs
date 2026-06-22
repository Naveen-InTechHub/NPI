using NPI.Shared.Enums;

namespace NPI.Shared.DTOs;

// ── Setup Questions (NPI Setup tab) ──
public class SetupQuestionDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public string QuestionKey { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public bool? Value { get; set; }
    public string? AnsweredBy { get; set; }
    public DateTime? AnsweredDate { get; set; }
    public string? Note { get; set; }
}

// ── Pilot Requirements ──
public class PilotRequirementDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public string QuestionKey { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public bool? Value { get; set; }
    public string? AnsweredBy { get; set; }
    public DateTime? AnsweredDate { get; set; }
    public string? Note { get; set; }
}

// ── Planner Questions ──
public class PlannerQuestionDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public PlannerCategory Category { get; set; }
    public string QuestionKey { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public bool? Value { get; set; }
    public string? AnsweredBy { get; set; }
    public DateTime? AnsweredDate { get; set; }
}

// ── Formula Specification ──
public class FormulaSpecDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public string FormulaName { get; set; } = string.Empty;
    public string? ProdType { get; set; }
    public string? PkgType { get; set; }
    public decimal OrderQty { get; set; }
    public decimal TotalUnits { get; set; }
    public decimal ServPerUnit { get; set; }
    public decimal FormulaSize { get; set; }
    public decimal TotalServings { get; set; }
    public decimal KG { get; set; }
    public decimal Density { get; set; }
    public string? Blender { get; set; }
    public int Batches { get; set; }
}


// ── Setup Questions ──
public class QualityPackageQuestionDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public QAQuestionCategory Category { get; set; }
    public string QuestionKey { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public bool? Value { get; set; }
    public string? AnsweredBy { get; set; }
    public DateTime? AnsweredDate { get; set; }
    public string? Note { get; set; }
}
// GGS Setup Details
public class FGGSSetupDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public string DosageForm { get; set; } = string.Empty;
    public string ShapeAndColor { get; set; } = string.Empty;
    public int TabletWeight { get; set; }
    public string Dimension { get; set; } = string.Empty;
    public string CoatingType { get; set; } = string.Empty;
    public string Flavor { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    public int CountPerUnit { get; set; }
    public string ContainerMaterial { get; set; } = string.Empty;
    public string ClouserType { get; set; } = string.Empty;
    public string SecondaryPackaging { get; set; } = string.Empty;
    public int LabelVersion { get; set; }
    public string RegulatoryMarket { get; set; } = string.Empty;
    public string TargetShelfLife { get; set; } = string.Empty;
    public string StorageConditions { get; set; } = string.Empty;
    public string SpecialHandling { get; set; } = string.Empty;
}
// ── Labor Items ──
public class LaborItemDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public LaborCategory Category { get; set; }
    public bool IsOverhead { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Base { get; set; }
    public decimal? ChangeOver { get; set; }
    public decimal? Run { get; set; }
    public decimal? Machine { get; set; }
    public decimal? Head { get; set; }
    public decimal? TotalHours { get; set; }
    public decimal? UnitsPerHour { get; set; }
    public decimal Rate { get; set; }
    public decimal Cost { get; set; }
    public decimal? CostPerEach { get; set; }
    public int SortOrder { get; set; }
}

// ── Packaging Components ──
public class PackagingComponentDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public ComponentType ComponentType { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Qty { get; set; }
    public string UnitOfMeasure { get; set; } = "ea";
    public bool IsCustomerSupplied { get; set; }
    public int SortOrder { get; set; }
}

// ── Packaging Options ──
public class PackagingOptionDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public string OptionName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
    public string? SubType { get; set; }
}

// ── Documents ──
public class NpiDocumentDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public DocumentSlot Slot { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime? UploadedDate { get; set; }
}

// ── Notes ──
public class NpiNoteDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Parent { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

// ── Change Log ──
public class ChangeLogEntryDto
{
    public int Id { get; set; }
    public int NpiRecordId { get; set; }
    public string FieldChanged { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedDate { get; set; }
}

// ── API Response Wrapper ──
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string error) =>
        new() { Success = false, Errors = new List<string> { error } };

    public static ApiResponse<T> Fail(List<string> errors) =>
        new() { Success = false, Errors = errors };
}

// ── Toggle Request ──
public class ToggleRequest
{
    public bool? Value { get; set; }
    public string? AnsweredBy { get; set; }
}

// ── Paged Result ──
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

// ── ItemCodesAX Bom ──

public class ItemCodeBomDto
{
    public string ItemDesc { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string BulkCode { get; set; } = string.Empty;
    public string FGCode { get; set; } = string.Empty;
    public string BarCode { get; set; } = string.Empty;
}

// Export request

public class ExportRequest
{
    public string? Search { get; set; }
    public NpiStatus? Status { get; set; }
    public List<int>? SelectedIds { get; set; }
}

public class CreateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<int> RoleIds { get; set; } = new();
}

public class UpdateUserByEmailRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}