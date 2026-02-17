using NPI.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPI.Shared.DTOs
{
    public class CreateNpiRecordDto
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string? QuoteNumber { get; set; }

        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? SalesRep1 { get; set; }
        public string? SalesRep2 { get; set; }

        public string? BulkCode { get; set; }
        public string? FGCode { get; set; }
        public string? PackoutDescription { get; set; }

        public ProductType ProductType { get; set; }
        public PackageType PackageType { get; set; }

        public decimal OrderQty { get; set; }
        public decimal TotalUnits { get; set; }
        public decimal FormulaSize { get; set; }
        public decimal Servings { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal StdQty { get; set; }
        public int LeadTimeDays { get; set; }
        public decimal POValue { get; set; }

        public string? ItemDescription { get; set; }
        public string? CustomerNumber { get; set; }
        public string? Barcode { get; set; }
        public int? Status { get; set; }  // 0= Draft, 1 = In Progress, 2 = Planner Launched, 3 = Launched, 4 = Completed, 5 = Cancelled
    }

}
