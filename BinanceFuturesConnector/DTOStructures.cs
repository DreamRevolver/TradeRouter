using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Shared.Models;

using SharedBinance.Models;

using Utf8Json;

// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 649
namespace BinanceFuturesConnector
{
	
	public struct BookTickerDTO
	{
		public string symbol;
		public string bidPrice;
		public string bidQty;
		public string askPrice;
		public string askQty;
		public long? time;
	}

	public struct OrderBookDTO
	{
		public long? lastUpdateId;
		public long? E;
		public long? T;
		public IEnumerable<string[]> bids;
		public IEnumerable<string[]> asks;
	}

	public struct LeverageDTO
	{
		public long? leverage;
		public string maxNotionalValue;
		public string symbol;
	}

	public struct PositionDTO
	{
		public string entryPrice;
		public string marginType;
		public string isAutoAddMargin;
		public string isolatedMargin;
		public string leverage;
		public string liquidationPrice;
		public string markPrice;
		public string maxNotionalValue;
		public string positionAmt;
		public string symbol;
		public string unRealizedProfit;
		public string positionSide;
		public long? lastUpdate;
		public Position Convert() => new Position
		{
			entryPrice = Double.Parse(entryPrice),
			marginType = marginType,
			isAutoAddMargin = Boolean.Parse(isAutoAddMargin),
			isolatedMargin = Double.Parse(isolatedMargin),
			leverage = Int32.Parse(leverage),
			liquidationPrice = Double.Parse(liquidationPrice),
			markPrice = Double.Parse(markPrice),
			maxNotionalValue = Double.Parse(maxNotionalValue),
			positionAmt = Double.Parse(positionAmt),
			symbol = symbol,
			unRealizedProfit =  Double.Parse(unRealizedProfit),
			positionSide = (PositionSide) Enum.Parse(typeof(PositionSide), positionSide, true),
			lastUpdate = lastUpdate
		};
	}

	public struct ListenKeyDTO
	{
		public string listenKey;
	}

	public struct BinanceSteamEventDTO
	{
		public string e;
		public long? E;
		public long? T;
		public string cw;
		public IEnumerable<MarginCallPositionDTO> p;
		public AccountUpdateDataDTO a;
		public OrderUpdateDataDto o;
		public LeverageUpdateDTO? ac;

		public ExecutionReport CreateReport()
			=> new ExecutionReport
			{
				Symbol = o.s,
				ClientId = o.c,
				Amount = Double.Parse(o.q),
				Price = Double.Parse(o.p),
				ExchangeId = o.i.ToString(),
				OrderStatus = Utilities.ParseOrderStatus(o.X.ToString()),
				LastFilledPrice = Double.Parse(o.L),
				AvgPx = Double.Parse(o.ap),
				FilledQty = Double.Parse(o.l),
				LeavesQty = Double.Parse(o.q) - Double.Parse(o.z),
				CumQty = Double.Parse(o.q),
				StopPrice = Double.Parse(o.sp),
				OrderType = (OrderType) Enum.Parse(typeof(OrderType), o.o.ToString(), true),
				OrderSide = (OrderSide) Enum.Parse(typeof(OrderSide), o.S.ToString(), true),
				PositionSide = (PositionSide) Enum.Parse(typeof(PositionSide), o.ps.ToString(), true),
				LastUpdate = E ?? default,
				ClosePosition = Boolean.Parse(o.cp.ToString()),
				WorkingType = (OrderWorkingType) Enum.Parse(typeof(OrderWorkingType), o.wt.ToString(), true)
			};
	}

	public struct LeverageUpdateDTO
	{
		public string s;
		public int l;
	}

	public struct AccountUpdateDataDTO
	{
		public string m;
		public IEnumerable<BalanceUpdateDto> B;
		public IEnumerable<PositionUpdateDto> P;
	}

	public struct MarginCallPositionDTO
	{
		public string s;
		public string ps;
		public string pa;
		public string mt;
		public string iw;
		public string mp;
		public string up;
		public string mm;
	}
	

	public struct BinanceErrorDto
	{
		public ErrorCodeWrapper code;
		public string msg;
	}

