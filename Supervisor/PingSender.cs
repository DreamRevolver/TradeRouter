using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Communication.Redis.Models;
using Communication.Redis.Services;
using Communication.Redis.Specific;

using JetBrains.Annotations;

using Shared.Interfaces;
using static Communication.Redis.Constants;
using SharedBinance.interfaces;
using SharedBinance.Models;

namespace Supervisor
{

	public sealed class PingSender: IVirtualMachine
	{
		private RedisClient _redisClient;
		private RecurrentAction _recurrentAction;
		private ILogger _logger;
		private readonly RedisCredentials _credentials;
		private string _id;
		public PingSender([NotNull] string id, [NotNull] ILogger logger, [CanBeNull] NameValueCollection settings)
		{
			_logger = logger;
			_id = id;
			_credentials = new RedisCredentials()
			{
				Host = settings.Get("RedisHost"),
				Port = settings.Get("RedisPort") is { } _port && int.TryParse(_port, out var port) ? port : DEFAULT_PORT,
				Username = settings.Get("RedisUsername"),
				Password = settings.Get("RedisPassword")
			};
			_redisClient = new RedisClient(_logger);
			_recurrentAction = new RecurrentAction(async () => await SendPing(), 5000);
		}
		private async Task SendPing()
		{
			if (_redisClient.IsConnected)
			{
				await _redisClient.WriteAsync(Encoding.UTF8.GetBytes(_id).AsMemory());
			}
			else
			{
				await _redisClient.Connect(_credentials);
				_logger.Log(LogPriority.Debug, "Failed connect to Redis", "PingSender");
			}
		}
	}

}
