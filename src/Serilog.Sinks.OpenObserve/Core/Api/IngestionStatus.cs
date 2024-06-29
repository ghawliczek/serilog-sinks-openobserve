namespace Serilog.Sinks.OpenObserve.Core.Api;

/// <summary>
///     Represents the ingestion status model.
/// </summary>
internal sealed class IngestionStatus
{
    /// <summary>
    ///     Gets the error.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    ///     Gets the number of failed messages.
    /// </summary>
    public int Failed { get; init; }

    /// <summary>
    ///     Gets the number of successful messages.
    /// </summary>
    public int Successful { get; init; }

    /// <summary>
    ///     Gets the stream name.
    /// </summary>
    public string? Name { get; init; }
}
