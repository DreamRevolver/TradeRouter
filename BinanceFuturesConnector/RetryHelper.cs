using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Shared.Interfaces;
using SharedBinance.Models;

namespace BinanceFuturesConnector
{
    public static class RetryHelper
    {
        private static ILogger _logger;
        private static int _attempts;
        private static int _delay;
        public static void Set(ILogger logger, string att, string del)
        {
            _logger = logger;
            Int32.TryParse(att, out _attempts);
            Int32.TryParse(del, out _delay);
        }
        public static async Task<BinanceResponseDto<OrderDTO>> ExecuteRetry(Func<Task<HttpResponseMessage>> action, Order order, string name)
        {
            var attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    _logger.Log(LogPriority.Info, $"--> ORDER | NEW | {order.Symbol} | {order.OrderSide} | {order.PositionSide} | {order.OrderType} | {order.Price} | {order.Amount} | #{order.ClientId}", name);
                    _logger.Log(LogPriority.BinanceWeight, $"SendNewOrder - Weight: 1", name);
                    var response = await action().ConfigureAwait(false);
                    if (response == null)
                    {
                        continue;
                    }
                    var content = await response.Content.ReadAsStringAsync();
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Forbidden:
                        case HttpStatusCode.Unauthorized:
                        case HttpStatusCode.BadGateway:
                        case HttpStatusCode.ProxyAuthenticationRequired:
                        case HttpStatusCode.UpgradeRequired:
                        case HttpStatusCode.MovedPermanently:
                        case HttpStatusCode.NotAcceptable:
                        case HttpStatusCode.NotFound:
                        case HttpStatusCode.RedirectMethod:
                        case HttpStatusCode.MethodNotAllowed:
                        case HttpStatusCode.PaymentRequired:
                            throw new Exception($"Order send unsuccessfull. HttpStatusCode {response.StatusCode} | Messege {content}");
                        case HttpStatusCode.OK:
                        case HttpStatusCode.BadRequest:
                            var result = new BinanceResponseDto<OrderDTO>(content);
                            if (result.error is { } _error && NeedRetry(_error))
                            {
                                throw new BinanceException(result.error);
                            }
                            else
                            {
                                return result;
                            }
                        default:
                            throw new BinanceException(response.StatusCode);
                    }
                }
                catch (BinanceException bexc)
                {
                    _logger.Log(LogPriority.Error, bexc, name, $"<-- ORDER | ORDER REJECTED | #{order.ExchangeId} / #{order.ClientId} | Trying to retry, {attempts} attempt | REJECT REASON: ");
                    Task.Delay(_delay).Wait();
                }
            } while (_attempts != attempts);
            throw new Exception($"Order #{order.ExchangeId} / #{order.ClientId} resend unsuccessfull. Number of attempts {attempts}");
        }

        private static bool NeedRetry(BinanceErrorDto error)
        {
            switch (error.code.value)
            {
                case ErrorCode.UNKNOWN:
                case ErrorCode.DISCONNECTED:
                case ErrorCode.INVALID_TIMESTAMP:
                    return true;
                default:
                    return false;
            }
        }
    }
}
