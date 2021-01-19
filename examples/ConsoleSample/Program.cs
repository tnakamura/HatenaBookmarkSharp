using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ConsoleAppFramework;
using HatenaBookmarkSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ConsoleSample
{
    class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<HatenaBookmarkClientOptions>(hostContext.Configuration);
                })
                .RunConsoleAppFrameworkAsync<Program>(args);
        }

        readonly IOptions<HatenaBookmarkClientOptions> options;

        public Program(IOptions<HatenaBookmarkClientOptions> options)
        {
            this.options = options;
        }

        public async Task Run()
        {
            var client = new HatenaBookmarkClient(
                options: options.Value);

            var requestToken = await client.GetRequestTokenAsync();

            var authenticationUri = client.GenerateAuthenticationUri(
                requestToken.OAuthToken);
            Process.Start(authenticationUri.ToString());

            Console.Write("authenticationCode:");
            var authenticationCode = Console.ReadLine();

            var accessToken = await client.GetAccessTokenAsync(
                authenticationCode);
            client.SetAccessToken(accessToken.OAuthToken);

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
