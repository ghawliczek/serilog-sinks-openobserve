namespace Serilog.Sinks.OpenObserve.Core.Formatters;

using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using Events;
using Expressions.Compilation.Linq;
using Formatting;
using Formatting.Json;
using Parsing;

/// <summary>
///     Represents the log entry formatter.
/// </summary>
internal sealed class LogEventFormatter : ITextFormatter
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
        RenderMessageTemplateWithoutDoubleQuotes(logEvent, output);

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

            if (logEvent.Properties.TryGetValue(name, out var propertyValue) &&
                propertyValue is ScalarValue { Value: string str })
            {
                output.Write($"\"{JsonEncodedText.Encode(str, JavaScriptEncoder.UnsafeRelaxedJsonEscaping)}\"");
                continue;
            }

            _jsonValueFormatter.Format(property.Value, output);
        }

        output.Write("}");
        output.WriteLine();
    }

    private static void RenderMessageTemplateWithoutDoubleQuotes(LogEvent logEvent, TextWriter output)
    {
        output.Write('"');

        foreach (var messageTemplateToken in logEvent.MessageTemplate.Tokens)
        {
            if (messageTemplateToken is TextToken textToken)
            {
                output.Write(textToken.Text);
                continue;
            }

            if (messageTemplateToken is PropertyToken propertyToken)
            {
                if (logEvent.Properties.TryGetValue(propertyToken.PropertyName, out var propertyValue) &&
                    propertyValue is ScalarValue { Value: string str })
                {
                    output.Write(JsonEncodedText.Encode(str, JavaScriptEncoder.UnsafeRelaxedJsonEscaping).ToString());
                    continue;
                }

                propertyToken.Render(logEvent.Properties, output, CultureInfo.InvariantCulture);
            }
        }

        output.Write('"');
    }
}
