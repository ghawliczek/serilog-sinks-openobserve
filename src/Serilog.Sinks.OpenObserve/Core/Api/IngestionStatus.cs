namespace Serilog.Sinks.OpenObserve.Core.Api;

internal sealed class IngestionStatus
{
    public string? Error { get; init; }

    public int Failed { get; init; }

    public int Successful { get; init; }

    public string? Name { get; init; }
}
