using System;
using System.Collections.Generic;

namespace WebRouterApp.Features.Trading.Application.MessageParts
{
    public class PositionsSnapshotMessage : ITradingMessage
    {
        public PositionsSnapshotMessage(Guid traderId, IReadOnlyList<PositionModel> positions)
        {
            TraderId = traderId;
            Positions = positions;
        }
        
        public string Tag => "Trading.PositionsSnapshot";

        public Guid TraderId { get; }
        public IReadOnlyList<PositionModel> Positions { get; }
    }
}