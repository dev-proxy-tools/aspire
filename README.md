# Dev Proxy .NET Aspire extensions

Use Dev Proxy extensions for .NET Aspire to seamlessly integrate Dev Proxy into your distributed applications. Use Dev Proxy to:

- Verify how your distributed app handles API errors, both from your own services and third-party APIs
- Mock APIs while developing your app
- And more!

## What This Package Does

The Dev Proxy .NET Aspire extensions allow you to easily add Dev Proxy as a resource in your distributed application. This package provides the following features:

- Add Dev Proxy as a container or executable resource to your application
- Configure Dev Proxy with custom arguments and bind mounts
- Integrate Dev Proxy with other services in your application

## Usage

Here's how to add Dev Proxy's Docker container to your application:

```csharp
using DevProxy.Hosting;

var builder = DistributedApplication
    .CreateBuilder(args);

// Add an API service to the application
var apiService = builder.AddProject<Projects.AspireStarterApp_ApiService>("apiservice")
    .WithHttpsHealthCheck("/health");

// Add Dev Proxy as a container resource
var devProxy = builder.AddDevProxyContainer("devproxy")
    // specify the Dev Proxy configuration file
    .WithConfigFile("./devproxy.json")
    // mount the local folder with PFX certificate for intercepting HTTPS traffic
    .WithCertFolder(".devproxy/cert")
    // mount the local folder with Dev Proxy configuration
    .WithConfigFolder(".devproxy/config")
    // let Dev Proxy intercept requests to the API service
    .WithUrlsToWatch(() => [$"{apiService.GetEndpoint("https").Url}/*"]);

// Add a web frontend project and configure it to use Dev Proxy
builder.AddProject<Projects.AspireStarterApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpsHealthCheck("/health")
    // set the HTTPS_PROXY environment variable to the Dev Proxy endpoint
    .WithEnvironment("HTTPS_PROXY", devProxy.GetEndpoint(DevProxyResource.ProxyEndpointName))
    .WithReference(apiService)
    .WaitFor(apiService)
    .WaitFor(devProxy);

// Build and run the application
builder.Build().Run();
```

If you have Dev Proxy installed locally, you can add it as a resource instead of using the Docker container.

```csharp
using DevProxy.Hosting;

var builder = DistributedApplication
    .CreateBuilder(args);

// Add an API service to the application
var apiService = builder.AddProject<Projects.AspireStarterApp_ApiService>("apiservice")
    .WithHttpsHealthCheck("/health");

var devProxy = builder.AddDevProxyExecutable("devproxy")
    .WithConfigFile(".devproxy/config/devproxy.json")
    .WithUrlsToWatch(() => [$"{apiService.GetEndpoint("https").Url}/*"]);

// Add a web frontend project and configure it to use Dev Proxy
builder.AddProject<Projects.AspireStarterApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpsHealthCheck("/health")
    .WithEnvironment("HTTPS_PROXY", devProxy.GetEndpoint(DevProxyResource.ProxyEndpointName))
    .WithReference(apiService)
    .WaitFor(apiService)
    .WaitFor(devProxy);

// Build and run the application
builder.Build().Run();
```

### Key Features

- **Container Resource**: Use `AddDevProxyContainer` to run Dev Proxy as a containerized service.
- **Executable Resource**: Use `AddDevProxyExecutable` to run Dev Proxy from the locally installed executable.
- **Custom Configuration**: Use the extension methods to configure Dev Proxy as needed.
- **Service Integration**: Easily integrate Dev Proxy with other services in your application.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Related Projects

- [Dev Proxy](https://learn.microsoft.com/microsoft-cloud/dev/dev-proxy/overview)
- [.NET Aspire overview](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview)
