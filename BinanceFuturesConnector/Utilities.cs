using System;
using System.Security.Cryptography;
using System.Text;

using Shared.Models;
using SharedBinance.Models;
namespace BinanceFuturesConnector
{
    public static class Utilities
    {
        public static string BinanceSymbol(Order order)
            => order.Symbol;

        public static string BinanceSymbol(Instrument instr)
            => instr.Symbol;

        public static string EndpointSymbol(Instrument instr)
            => $"{instr.Symbol}".ToLower();

        public static string GenerateSignature(string apiSecret, string message)
        {
            var key = Encoding.UTF8.GetBytes(apiSecret);
            string stringHash;
            using (var hmac = new HMACSHA256(key))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                stringHash = BitConverter.ToString(hash).Replace("-", "");
            }

            return stringHash;
        }

        public static string GenerateTimeStamp(DateTime baseDateTime)
            => new DateTimeOffset(baseDateTime).ToUnixTimeMilliseconds().ToString();

        public static string BinanceOrder(Order order)
        {

            var result = $"symbol={BinanceSymbol(order)}&side={order.OrderSide.ToString().ToUpper()}&type={order.OrderType.ToString().ToUpper()}&positionSide={order.PositionSide.ToString().ToUpper()}";

            if (order.OrderType == OrderType.MARKET)
            {
                result += $"&quantity={order.Amount.ToString()}";
            }

            if (order.OrderType == OrderType.LIMIT)
            {
                result += $"&price={((decimal)order.Price).ToString()}&quantity={order.Amount.ToString()}&timeInForce=GTC";
            }

            if (order.OrderType == OrderType.STOP || order.OrderType == OrderType.TAKE_PROFIT)
            {
                result += $"&price={((decimal)order.Price).ToString()}&quantity={order.Amount.ToString()}&stopPrice={((decimal)order.StopPrice).ToString()}";
            }

            if (order.OrderType == OrderType.STOP_MARKET || order.OrderType == OrderType.TAKE_PROFIT_MARKET)
            {
                result += $"&stopPrice={((decimal)order.StopPrice).ToString()}&closePosition={order.ClosePosition}";
                if (!order.ClosePosition)
                {
                    result += $"&quantity={order.Amount.ToString()}";
                }
            }
            result += $"&newClientOrderId={order.ClientId}";
            return result;
        }

        public static OrderStatus ParseOrderStatus(string type)
            => Enum.TryParse(type, out OrderStatus result) ? result : OrderStatus.UNDEFINED;

        public static int BinanceLvl(int level)
        {
            if (level > 0 && level <= 5)
            {
                return 5;
            }
            if (level > 5 && level <= 10)
            {
                return 10;
            }
            if (level > 10 && level <= 20)
            {
                return 20;
            }
            if (level > 20 && level <= 100)
            {
                return 100;
            }
            if (level > 100 && level <= 500)
            {
                return 500;
            }
            if (level > 500)
            {
                return 1000;
            }
            return level;
        }

        public static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddMilliseconds(timestamp);
        }

    }
}
