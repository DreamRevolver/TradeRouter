using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

using Communication.Redis.Models;
using Communication.Redis.Services;
using Shared.Interfaces;


namespace Communication.Redis.Specific
{

	public sealed class RedisConnector : IDisposable
	{

		public static bool EnableHardcodedCredentials { get; set; } = false;

		private RedisTransmitter _transmitter;
		private RedisListener _listener;
		private readonly ILogger _logger;
		private readonly NameValueCollection _settings;

		public event Action<ReadOnlyMemory<byte>> OnReceive = delegate { };
		public event Action<Exception> OnException = delegate { };

		private void DispatchReceiveEvent(ReadOnlyMemory<byte> data)
			=> OnReceive(data);
		private void DispatchExceptionEvent(Exception exception)
			=> OnException(exception);

		public RedisConnector(ILogger logger, NameValueCollection settings = null)
		{
			this._settings = !EnableHardcodedCredentials && settings is { } _settings ? _settings : RedisCredentials.DEFAULT;
			_logger = logger;
		}

		public async Task Run()
		{
			_transmitter = new RedisTransmitter(_logger, _settings);
			_listener = new RedisListener(_logger, _settings);
			_listener.OnReceive += DispatchReceiveEvent;
			_listener.OnException += DispatchExceptionEvent;
			await Task.WhenAll(_listener.Run(), _transmitter.Connect());
		}

		public async ValueTask Send(ReadOnlyMemory<byte> data)
			=> await _transmitter.SendAsync(Dummy.Topic.PUBLISH_TOPIC, data);

		public void Dispose()
		{
			_transmitter?.Dispose();
			_listener?.Dispose();
		}

	}

}
