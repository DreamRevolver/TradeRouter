using System.Collections.Specialized;
using System.Text;

namespace Communication.Redis.Models
{
	public sealed class RedisCredentials
	{
		public static readonly NameValueCollection DEFAULT = new NameValueCollection()
		{
			["RedisBufferSize"] = "4096",
			["RedisHost"] = "redis-19314.c135.eu-central-1-1.ec2.cloud.redislabs.com",
			["RedisPort"] = "19314",
			["RedisUsername"] = "FDEB5B9B-233F-4914-93BB-2CE2CEA5E5FE",
			["RedisPassword"] = "1488_Router"
		};
		public string Host { get; set; } = "redis-19314.c135.eu-central-1-1.ec2.cloud.redislabs.com";
		public int Port { get; set; } = 19314;
		public string Username { get; set; } = "FDEB5B9B-233F-4914-93BB-2CE2CEA5E5FE";
		public string Password { get; set; } = "1488_Router";
		public byte[] AuthMessagePayload => Encoding.UTF8.GetBytes($"AUTH {Username} {Password}\r\n");
	}

}
