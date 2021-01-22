using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ConsoleAppFramework;
using HatenaBookmarkSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConsoleSample
{
    class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                .RunConsoleAppFrameworkAsync<Program>(args);
        }

        readonly IConfiguration configuration;

        public Program(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task Run()
        {
            var consumerKey = configuration["OAuthConsumerKey"];
            var consumerSecret = configuration["OAuthConsumerSecret"];

            var authorizer = new HatenaAuthorizer(
                consumerKey: consumerKey,
                consumerSecret: consumerSecret);

            var requestToken = await authorizer.GetRequestTokenAsync(
                scope: Scopes.ReadPublic | Scopes.ReadPrivate);

            var authenticationUri = authorizer.GenerateAuthenticationUri(
                requestToken.OAuthToken);
            Console.WriteLine(authenticationUri);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var escapedUrl = authenticationUri.ToString().Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {escapedUrl}")
                {
                    CreateNoWindow = true,
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", authenticationUri.ToString());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", authenticationUri.ToString());
            }

            Console.Write("authenticationCode:");

            var verifier = Console.ReadLine();

            var accessToken = await authorizer.GetAccessTokenAsync(
                oauthToken: requestToken.OAuthToken,
                oauthTokenSecret: requestToken.OAuthTokenSecret,
                verifier: verifier);

            var client = new HatenaBookmarkClient(
                consumerKey: consumerKey,
                consumerSecret: consumerSecret,
                oauthToken: accessToken.OAuthToken,
                oauthTokenSecret: accessToken.OAuthTokenSecret);

            var user = await client.GetMyAsync();
            Console.WriteLine(user.Name);

            var tags = await client.GetMyTagsAsync();
            foreach (var tag in tags)
            {
                Console.WriteLine(tag.Name);
            }

            var url = new Uri("https://tnakamura.hatenablog.com");

            var postedBookmark = await client.PostBookmarkAsync(
                new PostRequest
                {
                    Uri = url,
                    Comment = "Test",
                });

            var bookmark = await client.GetBookmarkAsync(url);

            var entry = await client.GetEntryAsync(url);

            await client.DeleteBookmarkAsync(url);
        }
    }
}
