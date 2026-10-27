using System.Net;
using System.Net.Http.Headers;

namespace DevProxy.Hosting;

internal static class DevProxyApiTokenStore
{
    internal static string CreateTemporaryTokenDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "devproxy-aspire", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }

    internal static async ValueTask<string> GetApiKeyAsync(
        string tokenDirectory,
        Uri apiEndpoint,
        CancellationToken cancellationToken)
    {
        if (!Directory.Exists(tokenDirectory))
        {
            throw new InvalidOperationException("The Dev Proxy API token directory is not available.");
        }

        var tokenFiles = Directory.EnumerateFiles(tokenDirectory, "api-*.token").ToArray();
        if (tokenFiles.Length == 0)
        {
            throw new InvalidOperationException("The Dev Proxy API token is not available yet.");
        }

        using var httpClient = new HttpClient(new HttpClientHandler { UseProxy = false });
        foreach (var tokenFile in tokenFiles)
        {
            var token = (await File.ReadAllTextAsync(tokenFile, cancellationToken)).Trim();
            using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(apiEndpoint, "/proxy"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await httpClient.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                continue;
            }

            response.EnsureSuccessStatusCode();
            return token;
        }

        throw new InvalidOperationException("No API token was found for this Dev Proxy resource.");
    }
}