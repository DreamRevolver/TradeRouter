using System.Net;
using System.Net.Http;

namespace Utility
{

	public static class ProxyHelper
	{

		public static HttpClient MakeProxyHttpClient(string ip, int port, string login, string password)
		{
			var handler = new HttpClientHandler();
			handler.Proxy = new WebProxy(ip, port);
			handler.Proxy.Credentials = new NetworkCredential(login, password);
			return new HttpClient(handler);
		}

	}

}
