using System;
using System.Collections.Generic;
using System.Linq;

namespace WebRouterApp.Features.Trading.Application.MessageParts
{
    public class PositionsChangedMessage : ITradingMessage
    {
        public PositionsChangedMessage(Guid traderId, PositionsChangeSet changeSet)
        {
            TraderId = traderId;
            ChangeSet = changeSet;
        }
        
        public string Tag => "Trading.PositionsChanged";

        public Guid TraderId { get; }
        public PositionsChangeSet ChangeSet { get; }
    }

    public class PositionsChangeSet
    {
        public PositionsChangeSet(
            IEnumerable<PositionModel> added, 
            IEnumerable<PositionModel> updated, 
            IEnumerable<PositionModel> removed)
        {
            Added = added.ToList();
            Updated = updated.ToList();
            Removed = removed.ToList();
        }
        
        public IReadOnlyList<PositionModel> Added { get; }
        public IReadOnlyList<PositionModel> Updated { get; }
        public IReadOnlyList<PositionModel> Removed { get; }
    }

    public static class PositionsChangeSetExtensions
    {
        public static bool Any(this PositionsChangeSet changeSet) 
            => changeSet.Added.Any() ||
               changeSet.Updated.Any() ||
               changeSet.Removed.Any();

        public static bool IsEmpty(this PositionsChangeSet changeSet)
            => !changeSet.Any();
    }
}