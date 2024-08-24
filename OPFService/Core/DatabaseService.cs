using OPFService.Utilities;
using System;
using System.Data.Common;
using Topshelf.Logging;

namespace OPFService.Core;

public sealed class DatabaseService
{
    private readonly LogWriter logger = HostLogger.Get<DatabaseService>();
    private readonly DbProviderFactory dbProviderFactory;
    private readonly string connectionString;

    public DatabaseService(string providerName, string connectionString)
    {
        this.dbProviderFactory = DbProviderFactories.GetFactory(providerName);
        this.connectionString = connectionString;
    }

    public bool CheckPassword(string password, bool byHash)
    {
        var methodName = $"{nameof(DatabaseService)}::{nameof(CheckPassword)}";
        // bool isPasswordForbidden = false;

        // Passwortcheck in der DB      
        try
        {
            // Abfrage in Klartext
            var query = $"SELECT * FROM Passwordlist WHERE Passwords='{password}'";
            using var dbConnection = CreateDbConnection(connectionString);
            dbConnection.Open();

            using var command = CreateDbCommand(dbConnection, query);
            using var reader = command.ExecuteReader();
            isPasswordForbidden = reader.HasRows;

            // Wenn das Passwort in der ersten Abfrage nicht gefunden wird, prüfe in der Hashtabelle
            if (!isPasswordForbidden && byHash)
            {
                // Abfrage in der Tabelle Passwordhashlist
                var passwordHash = StringUtilities.GetPasswordHash(password);
                query = $"SELECT * FROM Passwordhashlist WHERE Hashes='{passwordHash}%'";

                using var hashCommand = CreateDbCommand(dbConnection, query);
                using var hashReader = hashCommand.ExecuteReader();
                isPasswordForbidden = hashReader.HasRows;
            }

            this.logger.Debug($"[{methodName}] - Given password was checked successfully. " +
                $"Is Password forbidden = {isPasswordForbidden}");

            return isPasswordForbidden;
        }
        catch (Exception ex)
        {
            this.logger.Error($"[{methodName}] - Error = {ex.Message}", ex);

            return true;
        }
    }

    private DbConnection CreateDbConnection(string connectionString)
    {
        var methodName = $"{nameof(DatabaseService)}::{nameof(CreateDbConnection)}";

        try
        {
            var dbConnection = this.dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = connectionString;

            logger.Debug($"[{methodName}] - Db connection is successfully created. " +
                $"Connection String = {dbConnection.ConnectionString}");

            return dbConnection;
        }
        catch (Exception ex)
        {
            logger.Error($"[{methodName}] - Error = {ex.Message}", ex);

            throw;
        }
    }

    private DbCommand CreateDbCommand(DbConnection dbConnection, string commandText)
    {
        var methodName = $"{nameof(DatabaseService)}::{nameof(CreateDbCommand)}";

        try
        {
            var dbCommand = dbProviderFactory.CreateCommand();
            dbCommand.Connection = dbConnection;
            dbCommand.CommandText = commandText;

            logger.Debug($"[{methodName}] - Db command is created successfully. " +
                $"Command text = {dbCommand.CommandText}");

            return dbCommand;
        }
        catch (Exception ex)
        {
            logger.Error($"[{methodName}] - Error = {ex.Message}", ex);

            throw;
        }
    }
}
