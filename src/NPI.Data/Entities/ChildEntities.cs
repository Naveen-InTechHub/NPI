using NPI.Shared.Enums;

namespace NPI.Data.Entities;

// ── Setup Questions ──
public class SetupQuestion : BaseEntity
{
    public int NpiRecordId { get; set; }
    public string QuestionKey { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public bool? Value { get; set; }
    public string? AnsweredBy { get; set; }
    public DateTime? AnsweredDate { get; set; }
    public string? Note { get; set; }

    public NpiRecord NpiRecord { get; set; } = null!;
}

// ── Pilot Requirements ──
public class PilotRequirement : BaseEntity
{
    public int NpiRecordId { get; set; }
    public string QuestionKey { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public bool? Value { get; set; }
    public string? AnsweredBy { get; set; }
    public DateTime? AnsweredDate { get; set; }
    public string? Note { get; set; }

    public NpiRecord NpiRecord { get; set; } = null!;
}

// ── Planner Questions ──
public class PlannerQuestion : BaseEntity
{
    public int NpiRecordId { get; set; }
    public PlannerCategory Category { get; set; }
    public string QuestionKey { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public bool? Value { get; set; }
    public string? AnsweredBy { get; set; }
    public DateTime? AnsweredDate { get; set; }

    public NpiRecord NpiRecord { get; set; } = null!;
}

// ── Formula Specification ──
public class FormulaSpec : BaseEntity
{
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

    public NpiRecord NpiRecord { get; set; } = null!;
}

// ── Labor Items ──
public class LaborItem : BaseEntity
{
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

    public NpiRecord NpiRecord { get; set; } = null!;
}

// ── Packaging Components ──
public class PackagingComponent : BaseEntity
{
    public int NpiRecordId { get; set; }
    public ComponentType ComponentType { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Qty { get; set; }
    public string UnitOfMeasure { get; set; } = "ea";
    public bool IsCustomerSupplied { get; set; }
    public int SortOrder { get; set; }

    public NpiRecord NpiRecord { get; set; } = null!;
}

// ── Packaging Options ──
public class PackagingOption : BaseEntity
{
    public int NpiRecordId { get; set; }
    public string OptionName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
    public string? SubType { get; set; }

    public NpiRecord NpiRecord { get; set; } = null!;
}

// ── Documents ──
public class NpiDocument : BaseEntity 
{
    public int NpiRecordId { get; set; }
    public DocumentSlot Slot { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long? FileSize { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime? UploadedDate { get; set; }

    public NpiRecord NpiRecord { get; set; } = null!;
}
 
// ── Notes ──
public class NpiNote : BaseEntity
{
    public int NpiRecordId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Parent { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public NpiRecord NpiRecord { get; set; } = null!;
}

// ── Change Log ──
public class ChangeLogEntry : BaseEntity
{
    public int NpiRecordId { get; set; }
    public string FieldChanged { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedDate { get; set; } = DateTime.UtcNow;

    public NpiRecord NpiRecord { get; set; } = null!;
}
