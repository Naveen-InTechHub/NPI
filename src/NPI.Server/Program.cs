using Microsoft.Extensions.FileProviders;
using NPI.Data;
using NPI.Data.Context;
using NPI.Data.Seed;
using NPI.Data.Service;
using NPI.Data.UnitOfWork;
using NPI.Server.Mapping;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Data layer (SQLite + Repository + UnitOfWork)
var connectionString = builder.Configuration.GetConnectionString("NpiDb")
    ?? "Data Source=npi.db";
builder.Services.AddDataServices(connectionString);

// AutoMapper (13.x uses assembly scanning directly)
builder.Services.AddAutoMapper(typeof(NpiMappingProfile).Assembly);

// CORS for Blazor WASM client
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
        policy.WithOrigins("http://localhost:7100", "http://localhost:5100")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INpiDocumentService, NpiDocumentService>();


var app = builder.Build();

// ── Ensure DB created + seed ──
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NpiDbContext>();
    db.Database.EnsureCreated();
    await NpiSeedData.SeedAsync(db);
    await NpiSeedData.SeedRolesAsync(db);
}


// ── Pipeline ──
app.UseHttpsRedirection();
app.UseStaticFiles();
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});
app.UseCors("BlazorClient");
app.UseAuthorization();
app.MapControllers();

app.Run();
