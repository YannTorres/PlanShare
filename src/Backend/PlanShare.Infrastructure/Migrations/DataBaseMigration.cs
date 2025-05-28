using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using PlanShare.Domain.Enums;
using PlanShare.Infrastructure.DataAccess;

namespace PlanShare.Infrastructure.Migrations;
public static class DataBaseMigration
{
    public static void Migrate(DataBaseType dataBaseType, string connectionString, IServiceProvider serviceProvider)
    {
        if (dataBaseType == DataBaseType.SqlServer)
            EnsureDatabaseCreatedForSQLServer(connectionString);
        else 
            EnsureDatabaseCreatedForMySql(connectionString);

        MigrateDataBase(serviceProvider);
    }

    private static void EnsureDatabaseCreatedForSQLServer(string connectionString)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

        var databaseName = connectionStringBuilder.InitialCatalog;

        connectionStringBuilder.Remove("Initial Catalog");

        // Precisamos remover o Database, pois se for especificado e ele já existir acontece uma exceção na proxima linha.
        using var dbConnnection = new SqlConnection(connectionStringBuilder.ConnectionString);

        var parameters = new DynamicParameters();
        parameters.Add("name", databaseName);

        var records = dbConnnection.Query("SELECT * FROM sys.databases WHERE name = @name", parameters);

        if (records.Any() == false)
            dbConnnection.Execute($@"CREATE DATABASE {databaseName}");
    }

    private static void EnsureDatabaseCreatedForMySql(string connectionString)
    {
        var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);

        var databaseName = connectionStringBuilder.Database;

        connectionStringBuilder.Remove("Database");

        // Precisamos remover o Database, pois se for especificado e ele já existir acontece uma exceção na proxima linha.
        using var dbConnnection = new MySqlConnection(connectionStringBuilder.ConnectionString);

        dbConnnection.Execute($@"CREATE DATABASE IF NOT EXISTS {databaseName}");
    }

    private static void MigrateDataBase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        runner.ListMigrations();

        runner.MigrateUp();
    }
}
