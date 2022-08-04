using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Shared.Interfaces;
using Shared.Models;

using SharedBinance.Models;
namespace SharedBinance.interfaces
{

	public interface ISlaveConnector: IEndpoint
	{
		event Action<ExecutionReport, string> OrderUpdateEvent;
		event Action<ConnectionEvent> ConnectionEvent;
		event Action<Position> PositionEvent;
		event Action<DepthSnapshot, string> MarketDataUpdateEvent;
		
		IMasterConnector Master { get; }

		ICollection<Order> Orders { get; }
		ICollection<Position> Positions { get; }
		
		bool AllowChangeSettings { get; }
		
		double GetBalanceValue(WalletCurrency currency);
		
		Task CancelAllOrders();
		Task CloseAllPositions();
	}

}
