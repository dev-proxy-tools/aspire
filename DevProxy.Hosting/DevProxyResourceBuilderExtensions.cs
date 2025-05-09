using Aspire.Hosting.ApplicationModel;
using DevProxy.Hosting;

#pragma warning disable IDE0130
namespace Aspire.Hosting;
#pragma warning restore IDE0130

/// <summary>
/// Provides extension methods for adding DevProxy resources to a distributed application builder.
/// </summary>
public static class DevProxyResourceBuilderExtensions
{
    /// <summary>
    /// Adds a DevProxy executable resource to the distributed application builder.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the DevProxy executable resource.</param>
    /// <returns>A resource builder for the DevProxy executable resource.</returns>
    public static IResourceBuilder<DevProxyExecutableResource> AddDevProxyExecutable(
        this IDistributedApplicationBuilder builder,
        string name)
    {
        var resource = new DevProxyExecutableResource(name);

        return builder.AddResource(resource)
            .WithArgs("--as-system-proxy", "false")
            .WithHttpEndpoint(targetPort: 8000, name: DevProxyResource.ProxyEndpointName)
            .WithHttpEndpoint(targetPort: 8897, name: DevProxyResource.ApiEndpointName);
    }

    /// <summary>
    /// Adds a DevProxy container resource to the distributed application builder.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the DevProxy container resource.</param>
    /// <returns>A resource builder for the DevProxy container resource.</returns>
    public static IResourceBuilder<DevProxyContainerResource> AddDevProxyContainer(
        this IDistributedApplicationBuilder builder,
        string name)
    {
        var resource = new DevProxyContainerResource(name);

        return builder.AddResource(resource)
            .WithImage(DevProxyContainerImageTags.Image)
            .WithImageRegistry(DevProxyContainerImageTags.Registry)
            .WithImageTag(DevProxyContainerImageTags.Tag)
            .WithHttpEndpoint(targetPort: 8000, name: DevProxyResource.ProxyEndpointName)
            .WithHttpEndpoint(targetPort: 8897, name: DevProxyResource.ApiEndpointName);
    }
}

internal static class DevProxyContainerImageTags
{
    internal const string Registry = "ghcr.io";

    internal const string Image = "dotnet/dev-proxy";

    internal const string Tag = "latest";
}