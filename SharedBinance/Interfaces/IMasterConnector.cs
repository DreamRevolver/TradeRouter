using System;
using System.Collections.Generic;

using Shared.Interfaces;
using Shared.Models;

using SharedBinance.Models;
namespace SharedBinance.interfaces
{

	public interface IMasterConnector : IEndpoint
	{
        
		event Action<Order> OrderEvent;
		event Action<Position> PositionEvent;
		event Action<LeverageChange> LeverageEvent;
		event Action<DepthSnapshot, string> MarketDataUpdateEvent;
		
		ICollection<ISlaveConnector> Slaves { get; }

		ICollection<Order> Orders { get; }
		ICollection<Position> Positions { get; }

	}

}
