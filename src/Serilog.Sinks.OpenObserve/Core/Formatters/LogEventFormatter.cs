namespace Serilog.Sinks.OpenObserve.Core.Formatters;

using System.Globalization;
using Events;
using Expressions.Compilation.Linq;
using Formatting;
using Formatting.Json;

/// <summary>
///     Represents the log entry formatter.
/// </summary>
internal sealed class LogEntryFormatter : ITextFormatter
{
    private readonly JsonValueFormatter _jsonValueFormatter = new();

    /// <inheritdoc />
    public void Format(LogEvent logEvent, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(output);

        output.Write('{');

        output.Write("\"_timestamp\":");
        output.Write($"\"{logEvent.Timestamp.UtcDateTime:O}\"");

        output.Write(",\"_message\":");
        JsonValueFormatter.WriteQuotedJsonString(
            logEvent.MessageTemplate.Render(logEvent.Properties, CultureInfo.InvariantCulture),
            output);

        output.Write(",\"_template\":");
        JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);

        output.Write(",\"_id\":");
        output.Write($"\"{EventIdHash.Compute(logEvent.MessageTemplate.Text):x8}\"");

        output.Write(",\"_level\":");
        output.Write($"\"{logEvent.Level}\"");

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

        output.Write("}\n");
    }
}
