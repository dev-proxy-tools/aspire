using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using Aspire.Hosting.ApplicationModel;
using DevProxy.Hosting;

#pragma warning disable IDE0130
namespace Aspire.Hosting;
#pragma warning restore IDE0130

/// <summary>
/// Adds dashboard commands for controlling Dev Proxy through its API.
/// </summary>
internal static class DevProxyCommandExtensions
{
    internal const string ApiTokenEnvironmentVariable = "DEV_PROXY_API_TOKEN";

    /// <summary>
    /// Generates an API token for the Dev Proxy resource, passes it to Dev Proxy
    /// and adds commands to start and stop recording.
    /// </summary>
    internal static IResourceBuilder<TResource> WithRecordingCommands<TResource>(
        this IResourceBuilder<TResource> builder)
        where TResource : IResourceWithEndpoints, IResourceWithEnvironment
    {
        var apiToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        builder.WithEnvironment(ApiTokenEnvironmentVariable, apiToken);

        builder.WithCommand(
            name: "start-recording",
            displayName: "Start recording",
            executeCommand: context => SetRecordingAsync(builder.Resource, apiToken, recording: true, context),
            commandOptions: new CommandOptions
            {
                Description = "Start recording requests in Dev Proxy",
                IconName = "Record",
                UpdateState = UpdateCommandState
            });

        builder.WithCommand(
            name: "stop-recording",
            displayName: "Stop recording",
            executeCommand: context => SetRecordingAsync(builder.Resource, apiToken, recording: false, context),
            commandOptions: new CommandOptions
            {
                Description = "Stop recording requests in Dev Proxy",
                IconName = "RecordStop",
                UpdateState = UpdateCommandState
            });

        return builder;
    }

    private static ResourceCommandState UpdateCommandState(UpdateCommandStateContext context) =>
        context.ResourceSnapshot.State?.Text == KnownResourceStates.Running
            ? ResourceCommandState.Enabled
            : ResourceCommandState.Disabled;

    private static async Task<ExecuteCommandResult> SetRecordingAsync(
        IResourceWithEndpoints resource,
        string apiToken,
        bool recording,
        ExecuteCommandContext context)
    {
        try
        {
            var endpointUrl = await resource
                .GetEndpoint(DevProxyResource.ApiEndpointName)
                .GetValueAsync(context.CancellationToken);

            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                return CommandResults.Failure("The Dev Proxy API endpoint is not available.");
            }

            using var httpClient = new HttpClient(new HttpClientHandler { UseProxy = false })
            {
                BaseAddress = new Uri(endpointUrl),
                Timeout = TimeSpan.FromSeconds(10)
            };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

            using var response = await httpClient.PostAsJsonAsync(
                "/proxy",
                new DevProxyUpdate { Recording = recording },
                context.CancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return CommandResults.Failure(
                    $"Dev Proxy rejected the API token. Make sure you're using a version of Dev Proxy that supports the {ApiTokenEnvironmentVariable} environment variable.");
            }

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(context.CancellationToken);
                return CommandResults.Failure(
                    $"Dev Proxy returned {(int)response.StatusCode} {response.ReasonPhrase}: {content}");
            }

            return CommandResults.Success(recording ? "Recording started." : "Recording stopped.");
        }
        catch (HttpRequestException exception)
        {
            return CommandResults.Failure($"Could not reach the Dev Proxy API: {exception.Message}");
        }
        catch (TaskCanceledException) when (!context.CancellationToken.IsCancellationRequested)
        {
            return CommandResults.Failure("The Dev Proxy API did not respond in time.");
        }
    }

    private sealed class DevProxyUpdate
    {
        public bool? Recording { get; set; }
    }
}
