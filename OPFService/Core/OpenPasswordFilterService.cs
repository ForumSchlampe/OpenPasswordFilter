using System.Net.Sockets;
using System.Threading;
using Topshelf.Logging;

namespace OPFService.Core
{
    public class OpenPasswordFilterService
    {
        private readonly LogWriter _logger = HostLogger.Get<OpenPasswordFilterService>();
        private Thread _worker;
        private Socket _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public OpenPasswordFilterService()
        {
            _logger.Debug("Initializing Open Password Filter Service started");

            string OPFMatchPath = Properties.Settings.Default.OPFMatchPath;
            _logger.Debug($"Path for text file containing " +
                $"forbidden passwords is retrieved. Path = {OPFMatchPath}");

            string OPFContPath = Properties.Settings.Default.OPFContPath;
            _logger.Debug($"Path for text file containing " +
                $"forbidden substrings is retrieved. Path = {OPFContPath}");

            string OPFRegexPath = Properties.Settings.Default.OPFRegexPath;
            _logger.Debug($"Path for text file containing " +
                $"forbidden password patterns is retrieved. Path = {OPFRegexPath}");

            string OPFGroupPath = Properties.Settings.Default.OPFRegexPath;
            _logger.Debug($"Path for text file containing " +
                $"Active Directory groups to check is retrieved. Path = {OPFGroupPath}");

            var oPFDictionary = new OPFDictionary(
                OPFMatchPath,
                OPFContPath,
                OPFRegexPath);

            OPFGroup g = new OPFGroup(OPFGroupPath);
            NetworkService svc = new NetworkService(oPFDictionary, g);
            _worker = new Thread(() => svc.main(_listener));
            _logger.Debug("Initializing Open Password Filter Service finished");
        }

        public void Start()
        {
            _worker.Start();
        }

        public void Stop()
        {
            _listener.Close();
            _worker.Abort();
        }
    }
}
