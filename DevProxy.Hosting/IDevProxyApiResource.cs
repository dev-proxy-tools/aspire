using Aspire.Hosting.ApplicationModel;

namespace DevProxy.Hosting;

/// <summary>
/// A Dev Proxy resource exposing its management API.
/// </summary>
public interface IDevProxyApiResource : IResourceWithEndpoints
{
	/// <summary>
	/// Gets the directory containing this resource's Dev Proxy API token files.
	/// </summary>
	string ApiTokenDirectory { get; }

	/// <summary>
	/// Gets the API token that authenticates against this resource's API endpoint.
	/// </summary>
	ValueTask<string> GetApiKeyAsync(Uri apiEndpoint, CancellationToken cancellationToken = default) =>
		DevProxyApiTokenStore.GetApiKeyAsync(ApiTokenDirectory, apiEndpoint, cancellationToken);
}