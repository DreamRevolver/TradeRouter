using System;
using JetBrains.Annotations;

using Shared.Interfaces;
using SharedBinance.interfaces;

namespace SharedBinance.Models
{

    public sealed class BinanceConnectorSettings : ISettingStorage
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _domain;
        private readonly string _path;
        private readonly string _retryAttempts;
        private readonly string _retryDelayMs;
        public BinanceConnectorSettings([NotNull] IDataStorage keys)
        {
            _apiKey = keys.Get("APIKey");
            _apiSecret = keys.Get("APISecret");
            _domain = keys.Get("Url");
            _path = keys.Get("Wss");
            _retryAttempts = keys.Get("RetryAttempts");
            _retryDelayMs = keys.Get("RetryDelayMs");
        }
        [CanBeNull]
        public string Get([CanBeNull] string key)
            => key switch
            {
                "APIKey" => _apiKey,
                "APISecret" => _apiSecret,
                "Url" => _domain,
                "Wss" => _path,
                "RetryAttempts" => _retryAttempts,
                "RetryDelayMs" => _retryDelayMs,
                _ => null
            };
    }

}
