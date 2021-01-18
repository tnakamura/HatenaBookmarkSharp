#nullable enable
using System;
using System.Threading;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HatenaBookmarkSharp.Models;

namespace HatenaBookmarkSharp
{
    public partial class HatenaBookmarkClient : IHatenaBookmarkClient
    {
        readonly HttpClient httpClient;

        readonly HatenaBookmarkClientOptions options;

        public HatenaBookmarkClient(
            HttpClient? httpClient = null,
            HatenaBookmarkClientOptions? options = null)
        {
            this.options = options ?? new HatenaBookmarkClientOptions(); ;
            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("https://bookmark.hatenaapis.com");
            if (this.options.AccessToken != null)
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    this.options.AccessToken);
            }
        }

        public Task<Bookmark> GetBookmarkAsync(
            Uri uri,
            CancellationToken cancellationToken = default)
        {
            return GetAsync<Bookmark>(
                $"/rest/1/my/bookmark?url={uri}",
                cancellationToken);
        }

        public Task<Entry> GetEntryAsync(
            Uri uri,
            CancellationToken cancellationToken = default)
        {
            return GetAsync<Entry>(
                $"/rest/1/entry?url={uri}",
                cancellationToken);
        }

        public Task<User> GetMyAsync(CancellationToken cancellationToken = default)
        {
            return GetAsync<User>(
                $"/rest/1/my",
                cancellationToken);
        }

        public Task<IReadOnlyList<Tag>> GetMyTagsAsync(
            CancellationToken cancellationToken = default)
        {
            return GetAsync<IReadOnlyList<Tag>>(
                $"/rest/1/my/tags",
                cancellationToken);
        }

        public async Task<Bookmark> PostBookmarkAsync(
            PostRequest parameter,
            CancellationToken cancellationToken = default)
        {
            var requestBody = JsonSerializer.Serialize(parameter);
            var response = await httpClient.PostAsync(
                $"/rest/1/my/bookmark",
                new StringContent(
                    requestBody,
                    Encoding.UTF8,
                    "application/json"),
                cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var bookmark = Deserialize<Bookmark>(responseBody);
            return bookmark!;
        }

        public async Task DeleteBookmarkAsync(
            Uri uri,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient
                .DeleteAsync(
                    $"/rest/1/my/bookmark?url={uri}",
                    cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        async Task<T> GetAsync<T>(
            string requestUri,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(requestUri, cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            return Deserialize<T>(json);
        }

        static T Deserialize<T>(string json)
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(DateTimeConverter.Default);
            var result = JsonSerializer.Deserialize<T>(json, options);
            return result!;
        }

        public Task<RequestToken> GetRequestTokenAsync()
        {
            return Task.FromResult(new RequestToken
            {
            });
        }

        public Uri GenerateAuthenticationUri(string requestToken)
        {
            return new Uri("");
        }

        public Task<AccessToken> GetAccessTokenAsync(string authenticationCode)
        {
            return Task.FromResult(new AccessToken
            {
            });
        }

        public void SetAccessToken(string accessToken)
        {
        }
    }
}
