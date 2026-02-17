using NPI.Shared.Enums;

namespace NPI.Data.Entities;

public class NpiRecord : AuditableEntity
{
    public string ItemCode { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
    public string? QuoteNumber { get; set; }
    public NpiStatus Status { get; set; } = NpiStatus.Draft;

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

    // Navigation properties
    public ICollection<SetupQuestion> SetupQuestions { get; set; } = new List<SetupQuestion>();
    public ICollection<PilotRequirement> PilotRequirements { get; set; } = new List<PilotRequirement>();
    public ICollection<PlannerQuestion> PlannerQuestions { get; set; } = new List<PlannerQuestion>();
    public FormulaSpec? FormulaSpec { get; set; }
    public ICollection<LaborItem> LaborItems { get; set; } = new List<LaborItem>();
    public ICollection<PackagingComponent> PackagingComponents { get; set; } = new List<PackagingComponent>();
    public ICollection<PackagingOption> PackagingOptions { get; set; } = new List<PackagingOption>();
    public ICollection<NpiDocument> Documents { get; set; } = new List<NpiDocument>();
    public ICollection<NpiNote> Notes { get; set; } = new List<NpiNote>();
    public ICollection<ChangeLogEntry> ChangeLog { get; set; } = new List<ChangeLogEntry>();
}
