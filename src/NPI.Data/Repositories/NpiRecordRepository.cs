using System.Linq.Expressions;
using System.Security.AccessControl;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using NPI.Data.Context;
using NPI.Data.Entities;
using NPI.Shared.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NPI.Data.Repositories;

// ── NpiRecord Repository Interface ──
public interface INpiRecordRepository : IRepository<NpiRecord>
{
    Task<NpiRecord?> GetFullRecordAsync(int id);
    Task<NpiRecord?> GetRecordWithSetupAsync(int id);
    Task<NpiRecord?> GetRecordWithBomAsync(int id);
    Task<NpiRecord?> GetRecordWithPackagingAsync(int id);
    Task<NpiRecord?> GetRecordWithDocsAsync(int id);
    Task<IReadOnlyList<NpiRecord>> GetByStatusAsync(NpiStatus status);
    Task<IReadOnlyList<NpiRecord>> SearchAsync(string? searchTerm, NpiStatus? status, int page, int pageSize);
    Task<byte[]> ExportAsync(string? searchTerm, NpiStatus? status, List<int>? selectedIds);
    Task<decimal> GetTotalLaborCostAsync(int npiRecordId, LaborCategory category);
    Task<int> RemoveDuplicatesByItemCodeAsync(string itemCode);
    Task<int> RemoveAllDuplicatesAsync();

}

// ── NpiRecord Repository Implementation ──
public class NpiRecordRepository : Repository<NpiRecord>, INpiRecordRepository
{
    public NpiRecordRepository(NpiDbContext context) : base(context) { }

