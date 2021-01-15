#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HatenaBookmarkSharp.Models;

namespace HatenaBookmarkSharp
{
    public class HatenaBookmarkClient : IHatenaBookmarkClient
    {
        readonly HttpClient httpClient;

        public HatenaBookmarkClient()
            : this(new HttpClient())
        {
        }

        public HatenaBookmarkClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("https://bookmark.hatenaapis.com");
        }

        public Task<Bookmark> GetBookmarkAsync(Uri url)
        {
            return GetAsync<Bookmark>($"/rest/1/my/bookmark?url={url}");
        }

        public Task<Entry> GetEntryAsync(Uri url)
        {
            return GetAsync<Entry>($"/rest/1/entry?url={url}");
        }

        public Task<User> GetMyAsync()
        {
            return GetAsync<User>($"/rest/1/my");
        }

        public Task<IReadOnlyList<Tag>> GetMyTagsAsync()
        {
            return GetAsync<IReadOnlyList<Tag>>($"/rest/1/my/tags");
        }

        public async Task<Bookmark> PostBookmarkAsync(PostRequest parameter)
        {
            var requestBody = JsonSerializer.Serialize(parameter);
            var response = await httpClient.PostAsync(
                $"/rest/1/my/bookmark",
                new StringContent(
                    requestBody,
                    Encoding.UTF8,
                    "application/json"))
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var bookmark = Deserialize<Bookmark>(responseBody);
            return bookmark!;
        }

        public async Task DeleteBookmarkAsync(Uri url)
        {
            var response = await httpClient
                .DeleteAsync($"/rest/1/my/bookmark?url={url}")
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        async Task<T> GetAsync<T>(string requestUri)
        {
            var response = await httpClient.GetAsync(requestUri)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return Deserialize<T>(json);
        }

        static T Deserialize<T>(string json)
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(DateTimeConverter.Default);
            var result = JsonSerializer.Deserialize<T>(json, options);
            return result!;
        }
    }
}
