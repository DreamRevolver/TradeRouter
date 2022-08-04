using System;
using System.Collections.Generic;
namespace SharedBinance.Models
{
    public struct DepthSnapshot
    {
        public IEnumerable<MarketUpdate> bids;
        public IEnumerable<MarketUpdate> asks;
        public DateTime lastTimeUpdate;
        public readonly long sequence;
        public readonly long firstUpdateId;
        public readonly long lastUpdateId;
        public DepthSnapshot(IEnumerable<MarketUpdate> bid, IEnumerable<MarketUpdate> ask, DateTime timeUpdate, long sequence = 0, long startId = 0, long endId = 0)
        {
            bids = bid;
            asks = ask;
            lastTimeUpdate = timeUpdate;
            this.sequence = sequence;
            firstUpdateId = startId;
            lastUpdateId = endId;
        }
        public bool IsValid
            => bids != null && asks != null;

    }
}
