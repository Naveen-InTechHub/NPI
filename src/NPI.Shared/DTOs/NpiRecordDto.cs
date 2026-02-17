using NPI.Shared.Enums;

namespace NPI.Shared.DTOs;

public class NpiRecordDto
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
    public string? QuoteNumber { get; set; }
    public NpiStatus Status { get; set; }

    // Customer
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? SalesRep1 { get; set; }
    public string? SalesRep2 { get; set; }

    // Codes
    public string? BulkCode { get; set; }
    public string? FGCode { get; set; }
    public string? PackoutDescription { get; set; }
    public ProductType ProductType { get; set; }
    public PackageType PackageType { get; set; }

    // Metrics
    public decimal OrderQty { get; set; }
    public decimal TotalUnits { get; set; }
    public decimal FormulaSize { get; set; }
    public decimal Servings { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal StdQty { get; set; }
    public int LeadTimeDays { get; set; }
    public decimal POValue { get; set; }

    // Item details
    public string? ItemDescription { get; set; }
    public string? CustomerNumber { get; set; }
    public string? Barcode { get; set; }

    // Audit
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation collections
    public List<SetupQuestionDto> SetupQuestions { get; set; } = new();
    public List<PilotRequirementDto> PilotRequirements { get; set; } = new();
    public List<PlannerQuestionDto> PlannerQuestions { get; set; } = new();
    public FormulaSpecDto? FormulaSpec { get; set; }
    public List<LaborItemDto> LaborItems { get; set; } = new();
    public List<PackagingComponentDto> PackagingComponents { get; set; } = new();
    public List<PackagingOptionDto> PackagingOptions { get; set; } = new();
    public List<NpiDocumentDto> Documents { get; set; } = new();
    public List<NpiNoteDto> Notes { get; set; } = new();
    public List<ChangeLogEntryDto> ChangeLog { get; set; } = new();
}
