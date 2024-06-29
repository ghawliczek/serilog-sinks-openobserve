namespace Serilog.Sinks.OpenObserve.Core.Configs;

/// <summary>
///     Represents the OpenObserve sink configuration.
/// </summary>
internal sealed class OpenObserveSinkConfiguration
{
    /// <summary>
    ///     Gets the organization name.
    /// </summary>
    public string Organization { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the stream name.
    /// </summary>
    public string StreamName { get; init; } = string.Empty;
}
