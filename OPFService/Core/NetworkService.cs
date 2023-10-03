// This file is part of OpenPasswordFilter.
// 
// OpenPasswordFilter is free software; you can redistribute it and / or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// OpenPasswordFilter is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OpenPasswordFilter; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111 - 1307  USA
//

using OPFService.Core.DatabaseAPI;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Topshelf.Logging;

namespace OPFService.Core
{
    public class NetworkService
    {
        private const string MSSQL_PROVIDER = "System.Data.SqlClient";
        private const string MYSQL_PROVIDER = "MySql.Data.MySqlClient";
        private const string TEST_COMMAND = "test";
        private const string HOST = "127.0.0.1";
        private const int PORT = 5999;

        private readonly LogWriter _logger = HostLogger.Get<NetworkService>();
        private readonly OPFDictionary _oPFDictionary;
        private readonly OPFGroup _oPFGroup;

        public NetworkService(OPFDictionary oPFDictionary, OPFGroup oPFGroup)
        {
            _oPFDictionary = oPFDictionary;
            _oPFGroup = oPFGroup;
        }

        public void main(Socket listener)
        {
            IPAddress ip = IPAddress.Parse(HOST);
            IPEndPoint local = new IPEndPoint(ip, PORT);

            try
            {
                listener.Bind(local);
                listener.Listen(64);
                _logger.Info("OpenPasswordFilter is now running.");
                while (true)
                {
                    Socket client = listener.Accept();
                    Handle(client);
                    new Thread(() => Handle(client)).Start();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                _logger.Error("Unable to bind local port");
            }
        }

        public void Handle(Socket client)
        {
            var methodName = $"{nameof(NetworkService)}::{nameof(Handle)}";

            try
            {
                using (var netStream = new NetworkStream(client))
                {
                    using (var streamReader = new StreamReader(netStream))
                    using (var streamWriter = new StreamWriter(netStream))
                    {
                        bool isPasswordBad = false;

                        string command = streamReader.ReadLine();
                        if (command == TEST_COMMAND)
                        {
                            string username = streamReader.ReadLine();
                            string password = streamReader.ReadLine();

                            _logger.Info($"[{methodName}] - Checking password for user with username = {username}");

                            if (_oPFGroup.Contains(username))
                            {
                                isPasswordBad = _oPFDictionary.Contains(password, username);

                                if (Properties.Settings.Default.PwnedPasswordsAPIEnabled && !isPasswordBad)
                                {
                                    var pwnedAPI = new PwnedAPI();
                                    isPasswordBad = pwnedAPI.CheckPassword(password);
                                }

                                if (Properties.Settings.Default.PwnedLocalMySQLDB && !isPasswordBad)
                                {
                                    var dbAPI = new DbAPI(MSSQL_PROVIDER, Properties.Settings.Default.PwnedLocalMSSQLDBConnString);
                                    isPasswordBad = dbAPI.CheckPassword(password);
                                }
                                else if (Properties.Settings.Default.PwnedLocalMySQLDB && !isPasswordBad)
                                {
                                    var dbAPI = new DbAPI(MYSQL_PROVIDER, Properties.Settings.Default.PwnedLocalMySQLDBConnString);
                                    isPasswordBad = dbAPI.CheckPassword(password);
                                }
                            }

                            _logger.Info($"[{methodName}] - Password checking finished successfully. " +
                                $"Is password bad = {isPasswordBad}");

                            streamWriter.WriteLine(isPasswordBad ? "false" : "true");
                            streamWriter.Flush();
                        }
                        else
                        {
                            _logger.Info($"[{methodName}] - Given command is unknown. Command = {command}");

                            streamWriter.WriteLine("ERROR");
                            streamWriter.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[{methodName}] - Unexpected exception. Error = {ex.Message}");
            }
            finally
            {
                _logger.Info($"[{methodName}] - Closing current socket. {client.LocalEndPoint}");
                client.Close();
            }
        }
    }
}
