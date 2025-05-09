using Aspire.Hosting.ApplicationModel;

namespace DevProxy.Hosting;

/// <summary>
/// Exposes Dev Proxy through its locally installed executable.
/// </summary>
public sealed class DevProxyExecutableResource(string name):
    ExecutableResource(name, "devproxy", "")
{
}
