using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Shared.Interfaces;
using SharedBinance.Models.ProxyConfigurationSection;
using Utility;

namespace BinanceFuturesConnector
{
    public struct ProxyValueStruct
    {
        private ProxyDescriptor _prdes;
        private int _users;

        public ProxyDescriptor Descriptor
        {
            get { return _prdes;}
            set { _prdes = value; }
        }
        public int Users
        {
            get { return _users;}
            set { _users = value; }
        }

        public ProxyValueStruct(ProxyDescriptor prdes, int users)
        {
            _prdes = prdes;
            _users = users;
        }
    }

    public static class ProxyWrapper
    {
        private static ILogger _logger { get; set; }
        private static Dictionary<string, ProxyValueStruct> _proxy = new Dictionary<string, ProxyValueStruct>();

        public static void Configure(ProxyConfig config, ILogger logger)
        {
            _logger = logger;
            foreach (ProxyInstanceElement instance in config.ProxyInstance)
            {
                _proxy?.Add(instance.Server, new ProxyValueStruct(new ProxyDescriptor(instance.Server, Int32.Parse(instance.Port), instance.Login, instance.Password), 0));
                _logger.Log(LogPriority.Debug, $"Proxy added:{instance.Server} {instance.Password} {instance.Login} {instance.Port}", "ProxyWrapper");
            }
        }

        public static ProxyDescriptor? GetHTTPClient(string name)
        {
            if (_proxy.Count == 0)
                return null;
            var keyMin = _proxy.Aggregate((l, r) => l.Value.Users < r.Value.Users ? l : r).Key;
            var value = _proxy[keyMin];
            _proxy[keyMin] = new ProxyValueStruct(value.Descriptor, value.Users+=1);
            _logger.Log(LogPriority.Debug, $"Proxy {keyMin} users: {_proxy[keyMin].Users}", "ProxyWrapper");
            _logger.Log(LogPriority.Debug, $"Proxy: {keyMin} => {name}", "ProxyWrapper");
            return value.Descriptor;
        }

        static void DeleteUser(HttpClient target)
        {
        }
    }
}
