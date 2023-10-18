using System;
using System.Text;
using Topshelf.Logging;
using static OPFService.Core.NetworkService;

namespace OPFService.Core;

public sealed class OpenPasswordFilterService
{
    private readonly LogWriter logger = HostLogger.Get<OpenPasswordFilterService>();
    private readonly NetworkService networkService;
    private readonly DictionaryService dictionaryService;
    private readonly DirectoryService directoryService;
    private readonly PwnedService pwnedService;
    private readonly Lazy<DatabaseService> sqlService;
    private readonly Lazy<DatabaseService> mysqlService;

    public OpenPasswordFilterService()
    {
        this.logger.Debug("Initializing Open Password Filter Service started");

        this.networkService = new();
        this.networkService.ClientDataReceived += ClientDataReceived;

        string OPFMatchPath = Properties.Settings.Default.OPFMatchPath;
        this.logger.Debug($"Path for text file containing forbidden passwords is retrieved. Path = {OPFMatchPath}");

        string OPFContPath = Properties.Settings.Default.OPFContPath;
        this.logger.Debug($"Path for text file containing forbidden substrings is retrieved. Path = {OPFContPath}");

        string OPFRegexPath = Properties.Settings.Default.OPFRegexPath;
        this.logger.Debug($"Path for text file containing forbidden password patterns is retrieved. Path = {OPFRegexPath}");

        string OPFGroupPath = Properties.Settings.Default.OPFGroupPath;
        this.logger.Debug($"Path for text file containing Active Directory groups to check is retrieved. Path = {OPFGroupPath}");

        this.dictionaryService = new(OPFMatchPath, OPFContPath, OPFRegexPath);
        this.directoryService = new(OPFGroupPath);
        this.pwnedService = new();
        this.sqlService = new(() => new DatabaseService("System.Data.SqlClient", Properties.Settings.Default.PwnedLocalMSSQLDBConnString));
        this.mysqlService = new(() => new DatabaseService("MySql.Data.MySqlClient", Properties.Settings.Default.PwnedLocalMySQLDBConnString));

        this.logger.Debug("Initializing Open Password Filter Service finished");
    }

    public void Start()
    {
        this.networkService.StartServer(Properties.Settings.Default.Port);
    }

    public void Stop()
    {
        this.networkService.StopServer();
    }

    private void ClientDataReceived(object sender, ClientDataReceivedEventArgs e)
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(ClientDataReceived)}";
        this.logger.Debug($"[{methodName}] - Client {e.ConnectionId} {e.IpAddress} message {e.Data.Length} received");

        string message = Encoding.UTF8.GetString(e.Data);
        string ipAddress = e.IpAddress;

        try
        {
            var messages = message.Split('\n');
            var command = messages[0];

            if (!string.Equals(command, "test"))
            {
                return;
            }

            var username = messages[1];
            var password = messages[2];

            var isPasswordBad = CheckPassword(username, password, ipAddress);
            this.networkService.SendString(e.ConnectionId, isPasswordBad ? "false" : "true");
        }
        catch (Exception ex)
        {
            this.logger.Error($"[{methodName}] - Unexpected exception. Error = {ex.Message}", ex);
        }
    }

    private bool CheckPassword(string username, string password, string ipAddress)
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(CheckPassword)}";

        this.logger.Info($"[{methodName}] - Checking password for user with username = {username} and client ip = {ipAddress}");

        bool isPasswordBad = false;
        string resultInfo = string.Empty;

        try
        {
            if (this.directoryService.ContainsInGroup(username))
            {
                if (Properties.Settings.Default.OPFMatchPathEnabled
                    && (isPasswordBad = this.dictionaryService.IsPasswordForbidden(password)))
                {
                    resultInfo = $"the forbidden passwords file {Properties.Settings.Default.OPFMatchPath}";
                }

                if (Properties.Settings.Default.OPFContPathEnabled
                    && !isPasswordBad
                    && (isPasswordBad = this.dictionaryService.DoesPasswordHaveForbiddenSubstring(password)))
                {
                    resultInfo = $"the forbidden substrings file {Properties.Settings.Default.OPFContPath}";
                }

                if (Properties.Settings.Default.OPFRegexPathEnabled
                    && !isPasswordBad
                    && (isPasswordBad = this.dictionaryService.DoesPasswordMatchForbiddenPattern(password)))
                {
                    resultInfo = $"forbidden pattern match file {Properties.Settings.Default.OPFRegexPath}";
                }

                if (Properties.Settings.Default.OPFActiveDirectoryEnabled
                    && !isPasswordBad
                    && (isPasswordBad = this.directoryService.DoesPasswordHaveUserInfo(username, password)))
                {
                    resultInfo = $"Active Directory (password contains user information)";
                }

                if (Properties.Settings.Default.PwnedPasswordsAPIEnabled
                    && !isPasswordBad
                    && (isPasswordBad = this.pwnedService.CheckPassword(password)))
                {
                    resultInfo = $"breach corpuses at haveibeenpwned.com";
                }

                if (Properties.Settings.Default.PwnedLocalMSSQLDB
                    && !isPasswordBad
                    && (isPasswordBad = this.sqlService.Value.CheckPassword(password)))
                {
                    resultInfo = $"breach corpuses at haveibeenpwned.com on local MSSQL database";
                }
                else if (Properties.Settings.Default.PwnedLocalMySQLDB
                    && !isPasswordBad
                    && (isPasswordBad = this.mysqlService.Value.CheckPassword(password)))
                {
                    resultInfo = $"reach corpuses at haveibeenpwned.com on local MySQL database";
                }
            }

            if (resultInfo is "")
            {
                this.logger.Info($"Password OK!"
                 + $" Username = {username}"
                 + $" ClientIP = {ipAddress}");
            }
            else
            {
                this.logger.Info($"Password NOT OK! Information about password found in {resultInfo}."
                 + $" Username = {username}"
                 + $" ClientIP = {ipAddress}");
            }
        }
        catch (Exception ex)
        {
            this.logger.Error($"[{methodName}] - Unexpected exception. Error = {ex.Message}", ex);
        }

        return isPasswordBad;
    }
}
