namespace Serilog.Sinks.OpenObserve.Tests.Sinks.OpenObserve;

using System.Net;
using Events;
using Formatting;
using NSubstitute;
using Serilog.Sinks.OpenObserve.Core.Api;
using Serilog.Sinks.OpenObserve.Core.Api.Abstractions;
using Serilog.Sinks.OpenObserve.Core.Configs;
using Serilog.Sinks.OpenObserve.Core.Formatters;
using Serilog.Sinks.OpenObserve.Sinks.OpenObserve;

internal sealed class OpenObserveSinkTests
{
    private readonly LogEvent _logEvent = new(
        DateTimeOffset.Now,
        LogEventLevel.Information,
        null,
        new MessageTemplate([]),
        []);

    private IOpenObserveApi _api = null!;
    private OpenObserveSinkConfiguration _configuration = null!;
    private ITextFormatter _formatter = null!;
    private OpenObserveSink _sink = null!;

    [SetUp]
    public void Setup()
    {
        _configuration = new OpenObserveSinkConfiguration
        {
            Organization = "org1",
            StreamName = "stream1"
        };
        _api = Substitute.For<IOpenObserveApi>();
        _api.SendEventsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(new MultiIngestionResponse { Code = 200 }));

        _formatter = Substitute.For<ITextFormatter>();

        _sink = new OpenObserveSink(_configuration, _api, _formatter);
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(69)]
    public async Task EmitBatchAsync_ShouldFormatEachEvent(int numberOfEvents)
    {
        await _sink.EmitBatchAsync(Enumerable.Range(0, numberOfEvents).Select(_ => _logEvent));

        _formatter.Received(numberOfEvents).Format(Arg.Any<LogEvent>(), Arg.Any<TextWriter>());
    }

    [Test]
    public async Task EmitBatchAsync_ShouldSendEventsToApi()
    {
        await _sink.EmitBatchAsync([_logEvent]);

        await _api.Received(1)
            .SendEventsAsync(
                Arg.Is<string>(arg => arg == _configuration.Organization),
                Arg.Is<string>(arg => arg == _configuration.StreamName),
                Arg.Any<string>());
    }

    [Test]
    public async Task EmitBatchAsync_ShouldUseEventsPayloadConstructedUsingFormatter()
    {
        var formatter = new LogEntryFormatter();
        var expectedPayload = new StringWriter();
        formatter.Format(_logEvent, expectedPayload);
        formatter.Format(_logEvent, expectedPayload);

        _sink = new OpenObserveSink(_configuration, _api, formatter);

        await _sink.EmitBatchAsync([_logEvent, _logEvent]);

        await _api.Received(1)
            .SendEventsAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Is<string>(arg => arg == expectedPayload.ToString()));
    }

    [Test]
    public void EmitBatchAsync_ShouldThrowWebException_WhenApiReturnedUnsuccessfulResponse()
    {
        _api.SendEventsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(new MultiIngestionResponse { Code = 400 }));

        Assert.ThrowsAsync<WebException>(async () => await _sink.EmitBatchAsync([_logEvent]));
    }
}
