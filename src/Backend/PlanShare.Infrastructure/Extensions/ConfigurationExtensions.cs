using Microsoft.Extensions.Configuration;
using PlanShare.Domain.Enums;

namespace PlanShare.Infrastructure.Extensions;
public static class ConfigurationExtensions
{
    public static string ConnectionString(this IConfiguration configuration)
    {
        var databaseType = configuration.GetDataBaseType();

        if (databaseType == DataBaseType.MySql)
        {
            return configuration.GetConnectionString("ConnectionMySql")!;
        }

        return configuration.GetConnectionString("ConnectionSqlServer")!;
    }

    public static DataBaseType GetDataBaseType(this IConfiguration configuration)
    {
        var dbType = configuration.GetConnectionString("DatabaseType");

        return Enum.Parse<DataBaseType>(dbType!);
    }

    public static bool IsUnitTestEnviroment(this IConfiguration configuration)
    {
        _ = bool.TryParse(configuration.GetSection("InMemoryTests").Value, out bool inMemoryTests);

        return inMemoryTests;
    }
}