namespace Serilog.Sinks.OpenObserve.Core.Configs;

internal sealed class OpenObserveSinkConfiguration
{
    public string ApiUrl { get; init; } = string.Empty;

    public string Organization { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string Key { get; init; } = string.Empty;

    public string StreamName { get; init; } = string.Empty;
}
