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

using System;
using Topshelf;
using OPFService.Core;
using Serilog;
using System.Diagnostics;
using System.Net.Http;

namespace OPFService
{
    static class Program
    {
        private static HttpClient _httpClient;
        public static HttpClient OpenPasswordFilterClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }

                return _httpClient;
            }
        }

        static void Main(string[] args)
        {
            if(!EventLog.SourceExists("OpenPasswordFilter"))
            {
                EventLog.CreateEventSource("OpenPasswordFilter", "OpenPasswordFilter");
            }

            var exitCode = HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();

                var loggerConfig = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.EventLog("OpenPasswordFilter")
                    .CreateLogger();
                x.UseSerilog(loggerConfig);

                x.Service<OpenPasswordFilterService>(s =>
                {
                    s.ConstructUsing(opf => new OpenPasswordFilterService());
                    s.WhenStarted(opf => opf.Start());
                    s.WhenStopped(opf => opf.Stop());
                });

                x.SetServiceName("OpenPasswordFilter");
                x.SetDisplayName("Open Password Filter");
                x.SetDescription("Custom service to better protect and control Active Directory domain passwords.");
            });

            Log.CloseAndFlush();
            OpenPasswordFilterClient.Dispose();

            var exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
