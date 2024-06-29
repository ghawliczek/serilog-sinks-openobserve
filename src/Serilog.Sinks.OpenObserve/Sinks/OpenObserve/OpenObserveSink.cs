namespace Serilog.Sinks.OpenObserve.Sinks.OpenObserve;

using System.Net;
using Core.Api.Abstractions;
using Core.Configs;
using Events;
using Formatting;
using PeriodicBatching;

/// <summary>
///     Represents the OpenObserve Serilog sink.
/// </summary>
/// <param name="configuration">The sink configuration.</param>
/// <param name="api">The OpenObserve API client.</param>
/// <param name="formatter">The text formatter.</param>
internal sealed class OpenObserveSink(
    OpenObserveSinkConfiguration configuration,
    IOpenObserveApi api,
    ITextFormatter formatter)
    : IBatchedLogEventSink
{
    /// <inheritdoc />
    public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
    {
        var payload = new StringWriter();

        foreach (var logEvent in batch)
        {
            formatter.Format(logEvent, payload);
        }

        var result = await api.SendEventsAsync(configuration.Organization, configuration.StreamName, payload.ToString());

        if (result is not { IsSuccessful: true })
        {
            throw new WebException(result.Error ?? result.ErrorDetail ?? result.Message ?? "Unknown error");
        }
    }

    /// <inheritdoc />
    public Task OnEmptyBatchAsync() => Task.CompletedTask;
}
