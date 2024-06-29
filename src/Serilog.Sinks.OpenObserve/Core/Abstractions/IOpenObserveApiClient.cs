namespace Serilog.Sinks.OpenObserve.Core.Abstractions;

using Api;

internal interface IOpenObserveApiClient
{
    Task<MultiIngestionResponse?> SendEventsAsync(string payload, CancellationToken cancellationToken = default);
}
