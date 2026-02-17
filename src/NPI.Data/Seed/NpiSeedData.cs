using NPI.Data.Context;
using NPI.Data.Entities;
using NPI.Shared.Enums;

namespace NPI.Data.Seed;

public static class NpiSeedData
{
    public static async Task SeedAsync(NpiDbContext context)
    {
        if (context.NpiRecords.Any()) return;

        var records = new List<NpiRecord>
        {
            new()
            {
                ItemCode = "FGTEST",
                ProductDescription = "Daily EU 439EURL Tablet",
                QuoteNumber = "6514",
                Status = NpiStatus.Launched,
                CustomerId = "50415", 
                CustomerName = "Aloe Vera of America",
                SalesRep1 = "A. Mattheus",
                SalesRep2 = "C. Axtell",
                BulkCode = "BTEST",
                FGCode = "FGTEST",
                PackoutDescription = "PS.2 Daily EU 439EURL – 60 CT",
                ProductType = ProductType.Tablet,
                PackageType = PackageType.Bottles,
                OrderQty = 200_000m,
                TotalUnits = 30_000m,
                FormulaSize = 12_000_000m,
                Servings = 6_000_000m,
                UnitPrice = 2.80m,
                StdQty = 200_000m,
                LeadTimeDays = 65,
                POValue = 560_000m,
                ItemDescription = "Daily EU 439EURL tablet",
                CreatedBy = "Candice A.",
                CreatedDate = new DateTime(2025, 2, 5, 11, 20, 0, DateTimeKind.Utc),
                SetupQuestions = new List<SetupQuestion>
                {
                    new() { QuestionKey = "existingBulk", QuestionText = "Does this new product use an existing bulk?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "multiplePacks", QuestionText = "Will this bulk be used in multiple packouts?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,8,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "bulkFinished", QuestionText = "Will this item be sold as a Bulk Finished Good?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                },
                LaborItems = new List<LaborItem>
                {
                    new() { Category = LaborCategory.Formula, Description = "Weigh-Up", Rate = 37.89m, Cost = 843m, SortOrder = 1 },
                    new() { Category = LaborCategory.Formula, Description = "Mixing", Rate = 37.89m, Cost = 968m, SortOrder = 2 },
                    new() { Category = LaborCategory.Packout, Description = "Bottle Labor", Rate = 37.89m, Cost = 18_653m, SortOrder = 3 },
                }
            },
            new()
            {
                ItemCode = "VIT-C500",
                ProductDescription = "Vitamin C 500mg Capsules",
                QuoteNumber = "6520",
                Status = NpiStatus.Draft,
                CustomerId = "50420",
                CustomerName = "Health Plus Inc.",
                SalesRep1 = "J. Smith",
                ProductType = ProductType.Capsule,
                PackageType = PackageType.Bottles,
                OrderQty = 100_000m,
                TotalUnits = 20_000m,
                UnitPrice = 1.50m,
                POValue = 150_000m,
                CreatedBy = "Mike T.",
                CreatedDate = new DateTime(2025, 2, 8, 9, 0, 0, DateTimeKind.Utc),
                 SetupQuestions = new List<SetupQuestion>
                {
                    new() { QuestionKey = "existingBulk", QuestionText = "Does this new product use an existing bulk?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "multiplePacks", QuestionText = "Will this bulk be used in multiple packouts?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,8,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "bulkFinished", QuestionText = "Will this item be sold as a Bulk Finished Good?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                },
                LaborItems = new List<LaborItem>
                {
                    new() { Category = LaborCategory.Formula, Description = "Weigh-Up", Rate = 37.89m, Cost = 843m, SortOrder = 1 },
                    new() { Category = LaborCategory.Formula, Description = "Mixing", Rate = 37.89m, Cost = 968m, SortOrder = 2 },
                    new() { Category = LaborCategory.Packout, Description = "Bottle Labor", Rate = 37.89m, Cost = 18_653m, SortOrder = 3 },
                }
            },
            new()
            {
                ItemCode = "OMG3-1000",
                ProductDescription = "Omega-3 Fish Oil 1000mg Softgel",
                QuoteNumber = "6525",
                Status = NpiStatus.InProgress,
                CustomerId = "50425",
                CustomerName = "NutraLife Labs",
                SalesRep1 = "R. Johnson",
                SalesRep2 = "L. Davis",
                ProductType = ProductType.Softgel,
                PackageType = PackageType.Bottles,
                OrderQty = 150_000m,
                TotalUnits = 25_000m,
                UnitPrice = 3.25m,
                POValue = 487_500m,
                CreatedBy = "Sarah K.",
                CreatedDate = new DateTime(2025, 2, 6, 14, 30, 0, DateTimeKind.Utc),
                ModifiedDate = new DateTime(2025, 2, 9, 10, 15, 0, DateTimeKind.Utc),
                 SetupQuestions = new List<SetupQuestion>
                {
                    new() { QuestionKey = "existingBulk", QuestionText = "Does this new product use an existing bulk?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "multiplePacks", QuestionText = "Will this bulk be used in multiple packouts?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,8,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "bulkFinished", QuestionText = "Will this item be sold as a Bulk Finished Good?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                },
                LaborItems = new List<LaborItem>
                {
                    new() { Category = LaborCategory.Formula, Description = "Weigh-Up", Rate = 37.89m, Cost = 843m, SortOrder = 1 },
                    new() { Category = LaborCategory.Formula, Description = "Mixing", Rate = 37.89m, Cost = 968m, SortOrder = 2 },
                    new() { Category = LaborCategory.Packout, Description = "Bottle Labor", Rate = 37.89m, Cost = 18_653m, SortOrder = 3 },
                }
            },
            new()
            {
                ItemCode = "PRO-WHEY",
                ProductDescription = "Whey Protein Powder - Chocolate",
                QuoteNumber = "6530",
                Status = NpiStatus.Launched,
                CustomerId = "50430",
                CustomerName = "FitBody Nutrition",
                SalesRep1 = "T. Williams",
                ProductType = ProductType.Powder,
                PackageType = PackageType.Pouches,
                OrderQty = 50_000m,
                TotalUnits = 50_000m,
                UnitPrice = 12.00m,
                POValue = 600_000m,
                CreatedBy = "Alex P.",
                CreatedDate = new DateTime(2025, 1, 20, 8, 45, 0, DateTimeKind.Utc),
                 SetupQuestions = new List<SetupQuestion>
                {
                    new() { QuestionKey = "existingBulk", QuestionText = "Does this new product use an existing bulk?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "multiplePacks", QuestionText = "Will this bulk be used in multiple packouts?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,8,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "bulkFinished", QuestionText = "Will this item be sold as a Bulk Finished Good?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                },
                LaborItems = new List<LaborItem>
                {
                    new() { Category = LaborCategory.Formula, Description = "Weigh-Up", Rate = 37.89m, Cost = 843m, SortOrder = 1 },
                    new() { Category = LaborCategory.Formula, Description = "Mixing", Rate = 37.89m, Cost = 968m, SortOrder = 2 },
                    new() { Category = LaborCategory.Packout, Description = "Bottle Labor", Rate = 37.89m, Cost = 18_653m, SortOrder = 3 },
                }
            },
            new()
            {
                ItemCode = "MULTI-ADT",
                ProductDescription = "Adult Multivitamin Gummies",
                QuoteNumber = "6535",
                Status = NpiStatus.Draft,
                CustomerId = "50435",
                CustomerName = "Gummy Health Co.",
                SalesRep1 = "M. Brown",
                ProductType = ProductType.Gummy,
                PackageType = PackageType.Bottles,
                OrderQty = 75_000m,
                TotalUnits = 15_000m,
                UnitPrice = 4.50m,
                POValue = 337_500m,
                CreatedBy = "Emily R.",
                CreatedDate = new DateTime(2025, 2, 10, 11, 0, 0, DateTimeKind.Utc),
                 SetupQuestions = new List<SetupQuestion>
                {
                    new() { QuestionKey = "existingBulk", QuestionText = "Does this new product use an existing bulk?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "multiplePacks", QuestionText = "Will this bulk be used in multiple packouts?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,8,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "bulkFinished", QuestionText = "Will this item be sold as a Bulk Finished Good?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                },
                LaborItems = new List<LaborItem>
                {
                    new() { Category = LaborCategory.Formula, Description = "Weigh-Up", Rate = 67.39m, Cost = 843m, SortOrder = 1 },
                    new() { Category = LaborCategory.Formula, Description = "Mixing", Rate = 67.19m, Cost = 968m, SortOrder = 2 },
                    new() { Category = LaborCategory.Packout, Description = "Bottle Labor", Rate = 57.09m, Cost = 18_653m, SortOrder = 3 },
                }
            },
            new()
            {
                ItemCode = "PROB-50B",
                ProductDescription = "Probiotic 50 Billion CFU",
                QuoteNumber = "6540",
                Status = NpiStatus.InProgress,
                CustomerId = "50440",
                CustomerName = "Gut Wellness LLC",
                SalesRep1 = "D. Martinez",
                ProductType = ProductType.Capsule,
                PackageType = PackageType.Bottles,
                OrderQty = 80_000m,
                TotalUnits = 16_000m,
                UnitPrice = 8.75m,
                POValue = 700_000m,
                CreatedBy = "Chris L.",
                CreatedDate = new DateTime(2025, 2, 3, 15, 20, 0, DateTimeKind.Utc),
                 SetupQuestions = new List<SetupQuestion>
                {
                    new() { QuestionKey = "existingBulk", QuestionText = "Does this new product use an existing bulk?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "multiplePacks", QuestionText = "Will this bulk be used in multiple packouts?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,8,0, DateTimeKind.Utc) },
                    new() { QuestionKey = "bulkFinished", QuestionText = "Will this item be sold as a Bulk Finished Good?", Value = true, AnsweredBy = "CA", AnsweredDate = new DateTime(2025,2,5,13,7,0, DateTimeKind.Utc) },
                },
                LaborItems = new List<LaborItem>
                {
                    new() { Category = LaborCategory.Formula, Description = "Weigh-Up", Rate = 27.89m, Cost = 843m, SortOrder = 1 },
                    new() { Category = LaborCategory.Formula, Description = "Mixing", Rate = 17.89m, Cost = 968m, SortOrder = 2 },
                    new() { Category = LaborCategory.Packout, Description = "Bottle Labor", Rate = 57.89m, Cost = 18_653m, SortOrder = 3 },
                }
            },
        };

        await context.NpiRecords.AddRangeAsync(records);
        await context.SaveChangesAsync();
    }

    public static async Task SeedRolesAsync(NpiDbContext context)
    {
        //if (context.Roles.Any()) return;
        //var roles = new List<Role>
        //{
        //    new() { Name = "Admin" },
        //    new() { Name = "Sales" },
        //    new() { Name = "Planner" },
        //    new() { Name = "Formula" },
        //    new() { Name = "Viewer" }
        //};
        //await context.Roles.AddRangeAsync(roles);
        //await context.SaveChangesAsync();
    }


}
