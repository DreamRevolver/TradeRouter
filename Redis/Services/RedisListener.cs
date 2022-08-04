using Communication.Redis.Models;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Communication.Redis.Specific;

using Shared.Interfaces;

using static Communication.Redis.Constants;

namespace Communication.Redis.Services
{

	internal sealed class RedisListener : IDisposable
	{

		public event Action<ReadOnlyMemory<byte>> OnReceive = delegate { };
		public event Action<Exception> OnException = delegate { };

		private readonly ILogger _logger;
		private CancellationTokenSource _cancellationToken;
		private readonly RedisClient _client;
		private readonly RedisCredentials _credentials;
		private readonly byte[] _buffer;
		private readonly string _name;

		public RedisListener(ILogger logger, NameValueCollection settings, string name = "RedisListener")
		{
			_logger = logger;
			_name = name;
			_client = new RedisClient(logger);
			_buffer = new byte[
				settings.Get("RedisBufferSize") is { } _bufferSize && int.TryParse(_bufferSize, out var bufferSize)
					? bufferSize : DEFAULT_BUFFER_SIZE];
			_credentials = new RedisCredentials()
			{
				Host = settings.Get("RedisHost"),
				Port = settings.Get("RedisPort") is { } _port && int.TryParse(_port, out var port) ? port : DEFAULT_PORT,
				Username = settings.Get("RedisUsername"),
				Password = settings.Get("RedisPassword")
			};
			_logger.Log(LogPriority.Info, $"redis listener with host: [{_credentials.Host}:{_credentials.Port}] was created", _name);
		}

		public RedisTopic Topic => Dummy.Topic.SUBSCRIBE_TOPIC;

		public bool IsRunning { get; private set; }
		public bool IsConnected => _client.IsConnected;
		private Task _polling;
		public async Task Run()
		{
			_logger.Log(LogPriority.Info, "run redis listener", _name);
			if (IsRunning)
			{
				_logger.Log(LogPriority.Warning, "running redis listener which already running", _name);
				ShutDown();
			}
			await _client.Connect(_credentials);
			if (!IsConnected)
			{
				throw new Exception("redis listener connecting error: can't connect redis client");
			}
			await (_polling = Polling());
		}
		public void ShutDown()
		{
			_logger.Log(LogPriority.Info, "shut down redis listener", _name);
			if (!IsRunning)
			{
				_logger.Log(LogPriority.Warning, "attempt to shut down redis listener which not running", _name);
				return;
			}
			_cancellationToken?.Cancel();
			_polling?.Wait();
			_client?.Disconnect();
		}

		private async Task Polling()
		{
			_logger.Log(LogPriority.Info, "trying to start redis listener polling...", _name);
			IsRunning = await _client.WriteAsync(Topic);
			if (!IsRunning || !IsConnected)
			{
				_logger.Log(LogPriority.Error, "starting redis listener failed", _name);
				return;
			}
			_cancellationToken = new CancellationTokenSource();
			while (!_cancellationToken.IsCancellationRequested)
			{
				try
				{
					var received = await ReceiveAsync(_cancellationToken.Token);
					if (received != null)
					{
						OnReceive(received);
					}
				}
				catch (Exception ex)
				{
					_logger.Log(LogPriority.Error, ex, _name, "redis listener polling error");
					OnException(ex);
				}
			}
			IsRunning = false;
			_logger.Log(LogPriority.Info, "redis listener polling completed", _name);
		}

		private async ValueTask<string> ReadString(CancellationToken cancellationToken)
		{
			var length = 0;
			for (var i = await _client.ReadByteAsync(cancellationToken); i != '\n' && i != -1; i = await _client.ReadByteAsync(cancellationToken))
			{
				_buffer[length++] = (byte)i;
			}
			return _buffer.AsMemory().Slice(0, length).ToArray().FromBytes();
		}
		private async ValueTask<long> ReadInteger(CancellationToken cancellationToken)
			=> long.Parse(await ReadString(cancellationToken));
		private async ValueTask<int> SkipData(int length, CancellationToken cancellationToken)
		{
			var received = 0;
			do
			{
				var temp = await _client.ReadAsync(_buffer, length >= _buffer.Length ? _buffer.Length : length, cancellationToken) ?? 0;
				received += temp;
				length -= temp;
			} while (length != 0);
			return received;
		}
		private async ValueTask<int> SkipBulkString(CancellationToken cancellationToken)
			=> await SkipData((int)await ReadInteger(cancellationToken) + 2, cancellationToken);
		private async ValueTask<byte[]> ReadBulkString(CancellationToken cancellationToken)
		{
			await ReadRespTypeAsync(cancellationToken);
			var buffer = new byte[await ReadInteger(cancellationToken)];
			return await _client.ReadAsync(buffer, buffer.Length, cancellationToken) is {} ? buffer : null;
		}
		private async ValueTask<RedisRespType> ReadRespTypeAsync(CancellationToken cancellationToken)
		{
			var received = await _client.ReadByteAsync(cancellationToken);
			return (RedisRespType)received;
		}
		private async ValueTask Skip(CancellationToken cancellationToken)
		{
			switch (await ReadRespTypeAsync(cancellationToken))
			{
			case RedisRespType.Array:
				await SkipArrayElements((int)await ReadInteger(cancellationToken), cancellationToken);
				break;
			case RedisRespType.BulkString:
				_ = await SkipBulkString(cancellationToken);
				break;
			case RedisRespType.Integer:
				_ = await ReadInteger(cancellationToken);
				break;
			case RedisRespType.SimpleString:
			case RedisRespType.Error:
				_ = await ReadString(cancellationToken);
				break;
			}
		}
		private async ValueTask SkipArrayElements(int count, CancellationToken cancellationToken)
		{
			for (var i = 0; i < count; ++i)
			{
				await Skip(cancellationToken);
			}
		}
		private async ValueTask SkipToEndAsync(CancellationToken cancellationToken)
		{
			int? received;
			do
			{
				received = await _client.ReadAsync(_buffer, _buffer.Length, cancellationToken);
			} while (received == _buffer.Length);
		}
		private async ValueTask<byte[]> ReceiveAsync(CancellationToken cancellationToken)
		{
			var type = await ReadRespTypeAsync(cancellationToken);
			if (type != RedisRespType.Array)
			{
				await SkipToEndAsync(cancellationToken);
				return null;
			}
			var size = (int)await ReadInteger(cancellationToken);
			if (size != 4)
			{
				await SkipToEndAsync(cancellationToken);
				return null;
			}
			await SkipArrayElements(size - 1, cancellationToken);
			var result = (await ReadBulkString(cancellationToken)).Select(Convert.ToChar).ToArray();
			return Convert.FromBase64CharArray(result, 0, result.Length);
		}

		public void Dispose()
		{
			ShutDown();
			_client?.Dispose();
		}

	}

}
