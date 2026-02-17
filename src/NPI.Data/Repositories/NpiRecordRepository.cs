using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NPI.Data.Context;
using NPI.Data.Entities;
using NPI.Shared.Enums;

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

}
