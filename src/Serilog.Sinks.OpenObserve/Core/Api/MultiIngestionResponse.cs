namespace Serilog.Sinks.OpenObserve.Core.Api;

/// <summary>
///     Represents the ingestion response from _multi endpoint.
/// </summary>
internal sealed class MultiIngestionResponse
{
    /// <summary>
    ///     Gets the code.
    /// </summary>
    public int Code { get; init; }

    /// <summary>
    ///     Gets the error.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    ///     Gets the status.
    /// </summary>
    public IEnumerable<IngestionStatus>? Status { get; init; }

    /// <summary>
    ///     Gets the error detail.
    /// </summary>
    public string? ErrorDetail { get; init; }

    /// <summary>
    ///     Gets the message.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    ///     Gets the flag indicating a successful response.
    /// </summary>
    public bool IsSuccessful => Code == 200;
}