	public readonly struct BinanceResponseDto<T>
	{
		public readonly BinanceErrorDto? error;
		public readonly T data;
		public BinanceResponseDto(string json)
		{
			var isError = json.Contains("msg") && json.Contains("code");
			error = isError ? JsonSerializer.Deserialize<BinanceErrorDto>(json) : (BinanceErrorDto?)null;
			data = isError ? default(T) : JsonSerializer.Deserialize<T>(json);
			if (error != null && error.Value.code.ToString() == "200")
				error = null;
		}
	}
	public struct CanceledOrderDTO
	{
		public string clientOrderId;
		public string cumQty;
		public string cumQuote;
		public string executedQty;
		public long? orderId;
		public string origQty;
		public string origType;
		public string price;
		public bool? reduceOnly;
		public string side;
		public string positionSide;
		public string status;
		public string stopPrice;
		public bool? closePosition;
		public string symbol;
		public string timeInForce;
		public string type;
		public string activatePrice;
		public string priceRate;
		public long? updateTime;
		public string workingType;
		public bool? priceProtect;
	}
	public struct InstrumentDTO
	{
		public string symbol;
		public string pair;
		public string contractType;
		public long? deliveryDate;
		public long? onboardDate;
		public string status;
		public string maintMarginPercent;
		public string requiredMarginPercent;
		public string baseAsset;
		public string quoteAsset;
		public string marginAsset;
		public long? pricePrecision;
		public long? quantityPrecision;
		public long? baseAssetPrecision;
		public long? quotePrecision;
		public string underlyingType;
		public long? settlePlan;
		public string triggerProtect;
		public string liquidationFee;
		public string marketTakeBound;

		public Instrument Convert()
		{
			var instr = new Instrument
			{
				Exchange = "Binance",
				Symbol = symbol,
				BaseAsset = baseAsset,
				QuoteAsset = quoteAsset,
				MarginAsset = marginAsset,
				PricePrecision = pricePrecision,
				QuantityPrecision = quantityPrecision,
				BaseAssetPrecision = baseAssetPrecision,
				QuotePrecision = quotePrecision,
				DeliveryDate = Utilities.ConvertFromUnixTimestamp(deliveryDate ?? default),
			};
			if (Enum.TryParse(contractType, true, out ContractType contract))
			{
				instr.ContractType = contract;
			}
			return instr;
		}

	}
	public struct rateLimitsDTO
	{
		public string interval;
		public long? intervalNum;
		public long? limit;
		public string rateLimitType;
	}
	public struct assetsDTO
	{
		public string asset;
		public bool? marginAvailable;
		public string autoAssetExchange;
	}
	public struct filtersDTO
	{
		public string filterType;
		public string maxPrice;
		public string minPrice;
		public string tickSize;
	}
	public struct ExchInfoDTO
	{
		public IEnumerable<rateLimitsDTO> rateLimits;
		public IEnumerable<assetsDTO> assets;
		public IEnumerable<InstrumentDTO> symbols;
		
	}
	public struct OrderDTO
	{
		public string clientOrderId;
		public string cumQty;
		public string cumQuote;
		public string executedQty;
		public long? orderId;
		public string avgPrice;
		public string origQty;
		public string price;
		public bool? reduceOnly;
		public string side;
		public string positionSide;
		public string status;
		public string stopPrice;
		public bool? closePosition;
		public string symbol;
		public string timeInForce;
		public string type;
		public string origType;
		public string activatePrice;
		public string priceRate;
		public long? updateTime;
		public string workingType;
		public bool? priceProtect;
	}
	public struct AccountInfoDTO
	{
		public long? feeTier;
		public bool? canTrade;
		public bool? canDeposit;
		public bool? canWithdraw;
		public long? updateTime;
		public string totalInitialMargin;
		public string totalMaintMargin;
		public string totalWalletBalance;
		public string totalUnrealizedProfit;
		public string totalMarginBalance;
		public string totalPositionInitialMargin;
		public string totalOpenOrderInitialMargin;
		public string totalCrossWalletBalance;
		public string totalCrossUnPnl;
		public string availableBalance;
		public string maxWithdrawAmount;
		public BinanceAssetDTO[] assets;
		public PositionInfoDTO[] positions;
	}
	public struct PositionInfoDTO
	{
		public string symbol;
		public string initialMargin;
		public string maintMargin;
		public string unrealizedProfit;
		public string positionInitialMargin;
		public string openOrderInitialMargin;
		public string leverage;
		public bool? isolated;
		public string entryPrice;
		public string maxNotional;
		public string positionSide;
		public string positionAmt;
		public long? updateTime;
		public Position Convert() => new Position
		{
			entryPrice = Double.Parse(entryPrice),
			isAutoAddMargin = isolated ?? default,
			leverage = Int32.Parse(leverage),
			maxNotionalValue = Double.Parse(maxNotional),
			positionAmt = Double.Parse(positionAmt),
			symbol = symbol,
			unRealizedProfit =  Double.Parse(unrealizedProfit),
			positionSide = (PositionSide) Enum.Parse(typeof(PositionSide), positionSide, true),
			lastUpdate = updateTime
		};
	}
	public struct OpenOrderDTO
	{
		public string avgPrice;
		public string clientOrderId;
		public string cumQuote;
		public string executedQty;
		public long? orderId;
		public string origQty;
		public string origType;
		public string price;
		public bool? reduceOnly;
		public string side;
		public string positionSide;
		public string status;
		public string stopPrice;
		public bool closePosition;
		public string symbol;
		public long? time;
		public string timeInForce;
		public string type;
		public string activatePrice;
		public string priceRate;
		public long? updateTime;
		public string workingType;
		public bool? priceProtect;

