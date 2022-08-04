using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shared.Interfaces;
using SharedBinance.interfaces;

using Utility;
using Utility.Benchmark;
using Utility.Benchmark.Default;
using Utility.Benchmark.Interfaces;

namespace BinanceFuturesConnector
{
    public class RestClient
    {

        private readonly ILogger _logger;
        private readonly string _APIKey;
        private readonly string _APISecret;
        private readonly string _name;
        private readonly string _domain;
        private readonly HttpClient _client;
        private readonly IBenchmarkScope _benchmark;
        private bool Authorized => !string.IsNullOrWhiteSpace(_APIKey) && !string.IsNullOrWhiteSpace(_APISecret);

        public RestClient(ILogger logger, ISettingStorage settings, string name, ProxyDescriptor? proxy = null)
        {
            _logger = logger;
            _name = name;
            _APIKey = settings.Get("APIKey");
            _APISecret = settings.Get("APISecret");
            _name = name;
            _domain = settings.Get("Url");
            _client = proxy is {} __proxy ? __proxy.Client : new HttpClient();
            _client.DefaultRequestHeaders.Add("X-MBX-APIKEY", _APIKey);
            _benchmark = DefaultBenchmark.ScopeFactory.MakeScope(
                (ref BenchmarkState state) =>
                {
                    if(state.iterationCount % 16 != 0)
                        return;
                    _logger.Log(LogPriority.Debug, $"Benchmark: {state}", _name);
                    state.Reset();
                });
        }
        
        public async Task<HttpResponseMessage> SendRequestFull(string url, HttpMethod method, bool sendApiSign = false, string parameters = null, string recvWindow = null)
        {
            if (!Authorized && sendApiSign)
            {
                _logger.Log(LogPriority.Error, "Can't send signed request with not authorized client", _name);
                throw new ArgumentException("Can't send signed request with not authorized client", nameof(sendApiSign));
            }
            var finalEndpoint = $"{_domain}{url}";
            if (sendApiSign)
            {
                parameters += (!string.IsNullOrWhiteSpace(parameters) ? "&timestamp=" : "timestamp=")
                              + Utilities.GenerateTimeStamp(DateTime.UtcNow) + $"&recvWindow={(string.IsNullOrWhiteSpace(recvWindow) ? "6000" : recvWindow)}";
                finalEndpoint = $"{_domain}{url}?{parameters}&signature={Utilities.GenerateSignature(_APISecret, parameters)}";
            } else if (parameters != null)
            {
                finalEndpoint = $"{_domain}{url}?{parameters}";
            }
            try
            {
                using (_benchmark.Context)
                {
                    return await _client.SendAsync(new HttpRequestMessage(method, finalEndpoint)).ConfigureAwait(false);
                };
            } catch (WebException ex)
            {
                using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    _logger.Log(LogPriority.Error, $"WebException => {await reader.ReadToEndAsync()}", _name);
                }
                throw;
            } catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, $"Rest client exception: {ex.Message} => {ex.StackTrace}", _name);
                throw;
            }
        }
        public async Task<string> SendRequest(string url, HttpMethod method, bool sendApiSign = false, string parameters = null, string recvWindow = null)
            => await (await SendRequestFull(url, method, sendApiSign, parameters, recvWindow)).Content.ReadAsStringAsync();

    }
}
