using System;
using System.Threading.Tasks;
using Shared.Broker;
namespace SharedBinance.Broker
{
    public interface IConnector : IDisposable
    {
        Task Start();
        void Stop();
        bool? IsStarted { get; }
        IBrokerClient Connector { get; }
    }
}
