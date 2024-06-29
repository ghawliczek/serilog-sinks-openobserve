namespace Serilog.Sinks.OpenObserve;

using Configuration;
using Core.Clients;
using Core.Configs;
using Core.Formatters;
using PeriodicBatching;
using Sinks.OpenObserve;

public static class LoggerConfigurationOpenObserveExtensions
{
    public static LoggerConfiguration OpenObserve(
        this LoggerSinkConfiguration loggerConfiguration,
        string apiUrl,
        string organization,
        string username,
        string key,
        string streamName = "default")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(organization);
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var configuration = new OpenObserveSinkConfiguration
        {
            ApiUrl = apiUrl,
            Organization = organization,
            Username = username,
            Key = key,
            StreamName = streamName
        };

        var sink = new OpenObserveSink(new OpenObserveApiClient(configuration), new LogEntryFormatter());

        return loggerConfiguration.Sink(new PeriodicBatchingSink(sink, new PeriodicBatchingSinkOptions()));
    }
}
