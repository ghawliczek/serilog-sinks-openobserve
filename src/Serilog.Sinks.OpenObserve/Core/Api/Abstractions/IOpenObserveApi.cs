namespace Serilog.Sinks.OpenObserve.Core.Api.Abstractions;

using Refit;

/// <summary>
///     Represents OpenObserve API client interface.
/// </summary>
[Headers("Authorization: Basic")]
internal interface IOpenObserveApi
{
    /// <summary>
    ///     Sends string-represented events to the specified stream within the specified organization.
    /// </summary>
    /// <param name="organization">The OpenObserve organization.</param>
    /// <param name="stream">The stream within the organization.</param>
    /// <param name="payload">The string representation of log events.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response representing successful or failed logs ingestion.</returns>
    [Post("/api/{organization}/{stream}/_multi")]
    Task<MultiIngestionResponse> SendEventsAsync(
        string organization,
        string stream,
        [Body] string payload,
        CancellationToken cancellationToken = default);
}
