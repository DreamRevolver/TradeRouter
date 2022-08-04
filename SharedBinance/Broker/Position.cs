using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using SharedBinance.Extensions;
using SharedBinance.Interfaces;

namespace Shared.Models
{

	public sealed class Position : IEqualityComparer<Position>, ICloneable, IComparable<Position>, IUpdateEvent
	{

		[DataMember(Name = "entryPrice")]
		public double entryPrice { get; set; }

		[DataMember(Name = "marginType")]
		public string marginType { get; set; }

		[DataMember(Name = "isAutoAddMargin")]
		public bool isAutoAddMargin { get; set; }

		[DataMember(Name = "isolatedMargin")]
		public double isolatedMargin { get; set; }

		[DataMember(Name = "leverage")]
		public int leverage { get; set; }

		[DataMember(Name = "liquidationPrice")]
		public double liquidationPrice { get; set; }

		[DataMember(Name = "markPrice")]
		public double markPrice { get; set; }

		[DataMember(Name = "maxNotionalValue")]
		public double maxNotionalValue { get; set; }

		[DataMember(Name = "positionAmt")]
		public double positionAmt { get; set; }

		[DataMember(Name = "symbol")]
		public string symbol { get; set; }

		[DataMember(Name = "unRealizedProfit")]
		public double unRealizedProfit { get; set; }

		[DataMember(Name = "positionSide")]
		public PositionSide positionSide { get; set; }

		[DataMember(Name = "lastUpdate")]
		public long? lastUpdate { get; set; }

		[IgnoreDataMember] public int precision { get; set; } = 3;

		public object Clone()
		{
			var clone = new Position
			{
				entryPrice = entryPrice,
				marginType = marginType,
				isAutoAddMargin = isAutoAddMargin,
				isolatedMargin = isolatedMargin,
				leverage = leverage,
				liquidationPrice = liquidationPrice,
				markPrice = markPrice,
				maxNotionalValue = maxNotionalValue,
				positionAmt = positionAmt,
				symbol = symbol,
				unRealizedProfit = unRealizedProfit,
				positionSide = positionSide,
				lastUpdate = lastUpdate
			};

			return clone;
		}

		public bool Equals(Position x, Position y)
			=> x.symbol.Equals(y.symbol) && x.positionSide.Equals(y.positionSide);

		public int GetHashCode(Position obj) => obj.GetHashCode();

		public int CompareTo(Position other)
			=> string.Compare($"{symbol}#{positionSide}", $"{other.symbol}#{other.positionSide}", StringComparison.Ordinal);

		public sealed class Comparer : IComparer<Position>
		{

			private readonly PositionSortField _sortField;
			private readonly int _sign;

			public Comparer(PositionSortField sortField, bool sign)
			{
				_sortField = sortField;
				_sign = sign ? 1 : -1;
			}

			public int Compare(Position x, Position y)
			{
				if (x is null || y is null)
				{
					return (x is null && y is null ? 0 :
						x is null ? -1 : 1) * _sign;
				}

				return _sortField switch
				{
					PositionSortField.symbol           => x.symbol.NullableCompare(y.symbol) * _sign,
					PositionSortField.entryPrice       => x.entryPrice.NullableCompare(y.entryPrice) * _sign,
					PositionSortField.leverage         => x.leverage.NullableCompare(y.leverage) * _sign,
					PositionSortField.liquidationPrice => x.liquidationPrice.NullableCompare(y.liquidationPrice) * _sign,
					PositionSortField.positionAmt      => x.positionAmt.NullableCompare(y.positionAmt) * _sign,
					PositionSortField.unrealizedProfit => x.unRealizedProfit.NullableCompare(y.unRealizedProfit) * _sign,
					PositionSortField.positionSide     => x.positionSide.ToString().NullableCompare(y.positionSide.ToString()) * _sign,
					_                                  => throw new ArgumentOutOfRangeException()
				};
			}

		}

		[IgnoreDataMember]
		public string Identifier => $"{(positionSide.ToString() is "SHORT" ? "-" : "+")}{symbol}";

		[IgnoreDataMember]
		public long Time => lastUpdate ?? default;

		[IgnoreDataMember]
		private UpdateEventState? _state;

		[IgnoreDataMember]
		public UpdateEventState State { get => _state ?? (positionAmt == 0 ? UpdateEventState.CLOSED : UpdateEventState.OPEN); set => _state = value; }

		public int CompareTo(IUpdateEvent other)
			=> string.Compare(Identifier, other.Identifier, StringComparison.Ordinal);
		public void UpdateFromReport(ExecutionReport report)
		{
			if (report.OrderStatus == OrderStatus.FILLED)
			{
				entryPrice = report.LastFilledPrice;
				lastUpdate = report.LastUpdate;
				if (report.OrderSide == OrderSide.BUY)
				{
					positionAmt = Math.Round(positionAmt + report.Amount, precision);
				} else
				{
					positionAmt = Math.Round(positionAmt - report.Amount, precision);
				}
			}
		}

	}

}
