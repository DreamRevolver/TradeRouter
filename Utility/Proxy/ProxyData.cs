using System.Net.Http;

namespace Utility
{

	public readonly struct ProxyDescriptor
	{

		internal readonly string _ip;
		internal readonly int _port;
		internal readonly string _login;
		internal readonly string _password;

		public ProxyDescriptor(string ip, int port, string login, string password)
		{
			_ip = ip;
			_port = port;
			_login = login;
			_password = password;
		}
		public override string ToString()
			=> $"{_ip}:{_port}";
		public HttpClient Client => ProxyHelper.MakeProxyHttpClient(_ip, _port, _login, _password);

	}

}
