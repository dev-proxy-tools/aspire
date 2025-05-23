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

        var resourceBuilder = builder.AddResource(resource)
            .WithArgs("--as-system-proxy", "false")
            .WithHttpEndpoint(targetPort: 8000, name: DevProxyResource.ProxyEndpointName)
            .WithHttpEndpoint(targetPort: 8897, name: DevProxyResource.ApiEndpointName);

        return resourceBuilder;
    }

    /// <summary>
    /// Configures the DevProxy executable to use a specific configuration file.
    /// </summary>
    /// <param name="builder">The resource builder for the DevProxy executable resource.</param>
    /// <param name="configFile">The path to the configuration file.</param>
    /// <returns>The resource builder for method chaining.</returns>
    public static IResourceBuilder<DevProxyExecutableResource> WithConfigFile(
        this IResourceBuilder<DevProxyExecutableResource> builder,
        string configFile)
    {
        return builder.WithArgs("-c", Path.GetFullPath(configFile, builder.ApplicationBuilder.AppHostDirectory));
    }

    /// <summary>
    /// Configures the DevProxy executable to watch specific URLs.
    /// </summary>
    /// <param name="builder">The resource builder for the DevProxy executable resource.</param>
    /// <param name="urlsToWatch">The collection of URLs to watch.</param>
    /// <returns>The resource builder for method chaining.</returns>
    public static IResourceBuilder<DevProxyExecutableResource> WithUrlsToWatch(
        this IResourceBuilder<DevProxyExecutableResource> builder,
        Func<IEnumerable<string>> urlsToWatch)
    {
        return builder.WithArgs(context =>
        {
            context.Args.Add("-u");
            context.Args.Add(string.Join(" ", urlsToWatch()));
        });
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

        var resourceBuilder = builder.AddResource(resource)
            .WithImage(DevProxyContainerImageTags.Image)
            .WithImageRegistry(DevProxyContainerImageTags.Registry)
            .WithImageTag(DevProxyContainerImageTags.Tag)
            .WithHttpEndpoint(targetPort: 8000, name: DevProxyResource.ProxyEndpointName)
            .WithHttpEndpoint(targetPort: 8897, name: DevProxyResource.ApiEndpointName);

        return resourceBuilder;
    }

    /// <summary>
    /// Configures the DevProxy container to mount a certificate folder.
    /// </summary>
    /// <param name="builder">The resource builder for the DevProxy container resource.</param>
    /// <param name="certFolder">The path to the certificate folder on the host.</param>
    /// <returns>The resource builder for method chaining.</returns>
    public static IResourceBuilder<DevProxyContainerResource> WithCertFolder(
        this IResourceBuilder<DevProxyContainerResource> builder,
        string certFolder)
    {
        return builder.WithBindMount(
            Path.GetFullPath(certFolder, builder.ApplicationBuilder.AppHostDirectory),
            "/home/devproxy/.config/dev-proxy/rootCert");
    }

    /// <summary>
    /// Configures the DevProxy executable to use a specific configuration file.
    /// </summary>
    /// <param name="builder">The resource builder for the DevProxy executable resource.</param>
    /// <param name="configFile">The path to the configuration file.</param>
    /// <returns>The resource builder for method chaining.</returns>
    public static IResourceBuilder<DevProxyContainerResource> WithConfigFile(
        this IResourceBuilder<DevProxyContainerResource> builder,
        string configFile)
    {
        return builder.WithArgs("-c", configFile);
    }

    /// <summary>
    /// Configures the DevProxy container to mount a configuration folder.
    /// </summary>
    /// <param name="builder">The resource builder for the DevProxy container resource.</param>
    /// <param name="configFolder">The path to the configuration folder on the host.</param>
    /// <returns>The resource builder for method chaining.</returns>
    public static IResourceBuilder<DevProxyContainerResource> WithConfigFolder(
        this IResourceBuilder<DevProxyContainerResource> builder,
        string configFolder)
    {
        return builder.WithBindMount(
            Path.GetFullPath(configFolder, builder.ApplicationBuilder.AppHostDirectory),
            "/config");
    }

    /// <summary>
    /// Configures the DevProxy container to watch specific URLs.
    /// </summary>
    /// <param name="builder">The resource builder for the DevProxy container resource.</param>
    /// <param name="urlsToWatch">The collection of URLs to watch.</param>
    /// <returns>The resource builder for method chaining.</returns>
    public static IResourceBuilder<DevProxyContainerResource> WithUrlsToWatch(
        this IResourceBuilder<DevProxyContainerResource> builder,
        Func<IEnumerable<string>> urlsToWatch)
    {
        return builder.WithArgs(context =>
        {
            context.Args.Add("-u");
            context.Args.Add(string.Join(" ", urlsToWatch()));
        });
    }
}

/// <summary>
/// Contains container image tags and registry information for DevProxy.
/// </summary>
internal static class DevProxyContainerImageTags
{
    /// <summary>
    /// The container registry hosting the DevProxy image.
    /// </summary>
    internal const string Registry = "ghcr.io";

    /// <summary>
    /// The DevProxy container image name.
    /// </summary>
    internal const string Image = "dotnet/dev-proxy";

    /// <summary>
    /// The DevProxy container image tag.
    /// </summary>
    internal const string Tag = "latest";
}