namespace Serilog.Sinks.OpenObserve.Tests.Core.Formatters;

using System.Globalization;
using Events;
using OpenObserve.Core.Formatters;
using Parsing;

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

    private LogEventFormatter _formatter = null!;

    [SetUp]
    public void Setup() => _formatter = new LogEventFormatter();

    [Test]
    public void Format_ShouldThrowArgumentException_WhenLogEventIsNull() =>
        Assert.Throws<ArgumentNullException>(() => _formatter.Format(null!, new StringWriter()));

    [Test]
    public void Format_ShouldThrowArgumentException_WhenTextWriterIsNull() =>
        Assert.Throws<ArgumentNullException>(() => _formatter.Format(_logEvent, null!));

    [Test]
    public void Format_ShouldCreateValidJsonString()
    {
        const string expected =
            "{" +
            "\"_timestamp\":\"2024-06-29T22:22:50.2359921Z\"" +
            ",\"_message\":\"\"" +
            ",\"_template\":\"\"" +
            ",\"_id\":\"00000000\"" +
            ",\"_level\":\"Information\"" +
            ",\"prop1\":\"value1\"" +
            ",\"prop2\":123" +
            ",\"prop3\":false" +
            "}\r\n";

        var writer = new StringWriter();

        _formatter.Format(_logEvent, writer);

        Assert.That(writer.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void Format_ShouldCorrectlyEscapeSpecialCharactersInMessage()
    {
        var messageTemplate = new MessageTemplate(
        [
            new TextToken("Test text token "),
            new PropertyToken("Property", "{Property}"),
            new TextToken(" .")
        ]);

        var logEvent = new LogEvent(
            DateTimeOffset.Parse("2024-06-29T22:22:50.2359921Z", CultureInfo.InvariantCulture),
            LogEventLevel.Information,
            null,
            messageTemplate,
            [new LogEventProperty("Property", new ScalarValue("!@#$%^&*() \" \\ / . ,"))]);

        const string expected =
            "{" +
            "\"_timestamp\":\"2024-06-29T22:22:50.2359921Z\"" +
            ",\"_message\":\"Test text token !@#$%^&*() \\\" \\\\ / . , .\"" +
            ",\"_template\":\"Test text token {Property} .\"" +
            ",\"_id\":\"f980cdba\"" +
            ",\"_level\":\"Information\"" +
            ",\"Property\":\"!@#$%^&*() \\\" \\\\ / . ,\"" +
            "}\r\n";

        var writer = new StringWriter();

        _formatter.Format(logEvent, writer);

        Assert.That(writer.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void Format_ShouldRenderStringPropertiesWithoutWrappingThemInDoubleQuotes()
    {
        var messageTemplate = new MessageTemplate(
        [
            new TextToken("Request finished "),
            new PropertyToken("Protocol", "{Protocol}"),
            new TextToken(" "),
            new PropertyToken("Method", "{Method}"),
            new TextToken(" "),
            new PropertyToken("Scheme", "{Scheme}"),
            new TextToken("://"),
            new PropertyToken("Host", "{Host}"),
            new PropertyToken("PathBase", "{PathBase}"),
            new PropertyToken("Path", "{Path}"),
            new PropertyToken("QueryString", "{QueryString}"),
            new TextToken(" - "),
            new PropertyToken("StatusCode", "{StatusCode}"),
            new TextToken(" "),
            new PropertyToken("ContentLength", "{ContentLength}"),
            new TextToken(" "),
            new PropertyToken("ContentType", "{ContentType}"),
            new TextToken(" "),
            new PropertyToken("ElapsedMilliseconds", "{ElapsedMilliseconds}"),
            new TextToken("ms")
        ]);

        var logEvent = new LogEvent(
            DateTimeOffset.Parse("2024-06-29T22:22:50.2359921Z", CultureInfo.InvariantCulture),
            LogEventLevel.Information,
            null,
            messageTemplate,
            [
                new LogEventProperty("Protocol", new ScalarValue("HTTP/1.1")),
                new LogEventProperty("Method", new ScalarValue("GET")),
                new LogEventProperty("Scheme", new ScalarValue("https")),
                new LogEventProperty("Host", new ScalarValue("localhost:8080")),
                new LogEventProperty("PathBase", new ScalarValue("")),
                new LogEventProperty("Path", new ScalarValue("/v1/resource/property/otherUrlSegment")),
                new LogEventProperty("QueryString", new ScalarValue("")),
                new LogEventProperty("StatusCode", new ScalarValue(301)),
                new LogEventProperty("ContentLength", new ScalarValue(0)),
                new LogEventProperty("ContentType", new ScalarValue(null)),
                new LogEventProperty("ElapsedMilliseconds", new ScalarValue(12.7291))
            ]);

        const string expected =
            "{" +
            "\"_timestamp\":\"2024-06-29T22:22:50.2359921Z\"" +
            ",\"_message\":\"Request finished HTTP/1.1 GET https://localhost:8080/v1/resource/property/otherUrlSegment - 301 0 null 12.7291ms\"" +
            ",\"_template\":\"Request finished {Protocol} {Method} {Scheme}://{Host}{PathBase}{Path}{QueryString} - {StatusCode} {ContentLength} {ContentType} {ElapsedMilliseconds}ms\"" +
            ",\"_id\":\"74253ad3\"" +
            ",\"_level\":\"Information\"" +
            ",\"Protocol\":\"HTTP/1.1\"" +
            ",\"Method\":\"GET\"" +
            ",\"Scheme\":\"https\"" +
            ",\"Host\":\"localhost:8080\"" +
            ",\"PathBase\":\"\"" +
            ",\"Path\":\"/v1/resource/property/otherUrlSegment\"" +
            ",\"QueryString\":\"\"" +
            ",\"StatusCode\":301" +
            ",\"ContentLength\":0" +
            ",\"ContentType\":null" +
            ",\"ElapsedMilliseconds\":12.7291" +
            "}\r\n";

        var writer = new StringWriter();

        _formatter.Format(logEvent, writer);

        Assert.That(writer.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void Format_ShouldCorrectlyEscapePaths()
    {
        var messageTemplate = new MessageTemplate(
        [
            new PropertyToken("Property", "{Property}")
        ]);

        var logEvent = new LogEvent(
            DateTimeOffset.Parse("2024-06-29T22:22:50.2359921Z", CultureInfo.InvariantCulture),
            LogEventLevel.Information,
            null,
            messageTemplate,
            [new LogEventProperty("Property", new ScalarValue(@"C:\Path\To\Something\Nice"))]);

        const string expected =
            "{" +
            "\"_timestamp\":\"2024-06-29T22:22:50.2359921Z\"" +
            ",\"_message\":\"C:\\\\Path\\\\To\\\\Something\\\\Nice\"" +
            ",\"_template\":\"{Property}\"" +
            ",\"_id\":\"2823c87b\"" +
            ",\"_level\":\"Information\"" +
            ",\"Property\":\"C:\\\\Path\\\\To\\\\Something\\\\Nice\"" +
            "}\r\n";

        var writer = new StringWriter();

        _formatter.Format(logEvent, writer);

        Assert.That(writer.ToString(), Is.EqualTo(expected));
    }
}
