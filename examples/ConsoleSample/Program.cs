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
                scope: Scopes.All);

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

            Console.Write("verifier:");

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
            Console.WriteLine(nameof(client.GetMyAsync));
            Console.WriteLine(user.Name);
            Console.WriteLine($"{user.IsPlusUser}");
            Console.WriteLine($"{user.IsPrivate}");
            Console.WriteLine($"{user.IsOAuthTwitter}");
            Console.WriteLine($"{user.IsOAuthFacebook}");
            Console.WriteLine($"{user.IsOAuthEvernote}");
            Console.WriteLine($"{user.IsOAuthMixiCheck}");

            var tags = await client.GetMyTagsAsync();
            Console.WriteLine(nameof(client.GetMyTagsAsync));
            foreach (var tag in tags.Tags)
            {
                Console.WriteLine($"{tag.Name}({tag.Count})");
            }

            var url = new Uri("https://tnakamura.hatenablog.com");

            var postedBookmark = await client.PostBookmarkAsync(
                new PostRequest(url)
                {
                    Comment = "[blog]Test",
                });
            Console.WriteLine(nameof(client.PostBookmarkAsync));
            Console.WriteLine(postedBookmark.Permalink);
            Console.WriteLine(postedBookmark.Comment);
            Console.WriteLine(postedBookmark.User);
            foreach (var tag in postedBookmark.Tags)
            {
                Console.WriteLine(tag);
            }

            var bookmark = await client.GetBookmarkAsync(url);
            Console.WriteLine(nameof(client.GetBookmarkAsync));
            Console.WriteLine(bookmark.Permalink);
            Console.WriteLine(bookmark.Comment);
            Console.WriteLine(bookmark.User);
            foreach (var tag in bookmark.Tags)
            {
                Console.WriteLine(tag);
            }

            var entry = await client.GetEntryAsync(url);
            Console.WriteLine(nameof(client.GetEntryAsync));
            Console.WriteLine(entry.Title);
            Console.WriteLine(entry.EntryUrl);
            Console.WriteLine(entry.SmartPhoneAppEntryUrl);

            await client.DeleteBookmarkAsync(url);
        }
    }
}
