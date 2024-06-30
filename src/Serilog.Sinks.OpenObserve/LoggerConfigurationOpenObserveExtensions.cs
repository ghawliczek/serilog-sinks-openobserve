namespace Serilog.Sinks.OpenObserve;

using System.Text;
using Configuration;
using Core.Api.Abstractions;
using Core.Configs;
using Core.Formatters;
using PeriodicBatching;
using Refit;
using Sinks.OpenObserve;

/// <summary>
///     Contains OpenObserve Serilog sink configuration extensions.
/// </summary>
public static class LoggerConfigurationOpenObserveExtensions
{
    /// <summary>
    ///     Registers OpenObserve sink.
    /// </summary>
    /// <param name="loggerConfiguration">The logger sink configuration.</param>
    /// <param name="apiUrl">The OpenObserve API base URL.</param>
    /// <param name="organization">The OpenObserve organization name.</param>
    /// <param name="username">The OpenObserve username.</param>
    /// <param name="key">The OpenObserve API key.</param>
    /// <param name="streamName">The stream name.</param>
    /// <returns></returns>
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
            Organization = organization,
            StreamName = streamName
        };

        var sink = new OpenObserveSink(
            configuration,
            RestService.For<IOpenObserveApi>(
                apiUrl,
                new RefitSettings
                {
                    AuthorizationHeaderValueGetter = (_, _) =>
                        Task.FromResult(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{key}")))
                }),
            new LogEventFormatter());

        return loggerConfiguration.Sink(new PeriodicBatchingSink(sink, new PeriodicBatchingSinkOptions()));
    }
}
