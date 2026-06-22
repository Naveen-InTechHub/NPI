using NPI.Data.Context;
using NPI.Data.Entities;
using NPI.Data.Repositories;

namespace NPI.Data.UnitOfWork;

// ── Interface ──
public interface IUnitOfWork : IDisposable
{
    INpiRecordRepository NpiRecords { get; }
    IRepository<SetupQuestion> SetupQuestions { get; }
    IRepository<PilotRequirement> PilotRequirements { get; }
    IRepository<PlannerQuestion> PlannerQuestions { get; }
    IRepository<QualityPackageQuestion> QualityPackageQuestion { get; }
    IRepository<FGGSSetup> FGSSSetup { get; }
    IRepository<FormulaSpec> FormulaSpecs { get; }
    IRepository<LaborItem> LaborItems { get; }
    IRepository<PackagingComponent> PackagingComponents { get; }
    IRepository<PackagingOption> PackagingOptions { get; }
    IRepository<NpiDocument> Documents { get; }
    IRepository<NpiNote> Notes { get; }
    IRepository<ChangeLogEntry> ChangeLog { get; }
    IRepository<User> Users { get; }
    IRepository<UserRole> UserRoles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task UpsertNote(NpiRecord record, string parent, string? content, string? user);
}

// ── Implementation ──
public class UnitOfWork : IUnitOfWork
{
    private readonly NpiDbContext _context;
    private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _transaction;

    // Lazy-initialized repositories
    private INpiRecordRepository? _npiRecords;
    private IRepository<SetupQuestion>? _setupQuestions;
    private IRepository<PilotRequirement>? _pilotRequirements;
    private IRepository<PlannerQuestion>? _plannerQuestions;
    private IRepository<QualityPackageQuestion>? _qualityPackageQuestion;
    private IRepository<FGGSSetup>? _fgssSetup;
    private IRepository<FormulaSpec>? _formulaSpecs;
    private IRepository<LaborItem>? _laborItems;
    private IRepository<PackagingComponent>? _packagingComponents;
    private IRepository<PackagingOption>? _packagingOptions;
    private IRepository<NpiDocument>? _documents;
    private IRepository<NpiNote>? _notes;
    private IRepository<ChangeLogEntry>? _changeLog;
    private IRepository<User>? _users;
    private IRepository<UserRole>? _userRoles;

    public UnitOfWork(NpiDbContext context)
    {
        _context = context;
    }

    // ── Repository accessors (lazy instantiation) ──

    public INpiRecordRepository NpiRecords
        => _npiRecords ??= new NpiRecordRepository(_context);

    public IRepository<SetupQuestion> SetupQuestions
        => _setupQuestions ??= new Repository<SetupQuestion>(_context);

    public IRepository<PilotRequirement> PilotRequirements
        => _pilotRequirements ??= new Repository<PilotRequirement>(_context);

    public IRepository<PlannerQuestion> PlannerQuestions
        => _plannerQuestions ??= new Repository<PlannerQuestion>(_context);
    public IRepository<FormulaSpec> FormulaSpecs
     => _formulaSpecs ??= new Repository<FormulaSpec>(_context);
    public IRepository<QualityPackageQuestion> QualityPackageQuestion
     => _qualityPackageQuestion ??= new Repository<QualityPackageQuestion>(_context);
    public IRepository<FGGSSetup> FGSSSetup
        => _fgssSetup ??= new Repository<FGGSSetup>(_context);

    public IRepository<LaborItem> LaborItems
        => _laborItems ??= new Repository<LaborItem>(_context);

    public IRepository<PackagingComponent> PackagingComponents
        => _packagingComponents ??= new Repository<PackagingComponent>(_context);

    public IRepository<PackagingOption> PackagingOptions
        => _packagingOptions ??= new Repository<PackagingOption>(_context);

    public IRepository<NpiDocument> Documents
        => _documents ??= new Repository<NpiDocument>(_context);

    public IRepository<NpiNote> Notes
        => _notes ??= new Repository<NpiNote>(_context);

    public IRepository<ChangeLogEntry> ChangeLog
        => _changeLog ??= new Repository<ChangeLogEntry>(_context);

    public IRepository<User> Users
        => _users ??= new Repository<User>(_context);

    public IRepository<UserRole> UserRoles
       => _userRoles ??= new Repository<UserRole>(_context);

    // ── Persistence ──

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-set audit fields
        foreach (var entry in _context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
            {
                entry.Entity.ModifiedDate = DateTime.UtcNow;
            }
        }
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // ── Transaction management ──

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    // ── Dispose ──

    private bool _disposed;

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            _context.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    public async Task UpsertNote(NpiRecord record, string parent, string? content, string? user)
    {
        if (string.IsNullOrWhiteSpace(content))
            return;

        var existingNote = record.Notes?
            .FirstOrDefault(n => n.Parent == parent);

        if (existingNote != null)
        {
            existingNote.Content = content;
        }
        else
        {
            record.Notes ??= new List<NpiNote>();

            record.Notes.Add(new NpiNote
            {
                NpiRecordId = record.Id,
                Parent = parent,
                Content = content,
                CreatedBy = user,
                CreatedDate = DateTime.UtcNow
            });
        }
    }
}
