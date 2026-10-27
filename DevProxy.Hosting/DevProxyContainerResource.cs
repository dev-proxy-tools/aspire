using Aspire.Hosting.ApplicationModel;

namespace DevProxy.Hosting;

/// <summary>
/// Exposes Dev Proxy as a Docker container.
/// </summary>
public sealed class DevProxyContainerResource : ContainerResource, IDevProxyApiResource
{
    private readonly string _apiTokenDirectory;

    /// <summary>
    /// Initializes a container resource with an isolated temporary API-token directory.
    /// </summary>
    public DevProxyContainerResource(string name)
        : this(name, DevProxyApiTokenStore.CreateTemporaryTokenDirectory())
    {
    }

    internal DevProxyContainerResource(string name, string apiConfigDirectory)
        : base(name)
    {
        _apiTokenDirectory = apiConfigDirectory;
        Directory.CreateDirectory(_apiTokenDirectory);
    }

    /// <inheritdoc />
    public string ApiTokenDirectory => _apiTokenDirectory;
}