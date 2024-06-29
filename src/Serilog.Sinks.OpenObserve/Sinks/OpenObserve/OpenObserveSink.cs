namespace Serilog.Sinks.OpenObserve.Sinks.OpenObserve;

using Contracts.Exceptions;
using Core.Abstractions;
using Events;
using Formatting;
using PeriodicBatching;

internal sealed class OpenObserveSink(IOpenObserveApiClient apiClient, ITextFormatter formatter) : IBatchedLogEventSink
{
    public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
    {
        var payload = new StringWriter();

        foreach (var logEvent in batch)
        {
            formatter.Format(logEvent, payload);
        }

        var result = await apiClient.SendEventsAsync(payload.ToString());

        if (result is not { IsSuccessful: true })
        {
            throw new FailedEventsIngestionException(result?.Error ?? result?.ErrorDetail ?? result?.Message ?? "Unknown error");
        }
    }

    public Task OnEmptyBatchAsync() => Task.CompletedTask;
}
