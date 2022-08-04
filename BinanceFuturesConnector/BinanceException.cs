using System;
using System.Net;
using BinanceFuturesConnector;

namespace SharedBinance.Models
{
    public class BinanceException: Exception
    {
        public HttpStatusCode StatusCode {get; private set;}
        public ErrorCodeWrapper? code {get; private set;}
        public string msg {get; private set;}
        public BinanceException()
        {
        }
        public BinanceException(HttpStatusCode errorCode)
            : base($"Binance HTTP error: {errorCode}")
        {
            StatusCode = errorCode;
        }
        public BinanceException(BinanceErrorDto? data)
            : base($"Binance error: {data?.msg} | code: {data?.code.ToString()}")
        {
            code = data?.code;
            msg = data?.msg;
        }

        public BinanceException(BinanceErrorDto? data, Exception inner)
            : base($"Binance error: {data?.msg} | code: {data?.code.ToString()}", inner)
        {
            code = data?.code;
            msg = data?.msg;
        }
    }
}
