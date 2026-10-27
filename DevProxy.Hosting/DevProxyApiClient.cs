using Refit;

namespace DevProxy.Hosting;

/// <summary>
/// Typed client for the Dev Proxy management API.
/// </summary>
public interface IDevProxyApiClient
{
    /// <summary>
    /// Gets the current proxy state and configuration file.
    /// </summary>
    [Get("/proxy")]
    Task<DevProxyInfo> GetProxyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the proxy recording state.
    /// </summary>
    [Post("/proxy")]
    Task<DevProxyInfo> SetProxyAsync([Body] DevProxyUpdate update, CancellationToken cancellationToken = default);

    /// <summary>
    /// Raises a mock request.
    /// </summary>
    [Post("/proxy/mockRequest")]
    Task<HttpResponseMessage> RaiseMockRequestAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops Dev Proxy.
    /// </summary>
    [Post("/proxy/stopProxy")]
    Task<HttpResponseMessage> StopProxyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a JWT using the supplied options.
    /// </summary>
    [Post("/proxy/jwtToken")]
    Task<DevProxyJwtToken> CreateJwtTokenAsync([Body] DevProxyJwtOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the Dev Proxy root certificate in PEM format.
    /// </summary>
    [Get("/proxy/rootCertificate")]
    Task<HttpResponseMessage> GetRootCertificateAsync([Query] string format, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or follows Dev Proxy logs.
    /// </summary>
    [Get("/proxy/logs")]
    Task<HttpResponseMessage> GetLogsAsync(
        [Query] int? lines = null,
        [Query] bool follow = false,
        [Query] string? since = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the Dev Proxy state returned by the management API.
/// </summary>
public sealed class DevProxyInfo
{
    /// <summary>
    /// Gets or sets whether Dev Proxy is recording.
    /// </summary>
    public bool? Recording { get; set; }

    /// <summary>
    /// Gets or sets the active configuration file.
    /// </summary>
    public string? ConfigFile { get; set; }
}

/// <summary>
/// Represents a request to update Dev Proxy state.
/// </summary>
public sealed class DevProxyUpdate
{
    /// <summary>
    /// Gets or sets the requested recording state.
    /// </summary>
    public bool? Recording { get; set; }
}

/// <summary>
/// Represents options for creating a Dev Proxy JWT.
/// </summary>
public sealed class DevProxyJwtOptions
{
    /// <summary>Gets or sets the token audiences.</summary>
    public IEnumerable<string>? Audiences { get; set; }

    /// <summary>Gets or sets the token claims.</summary>
    public Dictionary<string, string>? Claims { get; set; }

    /// <summary>Gets or sets the token issuer.</summary>
    public string? Issuer { get; set; }

    /// <summary>Gets or sets the token name.</summary>
    public string? Name { get; set; }

    /// <summary>Gets or sets the token roles.</summary>
    public IEnumerable<string>? Roles { get; set; }

    /// <summary>Gets or sets the token scopes.</summary>
    public IEnumerable<string>? Scopes { get; set; }

    /// <summary>Gets or sets the signing key, which must contain at least 32 characters.</summary>
    public string? SigningKey { get; set; }

    /// <summary>Gets or sets the token lifetime in minutes.</summary>
    public double? ValidFor { get; set; }
}

/// <summary>
/// Represents a generated JWT.
/// </summary>
public sealed class DevProxyJwtToken
{
    /// <summary>Gets or sets the generated token.</summary>
    public string Token { get; set; } = "";
}