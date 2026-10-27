using Aspire.Hosting.ApplicationModel;

namespace DevProxy.Hosting;

/// <summary>
/// Exposes Dev Proxy through its locally installed executable.
/// </summary>
public sealed class DevProxyExecutableResource(string name) :
    ExecutableResource(name, "devproxy", ""), IDevProxyApiResource
{
    /// <inheritdoc />
    public string ApiTokenDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "dev-proxy",
        "credentials");
}