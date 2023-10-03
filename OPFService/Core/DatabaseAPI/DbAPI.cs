using System;
using System.Data.Common;
using Topshelf.Logging;

namespace OPFService.Core.DatabaseAPI
{
    public class DbAPI
    {
        private const string GET_PASSWORD_QUERY = "SELECT * FROM Passwordlist WHERE Passwords='{0}'";

        private readonly LogWriter _logger = HostLogger.Get<DbAPI>();
        private readonly DbProviderFactory _dbProviderFactory;

        private readonly string _connectionString;

        public DbAPI(string providerName, string connectionString)
        {
            _dbProviderFactory = DbProviderFactories.GetFactory(providerName);
            _connectionString = connectionString;
        }

        public bool CheckPassword(string password)
        {
            var methodName = $"{nameof(DbAPI)}::{nameof(CheckPassword)}";

            var query = string.Format(GET_PASSWORD_QUERY, password);
            try
            {
                using (var dbConnection = CreateDbConnection(_connectionString))
                {
                    dbConnection.Open();

                    using (var command = CreateDbCommand(dbConnection, query))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var isPasswordForbidden = reader.HasRows;

                            _logger.Info($"[{methodName}] - Given password was checked successfully. " +
                                $"Is Password frobidden = {isPasswordForbidden}");

                            return isPasswordForbidden;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[{methodName}] - Error = {ex.Message}");

                return true;
            }
        }

        private DbConnection CreateDbConnection(string connectionString)
        {
            var methodName = $"{nameof(DbAPI)}::{nameof(CreateDbConnection)}";

            try
            {
                var dbConnection = _dbProviderFactory.CreateConnection();
                dbConnection.ConnectionString = connectionString;

                _logger.Debug($"[{methodName}] - Db connection is successfully created. " +
                    $"Connection String = {dbConnection.ConnectionString}");

                return dbConnection;
            }
            catch (Exception ex)
            {
                _logger.Error($"[{methodName}] - Error = {ex.Message}");

                throw;
            }
        }

        private DbCommand CreateDbCommand(DbConnection dbConnection, string commandText)
        {
            var methodName = $"{nameof(DbAPI)}::{nameof(CreateDbCommand)}";

            try
            {
                var dbCommand = _dbProviderFactory.CreateCommand();
                dbCommand.Connection = dbConnection;
                dbCommand.CommandText = commandText;

                _logger.Debug($"[{methodName}] - Db command is created successfully. " +
                    $"Command text = {dbCommand.CommandText}");

                return dbCommand;
            }
            catch (Exception ex)
            {
                _logger.Error($"[{methodName}] - Error = {ex.Message}");

                throw;
            }
        }
    }
}
