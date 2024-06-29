namespace Serilog.Sinks.OpenObserve.Core.Formatters;

using System.Globalization;
using Events;
using Expressions.Compilation.Linq;
using Formatting;
using Formatting.Json;

internal sealed class LogEntryFormatter : ITextFormatter
{
    private readonly JsonValueFormatter _jsonValueFormatter = new();

    public void Format(LogEvent logEvent, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(output);

        output.Write("{\"_timestamp\":\"");
        output.Write(logEvent.Timestamp.UtcDateTime.ToString("O"));
        output.Write("\",\"_message\":");

        var message = logEvent.MessageTemplate.Render(logEvent.Properties, CultureInfo.InvariantCulture);
        JsonValueFormatter.WriteQuotedJsonString(message, output);

        output.Write(",\"_template\":");
        JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);

        output.Write(",\"_id\":\"");
        var id = EventIdHash.Compute(logEvent.MessageTemplate.Text);
        output.Write(id.ToString("x8", CultureInfo.InvariantCulture));

        output.Write('"');
        output.Write(",\"_level\":\"");
        output.Write(logEvent.Level);
        output.Write('\"');

        if (logEvent.Exception != null)
        {
            output.Write(",\"_exception\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
        }

        foreach (var property in logEvent.Properties)
        {
            var name = property.Key;
            if (name.Length > 0 && name[0] == '@')
            {
                name = '@' + name;
            }

            output.Write(',');
            JsonValueFormatter.WriteQuotedJsonString(name, output);
            output.Write(':');
            _jsonValueFormatter.Format(property.Value, output);
        }

        output.Write('}');
        output.Write('\n');
    }
}
