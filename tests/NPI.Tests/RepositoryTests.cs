using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NPI.Data.Context;
using NPI.Data.Entities;
using NPI.Data.Repositories;
using NPI.Data.UnitOfWork;
using NPI.Shared.Enums;
using Xunit;

namespace NPI.Tests;

public class RepositoryTests : IDisposable
{
    private readonly NpiDbContext _context;
    private readonly IUnitOfWork _uow;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<NpiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new NpiDbContext(options);
        _uow = new UnitOfWork(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _uow.Dispose();
    }

    // ── Helpers ──

    private async Task<NpiRecord> SeedRecordAsync(string code = "TEST01")
    {
        var record = new NpiRecord
        {
            ItemCode = code,
            ProductDescription = "Test Product",
            Status = NpiStatus.Draft,
            OrderQty = 50_000m,
            CreatedDate = DateTime.UtcNow,
            SetupQuestions = new List<SetupQuestion>
            {
                new() { QuestionKey = "existingBulk", QuestionText = "Existing bulk?", Value = true, AnsweredBy = "Test" },
                new() { QuestionKey = "multiplePacks", QuestionText = "Multiple packs?", Value = false }
            },
            LaborItems = new List<LaborItem>
            {
                new() { Category = LaborCategory.Formula, Description = "Mixing", Rate = 37.89m, Cost = 968m, SortOrder = 1 },
                new() { Category = LaborCategory.Formula, Description = "Compression", Rate = 37.89m, Cost = 8456m, SortOrder = 2 },
                new() { Category = LaborCategory.Packout, Description = "Bottle Labor", Rate = 37.89m, Cost = 18653m, SortOrder = 3 },
            }
        };
        await _uow.NpiRecords.AddAsync(record);
        await _uow.SaveChangesAsync();
        return record;
    }

    // ════════════════════════════════════════════
    // Generic Repository Tests
    // ════════════════════════════════════════════

