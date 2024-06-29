namespace Serilog.Sinks.OpenObserve.Utils;

internal static class StringUtils
{
    public static string TrimForwardSlashes(this string str) => str.Trim('/');
}
