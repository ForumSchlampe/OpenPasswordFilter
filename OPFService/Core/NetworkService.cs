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
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using TcpSharp;
using TcpSharp.Events;
using Topshelf.Logging;

namespace OPFService.Core;

public sealed class NetworkService
{
    public class ClientDataReceivedEventArgs : EventArgs
    {
        public ClientDataReceivedEventArgs(string connectionId, string ipAddress, byte[] data)
        {
            this.ConnectionId = connectionId;
            this.IpAddress = ipAddress;
            this.Data = data;
        }

        public string ConnectionId { get; }
        public string IpAddress { get; set; }
        public byte[] Data { get; }
    }

    public event EventHandler<ClientDataReceivedEventArgs> ClientDataReceived;

    private readonly LogWriter logger = HostLogger.Get<NetworkService>();
    private readonly TcpSharpSocketServer tcpsharpServer = new();
    private readonly ConcurrentDictionary<string, byte[]> clientDataObserver = new();

    public NetworkService()
    {
        this.tcpsharpServer.OnStarted += OnStarted;
        this.tcpsharpServer.OnStopped += OnStopped;
        this.tcpsharpServer.OnError += OnError;
        this.tcpsharpServer.OnConnected += OnConnected;
        this.tcpsharpServer.OnDisconnected += OnDisconnected;
        this.tcpsharpServer.OnDataReceived += OnDataReceived;
    }

    public void StartServer(int port)
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(StartServer)}";
        this.logger.Debug($"[{methodName}] - Start server on port:{port}...");

        if (!IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(endPoint => endPoint.Port == port))
        {
            this.tcpsharpServer.Port = port;

            try
            {
                this.tcpsharpServer.StartListening();
            }
            catch (Exception ex)
            {
                this.logger.Error($"[{methodName}] - Unable to bind local port {this.tcpsharpServer.Port}", ex);
            }
        }
        else
        {
            this.logger.Error($"[{methodName}] - Unable to start server. Port:{port} already in use");
            return;
        }
    }

    private void OnStarted(object sender, OnServerStartedEventArgs e)
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(OnStarted)}";

        if (e.IsStarted)
        {
            this.logger.Debug($"[{methodName}] - Server started with port {this.tcpsharpServer.Port}");
        }
    }

    public void StopServer()
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(StopServer)}";
        this.logger.Debug($"[{methodName}] - Stop server...");

        if (this.tcpsharpServer.Listening)
        {
            this.tcpsharpServer.StopListening();
        }
    }

    public void SendString(string connectionId, string message)
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(SendString)}";
        this.logger.Debug($"[{methodName}] - SendString");

        if (this.tcpsharpServer.GetClient(connectionId).Connected)
        {
            try
            {
                this.tcpsharpServer.SendString(connectionId, message);
                this.tcpsharpServer.Disconnect(connectionId);
            }
            catch (Exception ex)
            {
                this.logger.Warn($"[{methodName}] - Client {connectionId} already disconnect", ex);
            }
        }
    }

    private void OnStopped(object sender, OnServerStoppedEventArgs e)
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(StopServer)}";

        if (e.IsStopped)
        {
            this.logger.Debug($"[{methodName}] - Server stopped");
        }
    }

    private void OnDataReceived(object sender, OnServerDataReceivedEventArgs e)
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(OnDataReceived)}";
        this.logger.Debug($"[{methodName}] - Client {e.ConnectionId} data received. Length: {e.Data.Length}");

        string ipAddress = ((IPEndPoint)(e.Client.Client.RemoteEndPoint)).Address.ToString();

        //workaround, if client sending multiple or just one time the data
        //if the first data length is 5, it means just the command 'test' is sent
        //we need to store the data and invoke it after the payload 
        if (e.Data.Length != 5 || !this.clientDataObserver.TryAdd(e.ConnectionId, e.Data))
        {
            this.ClientDataReceived?.Invoke(this,
                new(e.ConnectionId, ipAddress, this.clientDataObserver.TryRemove(e.ConnectionId, out var odata) ? odata.Concat(e.Data).ToArray() : e.Data));
        }
    }

    private void OnDisconnected(object sender, OnServerDisconnectedEventArgs e)
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(OnDisconnected)}";
        this.logger.Debug($"[{methodName}] - Client {e.ConnectionId} disconnected. Reason: {e.Reason}");
    }

    private void OnConnected(object sender, OnServerConnectedEventArgs e)
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(OnConnected)}";
        this.logger.Debug($"[{methodName}] - Client {e.ConnectionId} connected");
    }

    private void OnError(object sender, OnServerErrorEventArgs e)
    {
        var methodName = $"{nameof(NetworkService)}::{nameof(OnError)}";
        this.logger.Debug($"[{methodName}] - Server throwns an exception", e.Exception);
    }
}
