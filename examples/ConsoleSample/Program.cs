using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HatenaBookmarkSharp;

namespace ConsoleSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HatenaBookmarkClient(
                options: new HatenaBookmarkClientOptions
                {
                });

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
