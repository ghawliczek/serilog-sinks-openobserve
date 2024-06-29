namespace Serilog.Sinks.OpenObserve.Core.Clients;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Abstractions;
using Api;
using Configs;
using Utils;

internal sealed class OpenObserveApiClient(OpenObserveSinkConfiguration configuration) : IOpenObserveApiClient, IDisposable
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(configuration.ApiUrl),
        DefaultRequestHeaders =
        {
            Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{configuration.Username}:{configuration.Key}")))
        }
    };

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    private string RequestUri =>
        $"/api/{configuration.Organization.TrimForwardSlashes()}/{configuration.StreamName.TrimForwardSlashes()}/_multi";

    public void Dispose() => _httpClient.Dispose();

    public async Task<MultiIngestionResponse?> SendEventsAsync(string payload, CancellationToken cancellationToken = default)
    {
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(RequestUri, content, cancellationToken);

        return await JsonSerializer.DeserializeAsync<MultiIngestionResponse>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            _jsonSerializerOptions,
            cancellationToken);
    }
}
