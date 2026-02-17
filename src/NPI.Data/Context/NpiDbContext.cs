using Microsoft.EntityFrameworkCore;
using NPI.Data.Entities;

namespace NPI.Data.Context;

public class NpiDbContext : DbContext
{
    public NpiDbContext(DbContextOptions<NpiDbContext> options) : base(options) { }

    public DbSet<NpiRecord> NpiRecords => Set<NpiRecord>();
    public DbSet<SetupQuestion> SetupQuestions => Set<SetupQuestion>();
    public DbSet<PilotRequirement> PilotRequirements => Set<PilotRequirement>();
    public DbSet<PlannerQuestion> PlannerQuestions => Set<PlannerQuestion>();
    public DbSet<FormulaSpec> FormulaSpecs => Set<FormulaSpec>();
    public DbSet<LaborItem> LaborItems => Set<LaborItem>();
    public DbSet<PackagingComponent> PackagingComponents => Set<PackagingComponent>();
    public DbSet<PackagingOption> PackagingOptions => Set<PackagingOption>();
    public DbSet<NpiDocument> Documents => Set<NpiDocument>();
    public DbSet<NpiNote> Notes => Set<NpiNote>();
    public DbSet<ChangeLogEntry> ChangeLogEntries => Set<ChangeLogEntry>();

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── NpiRecord ──
        modelBuilder.Entity<NpiRecord>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ItemCode).IsRequired().HasMaxLength(50);
            e.Property(x => x.ProductDescription).IsRequired().HasMaxLength(200);
            e.Property(x => x.QuoteNumber).HasMaxLength(20);
            e.Property(x => x.CustomerId).HasMaxLength(20);
            e.Property(x => x.CustomerName).HasMaxLength(200);
            e.Property(x => x.SalesRep1).HasMaxLength(100);
            e.Property(x => x.SalesRep2).HasMaxLength(100);
            e.Property(x => x.BulkCode).HasMaxLength(50);
            e.Property(x => x.FGCode).HasMaxLength(50);
            e.Property(x => x.PackoutDescription).HasMaxLength(200);
            e.Property(x => x.ItemDescription).HasMaxLength(200);
            e.Property(x => x.CustomerNumber).HasMaxLength(50);
            e.Property(x => x.Barcode).HasMaxLength(50);
            e.Property(x => x.OrderQty).HasPrecision(18, 2);
            e.Property(x => x.TotalUnits).HasPrecision(18, 2);
            e.Property(x => x.FormulaSize).HasPrecision(18, 2);
            e.Property(x => x.Servings).HasPrecision(18, 2);
            e.Property(x => x.UnitPrice).HasPrecision(18, 4);
            e.Property(x => x.StdQty).HasPrecision(18, 2);
            e.Property(x => x.POValue).HasPrecision(18, 2);
            e.Property(x => x.CreatedBy).HasMaxLength(100);
            e.Property(x => x.ModifiedBy).HasMaxLength(100);

            e.HasIndex(x => x.ItemCode);
            e.HasIndex(x => x.QuoteNumber);
            e.HasIndex(x => x.Status);
        });

        // ── SetupQuestion ──
        modelBuilder.Entity<SetupQuestion>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.QuestionKey).IsRequired().HasMaxLength(50);
            e.Property(x => x.QuestionText).IsRequired().HasMaxLength(300);
            e.Property(x => x.AnsweredBy).HasMaxLength(100);
            e.HasOne(x => x.NpiRecord).WithMany(r => r.SetupQuestions).HasForeignKey(x => x.NpiRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── PilotRequirement ──
        modelBuilder.Entity<PilotRequirement>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.QuestionKey).IsRequired().HasMaxLength(50);
            e.Property(x => x.QuestionText).IsRequired().HasMaxLength(300);
            e.Property(x => x.AnsweredBy).HasMaxLength(100);
            e.HasOne(x => x.NpiRecord).WithMany(r => r.PilotRequirements).HasForeignKey(x => x.NpiRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── PlannerQuestion ──
        modelBuilder.Entity<PlannerQuestion>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.QuestionKey).IsRequired().HasMaxLength(50);
            e.Property(x => x.QuestionText).IsRequired().HasMaxLength(300);
            e.Property(x => x.AnsweredBy).HasMaxLength(100);
            e.HasOne(x => x.NpiRecord).WithMany(r => r.PlannerQuestions).HasForeignKey(x => x.NpiRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── FormulaSpec ── (1:1)
        modelBuilder.Entity<FormulaSpec>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FormulaName).IsRequired().HasMaxLength(200);
            e.Property(x => x.OrderQty).HasPrecision(18, 2);
            e.Property(x => x.TotalUnits).HasPrecision(18, 2);
            e.Property(x => x.ServPerUnit).HasPrecision(18, 2);
            e.Property(x => x.FormulaSize).HasPrecision(18, 2);
            e.Property(x => x.TotalServings).HasPrecision(18, 2);
            e.Property(x => x.KG).HasPrecision(18, 3);
            e.Property(x => x.Density).HasPrecision(10, 2);
            e.HasOne(x => x.NpiRecord).WithOne(r => r.FormulaSpec).HasForeignKey<FormulaSpec>(x => x.NpiRecordId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.NpiRecordId).IsUnique();
        });

        // ── LaborItem ──
        modelBuilder.Entity<LaborItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Description).IsRequired().HasMaxLength(200);
            e.Property(x => x.Rate).HasPrecision(18, 4);
            e.Property(x => x.Cost).HasPrecision(18, 2);
            e.Property(x => x.CostPerEach).HasPrecision(18, 4);
            e.HasOne(x => x.NpiRecord).WithMany(r => r.LaborItems).HasForeignKey(x => x.NpiRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── PackagingComponent ──
        modelBuilder.Entity<PackagingComponent>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ItemCode).IsRequired().HasMaxLength(50);
            e.Property(x => x.Qty).HasPrecision(18, 6);
            e.HasOne(x => x.NpiRecord).WithMany(r => r.PackagingComponents).HasForeignKey(x => x.NpiRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── PackagingOption ──
        modelBuilder.Entity<PackagingOption>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.OptionName).IsRequired().HasMaxLength(100);
            e.HasOne(x => x.NpiRecord).WithMany(r => r.PackagingOptions).HasForeignKey(x => x.NpiRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── NpiDocument ──
        modelBuilder.Entity<NpiDocument>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FileName).HasMaxLength(200);
            e.Property(x => x.FilePath).HasMaxLength(500);
            e.HasOne(x => x.NpiRecord).WithMany(r => r.Documents).HasForeignKey(x => x.NpiRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── NpiNote ──
        modelBuilder.Entity<NpiNote>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Content).IsRequired();
            e.HasOne(x => x.NpiRecord).WithMany(r => r.Notes).HasForeignKey(x => x.NpiRecordId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── ChangeLogEntry ──
        modelBuilder.Entity<ChangeLogEntry>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FieldChanged).IsRequired().HasMaxLength(100);
            e.HasOne(x => x.NpiRecord).WithMany(r => r.ChangeLog).HasForeignKey(x => x.NpiRecordId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.NpiRecordId, x.ChangedDate });
        });

        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
             new Role { Id = 1, Name = "SuperAdmin" },
             new Role { Id = 2, Name = "NPIManager" },
             new Role { Id = 3, Name = "Planner" },
             new Role { Id = 4, Name = "QualityOfficer" },
             new Role { Id = 5, Name = "PackagingExec" }
         );


        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, UserName = "superadmin", Password = "123456", Email = "superadmin@ans.com" },
            new User { Id = 2, UserName = "npimanager", Password = "123456", Email = "npi.manager@ans.com" },
            new User { Id = 3, UserName = "planner", Password = "123456", Email = "planner@ans.com" },
            new User { Id = 4, UserName = "quality", Password = "123456", Email = "quality@ans.com" },
            new User { Id = 5, UserName = "packaging", Password = "123456", Email = "packaging@ans.com" }
        );

        // Seed UserRoles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = 1, UserId = 1, RoleId = 1 }, // superadmin -> SuperAdmin
            new UserRole { Id = 2, UserId = 2, RoleId = 2 }, // npimanager -> NPIManager
            new UserRole { Id = 3, UserId = 3, RoleId = 3 }, // planner -> Planner
            new UserRole { Id = 4, UserId = 4, RoleId = 4 }, // quality -> QualityOfficer
            new UserRole { Id = 5, UserId = 5, RoleId = 5 }  // packaging -> PackagingExec
        );
           

    }
}
