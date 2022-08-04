using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Core.RecordingLoggerParts;
using WebRouterApp.Shared.Collections;

namespace WebRouterApp.Features.Logs.Application.UseCases
{
    public sealed class LogsApiQuery : IApiRequest<IReadOnlyList<LogRecordModel>>
    {
        public long? FromTime { get; set;}
        public long? ToTime { get; set; }
    }

    public sealed record LogRecordModel(
        string Priority, 
        string Message, 
        string Source, 
        // Unix time (also known as POSIX time or epoch time) is a system for describing instants in time,
        // defined as the number of seconds that have elapsed since
        // 00:00:00 *Coordinated Universal Time (UTC)*, Thursday, 1 January 1970, not counting leap seconds.
        long UnixSeconds);

    public class LogsApiQueryHandler : IApiRequestHandler<LogsApiQuery, IReadOnlyList<LogRecordModel>>
    {
        private readonly RecordingLogger _logger;

        public LogsApiQueryHandler(RecordingLogger logger)
        {
            _logger = logger;
        }

        public async Task<IReadOnlyList<LogRecordModel>> Handle(LogsApiQuery query)
        {
            return (await _logger.LoadAsync(query.FromTime, query.ToTime))
                .Select(it => new LogRecordModel(
                    Priority: it.Priority, 
                    Message: it.Message, 
                    Source: it.Source, 
                    UnixSeconds: new DateTimeOffset(it.Ticks, TimeSpan.Zero).ToUnixTimeSeconds())
                )
                .ToReadOnlyList();
        }
    }
}