		public Order Convert()
			=> new Order
			{
				Symbol = symbol,
				OrderSide = (OrderSide) Enum.Parse(typeof(OrderSide), side, true),
				OrderStatus = (OrderStatus) Enum.Parse(typeof(OrderStatus), status, true),
				Price = double.Parse(price),
				Amount = double.Parse(origQty),
				StopPrice = double.Parse(stopPrice),
				PositionSide = (PositionSide) Enum.Parse(typeof(PositionSide), positionSide, true),
				ExchangeId = orderId.ToString(),
				ClientId = clientOrderId,
				OrderType = (OrderType) Enum.Parse(typeof(OrderType), origType, true),
				ClosePosition = closePosition
			};

	}

	public struct OrderUpdateDataDto
	{

		public string s;
		public string c;
		public OrderSide S;
		public OrderType o;
		public OrderTimeInForce f;
		public string q;
		public string p;
		public string ap;
		public string sp;
		public OrderExecutionType x;
		public OrderStatus X;
		public long? i;
		public string l;
		public string z;
		public string L;
		public string N;
		public string n;
		public long? T;
		public long? t;
		public string b;
		public string a;
		public bool? m;
		public bool? R;
		public OrderWorkingType wt;
		// public string ot;		//TRAILING_STOP_MARKET",
		public PositionSide ps;
		public bool? cp;
		public string AP;
		public string cr;
		public bool pP;
		public double si;
		public double ss;
		public string rp;

	}

	public struct OrderUpdateDto
	{

		public long E;
		public OrderUpdateDataDto o;

		public Order ToOrder()
			=> new Order
			{
				ClientId = o.c,
				OrderType = o.o,
				OrderStatus = o.X,
				OrderSide = o.S,
				PositionSide = o.ps,
				TimeInForce = o.f,
				ExecutionType = o.x,
				WorkingType = o.wt,
				Amount = double.Parse(o.q),
				Price = double.Parse(o.p),
				StopPrice = double.Parse(o.sp),
				ExchangeId = o.i.ToString(),
				Symbol = o.s,
				LastUpdate = E
			};

	}

	public struct PositionUpdateDto
	{

		public string s;
		public string pa;
		public string ep;
		public string up;
		public string mt;
		public string iw;
		public string ps;

		internal Position Convert(long? updateTime)
			=> new Position
			{
				symbol = s,
				positionAmt = double.Parse(pa),
				entryPrice = double.Parse(ep),
				unRealizedProfit = double.Parse(up),
				marginType = mt,
				isolatedMargin = double.Parse(iw),
				positionSide = (PositionSide) Enum.Parse(typeof(PositionSide), ps, true),
				lastUpdate = updateTime
			};

	}

	public struct BalanceUpdateDto
	{

		public string a;
		public string wb;
		public string cw;
		public string bc;

		public Balance Convert(long? updateTime)
			=> new Balance
			{
				Value = double.Parse(wb),
				Currency = a,
				BalanceChange = bc,
				lastUpdate = updateTime ?? default,
			};

	}

	public struct BalanceDTO
	{

		public string accountAlias;
		public string asset;
		public string balance;
		public string crossWalletBalance;
		public string crossUnPnl;
		public string availableBalance;
		public string maxWithdrawAmount;
		public bool? marginAvailable;
		public long? updateTime;

		public Balance Convert()
			=> new Balance
			{
				Value = double.Parse(balance),
				Currency = asset
			};

	}

	public struct BinanceAssetDTO
	{

		public string asset;
		public string walletBalance;
		public string unrealizedProfit;
		public string marginBalance;
		public string maintMargin;
		public string initialMargin;
		public string positionInitialMargin;
		public string openOrderInitialMargin;
		public string crossWalletBalance;
		public string crossUnPnl;
		public string availableBalance;
		public string maxWithdrawAmount;
		public bool? marginAvailable;
		public long? updateTime;

		public Balance ConvertToBalance()
		{
			var _balance = new Balance
			{
				Value = double.Parse(walletBalance),
				Currency = asset
			};

			return _balance;
		}

	}

}
