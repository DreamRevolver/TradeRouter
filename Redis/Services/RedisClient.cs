using Communication.Redis.Models;

using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Shared.Interfaces;

using static Communication.Redis.Constants;

namespace Communication.Redis.Services
{


    public sealed class RedisClient : IDisposable
    {

        private readonly ILogger _logger;
        private readonly string _name;

        public RedisClient(ILogger logger, string name = "RedisClient")
        {
            _logger = logger;
            _name = name;
            _logger.Log(LogPriority.Info, "redis client created", _name);
        }

        private Stream Stream { get; set; }
        public bool IsConnected { get; private set; }

        private bool Authenticate(RedisCredentials credentials)
        {
            var username = credentials.Username;
            var password = credentials.Password;
            if (username is null || password is null)
            {
                _logger.Log(LogPriority.Info, $"redis username or password is not provided, authenticate skipped", _name);
                return true;
            }
            var payload = credentials.AuthMessagePayload;
            var passwordMask = string.Join("", Enumerable.Range(0, (password ?? "").Length).Select(_ => '*'));
            _logger.Log(LogPriority.Info, $"trying to login redis by username: \"{username}\" and password: \"{passwordMask}\"...", _name);
            Stream.Write(payload, 0, payload.Length);
            var buffer = new byte[5];
            Stream.Read(buffer, 0, 5);
            var result = buffer.IsOkMessage();
            _logger.Log(LogPriority.Info, $"login redis by username: \"{username}\" and password: \"{passwordMask}\" {(result ? "successfully" : "failed")}", _name);
            return result;
        }

        private ValueTask SetupStream(string host, int port)
        {
            _logger.Log(LogPriority.Info, $"trying to setup redis stream with endpoint [{host}:{port}]...", _name);
            Stream = new TcpClient(host, port).GetStream();
            _logger.Log(LogPriority.Info, $"stream with endpoint [{host}:{port}] setup successfully", _name);
            return new ValueTask(Task.CompletedTask);
        }

        public async Task Connect(RedisCredentials credentials = null)
        {
        
            _logger.Log(LogPriority.Info, $"trying to connect redis...", _name);
            if (credentials is null)
            {
                throw new ArgumentNullException(nameof(credentials), "redis credentials must not be null");
            }
            if (IsConnected)
            {
                _logger.Log(LogPriority.Warning, "attempt to connect redis with already connected client", _name);
                Disconnect();
            }
            try
            {
                await SetupStream(credentials.Host, credentials.Port);
                IsConnected = Authenticate(credentials);
                _logger.Log(IsConnected ? LogPriority.Info : LogPriority.Error, IsConnected
                    ? "redis client connected successfully" : "redis client connection failed", _name);
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, _name, "Redis client connecting error");
                Disconnect();
                IsConnected = false;
            }
        }

        public void Disconnect()
        {
            _logger.Log(LogPriority.Info, "trying to disconnect redis...", _name);
            Stream?.Close();
            Stream = null;
            IsConnected = false;
            _logger.Log(LogPriority.Info, "redis client disconnected successfully", _name);
        }

        public async ValueTask<bool> WriteAsync(params ReadOnlyMemory<byte>[] message)
        {
            if (!IsConnected)
            {
                _logger.Log(LogPriority.Debug, "attempt to write with redis client which not connected", "Redis client");
                //throw new Exception("attempt to write with redis client which not connected");
            }
            try
            {
                foreach (var i in message)
                {
                    await Stream.WriteAsync(i.ToArray(), 0, i.Length);
                }
                await Stream.WriteAsync(_separator, 0, _separator.Length);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, "Redis client", "Error writing to redis");
                Disconnect();
                return false;
            }
        }

        public async ValueTask<int?> ReadAsync(byte[] buffer, int count, CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
            {
                throw new Exception("attempt to read with redis client which not connected");
            }
            try
            {
                var result = await Stream.ReadAsync(buffer, 0, count, cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex,"Redis client", "Error reading from redis");
                Disconnect();
                return null;
            }
        }

        private readonly byte[] _byte = new byte[1];
        public async ValueTask<int> ReadByteAsync(CancellationToken cancellationToken = default)
            => await ReadAsync(_byte, 1, cancellationToken) is 1 ? _byte[0] : -1;

        public void Dispose()
        {
            Stream?.Dispose();
            _logger.Log(LogPriority.Info, "redis client was disposed", _name);
        }

    }

}
