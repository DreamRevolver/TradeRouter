using System.Collections.Generic;

namespace WebRouterApp.Core.CopyEngineParts
{
    public static class AppSettingConstants
    {
        public static readonly string ApiKey = "APIKey";
        public static readonly string ApiSecret = "APISecret";
        public static readonly string Url = "Url";
        public static readonly string Wss = "Wss";
        public static readonly string CopyMasterOrders = "CopyMasterOrders";
        public static readonly string TeleBotToken = "TeleBotToken";

        public static readonly IReadOnlyList<string> PublisherKeys = new[]
        {
            Url,
            Wss
        };

        public static readonly IReadOnlyList<string> SubscriberKeys = new[]
        {
            Url,
            Wss,
            CopyMasterOrders,
            TeleBotToken
        };
    }
}
