using Communication.Redis.Models;


using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Shared.Interfaces;

using static Communication.Redis.Constants;

namespace Communication.Redis.Services
{

	public sealed class RedisTransmitter : IDisposable
	{

		private readonly RedisClient _client;
		private readonly RedisCredentials _credentials;

        public RedisTransmitter(ILogger logger, NameValueCollection settings, string name = "RedisTransmitter")
		{
			_client = new RedisClient(logger);
			_credentials = new RedisCredentials()
			{
				Host = settings.Get("RedisHost"),
				Port = settings.Get("RedisPort") is { } _port && int.TryParse(_port, out var port) ? port : DEFAULT_PORT,
				Username = settings.Get("RedisUsername"),
				Password = settings.Get("RedisPassword"),
			};
			logger.Log(LogPriority.Info, $"redis transmitter was created", name);
		}

		// ReSharper disable once UnusedMember.Global
		public bool IsConnected => _client.IsConnected;
		public async Task Connect() => await _client.Connect(_credentials);

		public async ValueTask SendAsync(RedisTopic topic, ReadOnlyMemory<byte> data)
			=> await _client.WriteAsync(topic, Convert.ToBase64String(data.Span.ToArray()).GetBytes());

		public void Dispose() => _client.Dispose();

	}

}
