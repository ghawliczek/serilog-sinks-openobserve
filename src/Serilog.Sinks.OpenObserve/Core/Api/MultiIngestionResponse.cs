namespace Serilog.Sinks.OpenObserve.Core.Api;

internal sealed class MultiIngestionResponse
{
    public int Code { get; init; }

    public string? Error { get; init; }

    public IEnumerable<IngestionStatus>? Status { get; init; }

    public string? ErrorDetail { get; init; }

    public string? Message { get; init; }

    public bool IsSuccessful => Code == 200;
}
