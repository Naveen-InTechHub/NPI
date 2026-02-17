using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NPI.Data.Context;
using NPI.Data.Repositories;
using NPI.Data.UnitOfWork;

namespace NPI.Data;

public static class DataServiceRegistration
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<NpiDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<INpiRecordRepository, NpiRecordRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        return services;
    }
}
