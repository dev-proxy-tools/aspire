using Aspire.Hosting.ApplicationModel;

namespace DevProxy.Hosting;

/// <summary>
/// Exposes Dev Proxy as a Docker container.
/// </summary>
public sealed class DevProxyContainerResource(string name):
    ContainerResource(name)
{
}
