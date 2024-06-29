namespace Serilog.Sinks.OpenObserve.Tests.Core.Formatters;

using System.Globalization;
using Events;
using OpenObserve.Core.Formatters;

internal sealed class LogEventFormatterTests
{
    private readonly LogEvent _logEvent = new(
        DateTimeOffset.Parse("2024-06-29T22:22:50.2359921Z", CultureInfo.InvariantCulture),
        LogEventLevel.Information,
        null,
        new MessageTemplate([]),
        [
            new LogEventProperty("prop1", new ScalarValue("value1")),
            new LogEventProperty("prop2", new ScalarValue(123)),
            new LogEventProperty("prop3", new ScalarValue(false))
        ]);

    private LogEntryFormatter _formatter = null!;

    [SetUp]
    public void Setup() => _formatter = new LogEntryFormatter();

    [Test]
    public void Format_ShouldThrowArgumentException_WhenLogEventIsNull() =>
        Assert.Throws<ArgumentNullException>(() => _formatter.Format(null!, new StringWriter()));

    [Test]
    public void Format_ShouldThrowArgumentException_WhenTextWriterIsNull() =>
        Assert.Throws<ArgumentNullException>(() => _formatter.Format(_logEvent, null!));

    [Test]
    public void Format_ShouldCreateValidJsonString_WhenGivenValidEvent()
    {
        const string expected = "{\"_timestamp\":\"2024-06-29T22:22:50.2359921Z\",\"_message\":\"\",\"_template\"" +
                                ":\"\",\"_id\":\"00000000\",\"_level\":\"Information\",\"prop1\":\"value1\",\"prop2\"" +
                                ":123,\"prop3\":false}\n";
        var writer = new StringWriter();

        _formatter.Format(_logEvent, writer);

        Assert.That(writer.ToString(), Is.EqualTo(expected));
    }
}
