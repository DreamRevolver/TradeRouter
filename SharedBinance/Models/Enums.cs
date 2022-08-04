using System;

namespace Shared.Models
{
    public enum CustomerRole
    {
        Subscriber,
        Publisher
    }

    public enum WalletCurrency
    { 
        USDT
    }
    public enum UpdateAction
    {
        New = 0,
        Update = 1,
        Delete = 2,
    }
    public enum MarginType
    {
        Isolated,
        Crossed
    }

    public enum PositionSide
    {
        BOTH,
        LONG,
        SHORT
    }
    public enum ContractType
    {
        PERPETUAL,
        CURRENT_MONTH,
        NEXT_MONTH,
        CURRENT_QUARTER,
        NEXT_QUARTER
    }
    public enum EntryType
    {
        Bid = '0',
        Offer = '1',
        Trade = '2',
    }

    public enum ConnectionEvent
    {
        Started,
        Stopped,
        ConnectionFault,
        Logon,
        Logout
    }
    public enum OrderType
    {
        LIMIT,
        MARKET,
        STOP,
        STOP_MARKET,
        TAKE_PROFIT,
        TAKE_PROFIT_MARKET,
        TRAILING_STOP_MARKET,
        LIQUIDATION
    }
    public enum OrderExecutionType
    {
        NEW,
        CANCELED,
        CALCULATED,
        EXPIRED,
        TRADE
    }
    public enum OrderSide
    {
        None = '0',
        BUY = '1',
        SELL = '2',
    }
    public enum UpdateEventState
    {
        OPEN,
        CLOSED,
        CHECKED
    }
    [Flags]
    public enum OrderStatus
    {
        NEW = 1 << 0,
        PARTIALLY_FILLED = 1 << 1,
        FILLED = 1 << 2,
        CANCELED = 1 << 3,
        EXPIRED = 1 << 4,
        NEW_INSURANCE = 1 << 5,
        NEW_ADL = 1 << 6,
        UNDEFINED = 1 << 7
    }
    public enum OrderTimeInForce
    {
        GTC,
        IOC,
        FOK,
        GTX
    }
    public enum OrderWorkingType
    {
        MARK_PRICE,
        CONTRACT_PRICE
    }
    public enum OrderSortField
    {
        None,
        Symbol,
        Side,
        Status,
        Id,
        ClientId,
        Price,
        Qty
    }
    public enum PositionSortField 
    {
        none,
        symbol,
        entryPrice,
        leverage,
        liquidationPrice,
        positionAmt,
        unrealizedProfit,
        positionSide
    }

}