    /// <summary>Full eager load of entire NPI record with all children</summary>
    public async Task<NpiRecord?> GetFullRecordAsync(int id)
    {
        return await _dbSet
            .Include(r => r.SetupQuestions)
            .Include(r => r.PilotRequirements)
            .Include(r => r.PlannerQuestions)
            .Include(r => r.QualityPackageQuestion)
            .Include(r => r.FGGSSetup)
            .Include(r => r.FormulaSpec)
            .Include(r => r.LaborItems.OrderBy(l => l.SortOrder))
            .Include(r => r.PackagingComponents.OrderBy(p => p.SortOrder))
            .Include(r => r.PackagingOptions)
            .Include(r => r.Documents)
            .Include(r => r.Notes.OrderByDescending(n => n.CreatedDate))
            .Include(r => r.ChangeLog.OrderByDescending(c => c.ChangedDate))
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id); 
    }



    /// <summary>Load Setup & Pilot tab data only</summary>
    public async Task<NpiRecord?> GetRecordWithSetupAsync(int id)
    {
        return await _dbSet
            .Include(r => r.SetupQuestions)
            .Include(r => r.PilotRequirements)
            .Include(r => r.PlannerQuestions)
            .Include(r => r.QualityPackageQuestion)
            .Include(r => r.FGGSSetup)
            .Include(r => r.FormulaSpec)
            .Include(r => r.Notes.OrderByDescending(n => n.CreatedDate))
            .Include(r => r.ChangeLog.OrderByDescending(c => c.ChangedDate).Take(10))
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>Load Labor & BOM tab data only</summary>
    public async Task<NpiRecord?> GetRecordWithBomAsync(int id)
    {
        return await _dbSet
            .Include(r => r.LaborItems.OrderBy(l => l.Category).ThenBy(l => l.SortOrder))
            .Include(r => r.FormulaSpec)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>Load Packaging tab data only</summary>
    public async Task<NpiRecord?> GetRecordWithPackagingAsync(int id)
    {
        return await _dbSet
            .Include(r => r.PackagingComponents.OrderBy(p => p.SortOrder))
            .Include(r => r.PackagingOptions)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>Load Documents tab data only</summary>
    public async Task<NpiRecord?> GetRecordWithDocsAsync(int id)
    {
        return await _dbSet
            .Include(r => r.Documents)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>Filter by status using LINQ</summary>
    public async Task<IReadOnlyList<NpiRecord>> GetByStatusAsync(NpiStatus status)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.ModifiedDate ?? r.CreatedDate)
            .ToListAsync();
    }

    /// <summary>Search with LINQ predicates and paging</summary>
    public async Task<IReadOnlyList<NpiRecord>> SearchAsync(string? searchTerm, NpiStatus? status, int page, int pageSize)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        // Dynamic LINQ filtering
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(r =>
                r.ItemCode.ToLower().Contains(term) ||
                r.ProductDescription.ToLower().Contains(term) ||
                (r.QuoteNumber != null && r.QuoteNumber.ToLower().Contains(term)) ||
                (r.CustomerName != null && r.CustomerName.ToLower().Contains(term)) ||
                (r.BulkCode != null && r.BulkCode.ToLower().Contains(term)) ||
                (r.FGCode != null && r.FGCode.ToLower().Contains(term)));
        }

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        return await query
            .OrderByDescending(r => r.ModifiedDate ?? r.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>Aggregate LINQ query for total labor cost by category</summary>
    public async Task<decimal> GetTotalLaborCostAsync(int npiRecordId, LaborCategory category)
    {
        return await _context.LaborItems
            .Where(l => l.NpiRecordId == npiRecordId && l.Category == category)
            .SumAsync(l => l.Cost);
    }
    public async Task<int> RemoveDuplicatesByItemCodeAsync(string itemCode)
    {
        var records = await _context.NpiRecords
            .Where(x => x.ItemCode == itemCode)
            .OrderBy(x => x.Id)
            .ToListAsync();

        if (records.Count <= 1)
            return 0;

        var duplicates = records.Skip(1).ToList();

        _context.NpiRecords.RemoveRange(duplicates);

        return duplicates.Count;
    }

    public async Task<int> RemoveAllDuplicatesAsync()
    {
        var duplicateGroups = await _context.NpiRecords
            .GroupBy(x => x.ItemCode)
            .Where(g => g.Count() > 1)
            .ToListAsync();

        var toDelete = new List<NpiRecord>();

        foreach (var group in duplicateGroups)
        {
            var duplicates = group
                .OrderBy(x => x.Id)
                .Skip(1);

            toDelete.AddRange(duplicates);
        }

        _context.NpiRecords.RemoveRange(toDelete);

        return toDelete.Count;
    }
    public async Task<byte[]> ExportAsync(string? searchTerm, NpiStatus? status, List<int>? selectedIds)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();

            query = query.Where(r =>
                r.ItemCode.ToLower().Contains(term) ||
                r.ProductDescription.ToLower().Contains(term) ||
                (r.QuoteNumber != null && r.QuoteNumber.ToLower().Contains(term)) ||
                (r.CustomerName != null && r.CustomerName.ToLower().Contains(term)) ||
                (r.BulkCode != null && r.BulkCode.ToLower().Contains(term)) ||
                (r.FGCode != null && r.FGCode.ToLower().Contains(term)));
        }

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);
        if(selectedIds != null && selectedIds.Any())
            query = query.Where(r => selectedIds.Contains(r.Id));

        var result = await query.OrderByDescending(x => x.Id).ToListAsync();

        if (!result.Any())
            throw new Exception("No data to export.");

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Bids Won");

        // =========================
        // HEADER
        // =========================

        worksheet.Cell(1, 1).Value = "Customer Name";
        worksheet.Cell(1, 2).Value = "Item Code";
        worksheet.Cell(1, 3).Value = "FG Code";
        worksheet.Cell(1, 4).Value = "Bulk Code";
        worksheet.Cell(1, 5).Value = "Created By";
        worksheet.Cell(1, 6).Value = "Created Date";
        worksheet.Cell(1, 7).Value = "Status";
        worksheet.Cell(1, 8).Value = "Packout Description";
        worksheet.Cell(1, 9).Value = "Product Description";

        var headerRange = worksheet.Range(1, 1, 1, 9);

        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        worksheet.Row(1).Height = 25;

        // Freeze Header
        worksheet.SheetView.FreezeRows(1);

        // =========================
        // DATA
        // =========================

        int row = 2;

        foreach (var item in result)
        {
            worksheet.Cell(row, 1).Value = item.CustomerName;
            worksheet.Cell(row, 2).Value = item.ItemCode;
            worksheet.Cell(row, 3).Value = item.FGCode;
            worksheet.Cell(row, 4).Value = item.BulkCode;
            worksheet.Cell(row, 5).Value = string.IsNullOrEmpty(item.CreatedBy)?"User" : item.CreatedBy;

            worksheet.Cell(row, 6).Value = item.CreatedDate;
            worksheet.Cell(row, 6).Style.DateFormat.Format = "dd-MMM-yyyy hh:mm AM/PM";

            worksheet.Cell(row, 7).Value = item.Status.ToString();
            worksheet.Cell(row, 8).Value = item.PackoutDescription;
            worksheet.Cell(row, 9).Value = item.ProductDescription;

            row++;
        }

        // =========================
        // TABLE BORDERS
        // =========================

        var tableRange = worksheet.Range(1, 1, row - 1, 9);
        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // =========================
        // COLUMN WIDTHS
        // =========================

        worksheet.Columns(1, 5).AdjustToContents();
        worksheet.Column(6).Width = 22; // Date column fixed width
        worksheet.Columns(7, 9).AdjustToContents();

        // =========================
        // SAVE
        // =========================

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return stream.ToArray();
    }

}