    [Fact]
    public async Task AddAsync_ShouldPersistEntity()
    {
        var record = await SeedRecordAsync();
        record.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity()
    {
        var record = await SeedRecordAsync();
        var fetched = await _uow.NpiRecords.GetByIdAsync(record.Id);
        fetched.Should().NotBeNull();
        fetched!.ItemCode.Should().Be("TEST01");
    }

    [Fact]
    public async Task GetByIdAsync_WithIncludes_ShouldEagerLoad()
    {
        var record = await SeedRecordAsync();
        var fetched = await _uow.NpiRecords.GetByIdAsync(record.Id, r => r.SetupQuestions);
        fetched.Should().NotBeNull();
        fetched!.SetupQuestions.Should().HaveCount(2);
    }

    [Fact]
    public async Task FindAsync_WithLinqPredicate_ShouldFilter()
    {
        await SeedRecordAsync("DRAFT01");
        var launched = new NpiRecord
        {
            ItemCode = "LAUNCH01", ProductDescription = "Launched", Status = NpiStatus.Launched, CreatedDate = DateTime.UtcNow
        };
        await _uow.NpiRecords.AddAsync(launched);
        await _uow.SaveChangesAsync();

        var results = await _uow.NpiRecords.FindAsync(r => r.Status == NpiStatus.Launched);
        results.Should().HaveCount(1);
        results[0].ItemCode.Should().Be("LAUNCH01");
    }

    [Fact]
    public async Task FindAsync_WithOrderBy_ShouldSort()
    {
        await SeedRecordAsync("AAA");
        await SeedRecordAsync("ZZZ");

        var results = await _uow.NpiRecords.FindAsync(
            predicate: null,
            orderBy: q => q.OrderByDescending(r => r.ItemCode));

        results.Should().HaveCountGreaterOrEqualTo(2);
        results[0].ItemCode.Should().Be("ZZZ");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        for (int i = 0; i < 15; i++)
            await SeedRecordAsync($"P{i:D3}");

        var (items, total) = await _uow.NpiRecords.GetPagedAsync(
            page: 2, pageSize: 5);

        total.Should().Be(15);
        items.Should().HaveCount(5);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        await SeedRecordAsync("C1");
        await SeedRecordAsync("C2");

        var count = await _uow.NpiRecords.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrueWhenExists()
    {
        await SeedRecordAsync("EXISTS");
        var exists = await _uow.NpiRecords.AnyAsync(r => r.ItemCode == "EXISTS");
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Update_ShouldPersistChanges()
    {
        var record = await SeedRecordAsync();
        record.ProductDescription = "Updated Description";
        _uow.NpiRecords.Update(record);
        await _uow.SaveChangesAsync();

        var fetched = await _uow.NpiRecords.GetByIdAsync(record.Id);
        fetched!.ProductDescription.Should().Be("Updated Description");
    }

    [Fact]
    public async Task Remove_ShouldDeleteEntity()
    {
        var record = await SeedRecordAsync();
        _uow.NpiRecords.Remove(record);
        await _uow.SaveChangesAsync();

        var fetched = await _uow.NpiRecords.GetByIdAsync(record.Id);
        fetched.Should().BeNull();
    }

    // ════════════════════════════════════════════
    // NPI-Specific Repository Tests (LINQ)
    // ════════════════════════════════════════════

    [Fact]
    public async Task GetFullRecordAsync_ShouldIncludeAllChildren()
    {
        var record = await SeedRecordAsync();
        var full = await _uow.NpiRecords.GetFullRecordAsync(record.Id);

        full.Should().NotBeNull();
        full!.SetupQuestions.Should().HaveCount(2);
        full.LaborItems.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldFilterCorrectly()
    {
        await SeedRecordAsync();
        var launched = new NpiRecord
        {
            ItemCode = "L1", ProductDescription = "Launched", Status = NpiStatus.Launched, CreatedDate = DateTime.UtcNow
        };
        await _uow.NpiRecords.AddAsync(launched);
        await _uow.SaveChangesAsync();

        var results = await _uow.NpiRecords.GetByStatusAsync(NpiStatus.Draft);
        results.Should().OnlyContain(r => r.Status == NpiStatus.Draft);
    }

    [Fact]
    public async Task SearchAsync_ShouldMatchItemCodeOrDescription()
    {
        await SeedRecordAsync("FINDME");

        var results = await _uow.NpiRecords.SearchAsync("findme", null, 1, 10);
        results.Should().HaveCount(1);
        results[0].ItemCode.Should().Be("FINDME");
    }

    [Fact]
    public async Task GetTotalLaborCostAsync_ShouldSumByCategoryUsingLinq()
    {
        var record = await SeedRecordAsync();
        var total = await _uow.NpiRecords.GetTotalLaborCostAsync(record.Id, LaborCategory.Formula);

        // 968 + 8456 = 9424
        total.Should().Be(9424m);
    }

    // ════════════════════════════════════════════
    // Unit of Work Transaction Tests
    // ════════════════════════════════════════════

    [Fact]
    public async Task UoW_SaveChanges_ShouldSetModifiedDate()
    {
        var record = await SeedRecordAsync();
        record.ModifiedDate.Should().BeNull();

        record.ProductDescription = "Modified";
        _uow.NpiRecords.Update(record);
        await _uow.SaveChangesAsync();

        record.ModifiedDate.Should().NotBeNull();
    }

    // ════════════════════════════════════════════
    // Child Repository Tests
    // ════════════════════════════════════════════

    [Fact]
    public async Task SetupQuestions_FindAsync_ShouldFilterByNpiRecordId()
    {
        var record = await SeedRecordAsync();
        var questions = await _uow.SetupQuestions.FindAsync(q => q.NpiRecordId == record.Id);
        questions.Should().HaveCount(2);
        questions.Should().Contain(q => q.QuestionKey == "existingBulk");
    }

    [Fact]
    public async Task SetupQuestions_Toggle_ShouldUpdateValue()
    {
        var record = await SeedRecordAsync();
        var question = await _uow.SetupQuestions.FirstOrDefaultAsync(q => q.QuestionKey == "existingBulk" && q.NpiRecordId == record.Id);
        question.Should().NotBeNull();
        question!.Value.Should().BeTrue();

        // Toggle
        var tracked = await _uow.SetupQuestions.GetByIdAsync(question.Id);
        tracked!.Value = false;
        tracked.AnsweredBy = "Tester";
        tracked.AnsweredDate = DateTime.UtcNow;
        _uow.SetupQuestions.Update(tracked);
        await _uow.SaveChangesAsync();

        var updated = await _uow.SetupQuestions.GetByIdAsync(question.Id);
        updated!.Value.Should().BeFalse();
        updated.AnsweredBy.Should().Be("Tester");
    }

    [Fact]
    public async Task Notes_AddAndRetrieve_ShouldWork()
    {
        var record = await SeedRecordAsync();

        var note = new NpiNote
        {
            NpiRecordId = record.Id,
            Content = "Test note content",
            CreatedBy = "Tester",
            CreatedDate = DateTime.UtcNow
        };
        await _uow.Notes.AddAsync(note);
        await _uow.SaveChangesAsync();

        var notes = await _uow.Notes.FindAsync(n => n.NpiRecordId == record.Id);
        notes.Should().HaveCount(1);
        notes[0].Content.Should().Be("Test note content");
    }

    [Fact]
    public async Task ChangeLog_ShouldRecordChanges()
    {
        var record = await SeedRecordAsync();

        await _uow.ChangeLog.AddAsync(new ChangeLogEntry
        {
            NpiRecordId = record.Id,
            FieldChanged = "Status",
            OldValue = "Draft",
            NewValue = "Launched",
            ChangedBy = "Tester",
            ChangedDate = DateTime.UtcNow
        });
        await _uow.SaveChangesAsync();

        var log = await _uow.ChangeLog.FindAsync(
            predicate: c => c.NpiRecordId == record.Id,
            orderBy: q => q.OrderByDescending(c => c.ChangedDate));

        log.Should().HaveCount(1);
        log[0].FieldChanged.Should().Be("Status");
    }
}
