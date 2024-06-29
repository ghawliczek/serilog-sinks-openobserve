namespace Serilog.Sinks.OpenObserve.Contracts.Exceptions;

public sealed class FailedEventsIngestionException(string? message, Exception? innerException = null)
    : Exception(message, innerException);
