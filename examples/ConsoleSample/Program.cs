using System;
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

            Console.Write("authenticationCode:");
            var authenticationCode = Console.ReadLine();

            var user = await client.GetMyAsync();

            var tags = await client.GetMyTagsAsync();

            var url = new Uri("https://tnakamura.hatenablog.com");

            var postedBookmark = await client.PostBookmarkAsync(
                new PostRequest
                {
                });

            var bookmark = await client.GetBookmarkAsync(url);

            var entry = await client.GetEntryAsync(url);

            await client.DeleteBookmarkAsync(url);
        }
    }
}
