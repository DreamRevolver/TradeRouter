using Shared.Broker;
using Shared.Models;
namespace SharedBinance.Services
{
    public static class EnumConvertExtension
    {
        public static ConnectionEvent? ToFrontEndEvent(this BrokerEvent @event)
            => @event switch
            {
                BrokerEvent.ConnectorStarted => ConnectionEvent.Started,
                BrokerEvent.ConnectorStopped => ConnectionEvent.Stopped,
                BrokerEvent.SessionLogon => ConnectionEvent.Logon,
                BrokerEvent.SessionLogout => ConnectionEvent.Logout,
                BrokerEvent.GatewayError => ConnectionEvent.ConnectionFault,
                _ => null
            };

    }
}
