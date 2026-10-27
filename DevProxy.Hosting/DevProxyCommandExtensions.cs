using Aspire.Hosting.ApplicationModel;
using DevProxy.Hosting;
using Refit;

#pragma warning disable IDE0130
namespace Aspire.Hosting;
#pragma warning restore IDE0130

/// <summary>
/// Adds dashboard commands for controlling Dev Proxy recording.
/// </summary>
public static class DevProxyCommandExtensions
{
    /// <summary>
    /// Adds commands to start and stop recording on a Dev Proxy resource.
    /// </summary>
    /// <typeparam name="TResource">The Dev Proxy resource type.</typeparam>
    /// <param name="builder">The Dev Proxy resource builder.</param>
    /// <returns>The resource builder for method chaining.</returns>
    public static IResourceBuilder<TResource> WithDevProxyRecordingCommands<TResource>(
        this IResourceBuilder<TResource> builder)
        where TResource : IDevProxyApiResource
    {
        builder.WithCommand(
            name: "start-recording",
            displayName: "Start recording",
            executeCommand: context => SetRecordingAsync(builder, context, recording: true),
            commandOptions: new CommandOptions());

        builder.WithCommand(
            name: "stop-recording",
            displayName: "Stop recording",
            executeCommand: context => SetRecordingAsync(builder, context, recording: false),
            commandOptions: new CommandOptions());

        return builder;
    }

    private static async Task<ExecuteCommandResult> SetRecordingAsync<TResource>(
        IResourceBuilder<TResource> builder,
        ExecuteCommandContext context,
        bool recording)
        where TResource : IDevProxyApiResource
    {
        var endpointUrl = await builder.Resource
            .GetEndpoint(DevProxyResource.ApiEndpointName)
            .GetValueAsync(context.CancellationToken);

        if (string.IsNullOrWhiteSpace(endpointUrl))
        {
            return CommandResults.Failure("The Dev Proxy API endpoint is not available.");
        }

        var apiEndpoint = new Uri(endpointUrl);
        using var httpClient = new HttpClient(new HttpClientHandler { UseProxy = false })
        {
            BaseAddress = apiEndpoint
        };
        var apiClient = RestService.ForGenerated<IDevProxyApiClient>(
            httpClient,
            new RefitSettings
            {
                AuthorizationHeaderValueGetter = (_, cancellationToken) =>
                    builder.Resource.GetApiKeyAsync(apiEndpoint, cancellationToken)
            });

        try
        {
            await apiClient.SetProxyAsync(
                new DevProxyUpdate { Recording = recording },
                context.CancellationToken);

            return CommandResults.Success(recording ? "Recording started." : "Recording stopped.");
        }
        catch (ApiException exception)
        {
            return CommandResults.Failure(
                $"Dev Proxy returned {(int)exception.StatusCode} {exception.ReasonPhrase}: {exception.Content}");
        }
        catch (InvalidOperationException exception)
        {
            return CommandResults.Failure(exception.Message);
        }
        catch (HttpRequestException exception)
        {
            return CommandResults.Failure($"Could not reach the Dev Proxy API: {exception.Message}");
        }
    }
}