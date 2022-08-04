using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Core.CopyEngineParts.TraderParts
{
    public class PositionSet
    {
        public enum ApplyChangedResultCode
        {
            AddedPosition,
            UpdatedPosition,
            RemovedPosition
        }

        private static readonly PositionEqualityComparer PositionEqComparer = new();
        private readonly HashSet<Position> _positions = new HashSet<Position>(PositionEqComparer);

        private PositionSet()
        {
        }

        public IReadOnlyList<Position> Values => _positions.ToList();

        public static PositionSet Empty { get; } = new PositionSet();

        public static PositionSet With(IEnumerable<Position> positions)
        {
            var positionSet = new PositionSet();
            positionSet._positions.AddRange(positions);
            return positionSet;
        }

        public ApplyChangedResultCode ApplyChanged(Position newOrChangedPosition)
        {
            // Try to remove the position from the set.
            // If it's an existing positions, we'll replace it with the object containing up-to-date values.
            // If it's a new position, removing is a no-op.
            var isNewPosition = !_positions.Remove(newOrChangedPosition);

            if (newOrChangedPosition.positionAmt == 0)
                return ApplyChangedResultCode.RemovedPosition;

            // At this point we know newOrChangedPosition.positionAmt != 0
            // If it's an existing position that has changed, we replace it with up-to-date values.
            // If it's a new position, we add it.
            _positions.Add(newOrChangedPosition);

            return isNewPosition
                ? ApplyChangedResultCode.AddedPosition
                : ApplyChangedResultCode.UpdatedPosition;
        }

        public bool AnyWithSymbol(string symbol) 
            => _positions.Any(it => it.symbol == symbol);

        public IEnumerable<Position> LongWithSymbol(string symbol) 
            => WithSymbolAndSide(symbol: symbol, side: "LONG");

        public IEnumerable<Position> ShortWithSymbol(string symbol)
            => WithSymbolAndSide(symbol: symbol, side: "SHORT");

        private IEnumerable<Position> WithSymbolAndSide(string symbol, string side)
            => _positions.Where(it => it.symbol == symbol && it.positionSide.ToString() == side);

        private class PositionEqualityComparer : IEqualityComparer<Position>
        {
            public bool Equals(Position? left, Position? right)
            {
                if (ReferenceEquals(left, right)) return true;
                if (ReferenceEquals(left, null)) return false;
                if (ReferenceEquals(right, null)) return false;
                if (left.GetType() != right.GetType()) return false;
                return left.symbol == right.symbol && left.positionSide == right.positionSide;
            }

            public int GetHashCode(Position position) 
                => HashCode.Combine(position.symbol, position.positionSide);
        }
    }
}
