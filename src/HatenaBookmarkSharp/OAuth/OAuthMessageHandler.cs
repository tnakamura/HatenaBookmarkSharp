#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HatenaBookmarkSharp.OAuth
{
    internal class OAuthMessageHandler : DelegatingHandler
    {
        readonly string consumerKey;

        readonly string consumerSecret;

        readonly Token? token;

        readonly IEnumerable<KeyValuePair<string, string>> parameters;

        public OAuthMessageHandler(
            string consumerKey,
            string consumerSecret,
            Token? token = null,
            IEnumerable<KeyValuePair<string, string>>? optionalOAuthHeaderParameters = null)
            : this(
                  new HttpClientHandler(),
                  consumerKey,
                  consumerSecret,
                  token,
                  optionalOAuthHeaderParameters)
        {
        }

        public OAuthMessageHandler(
            HttpMessageHandler innerHandler,
            string consumerKey,
            string consumerSecret,
            Token? token = null,
            IEnumerable<KeyValuePair<string, string>>? optionalOAuthHeaderParameters = null)
            : base(innerHandler)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.token = token;
            parameters = optionalOAuthHeaderParameters ??
                Enumerable.Empty<KeyValuePair<string, string>>();
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var sendParameter = parameters;
            if (request.Method == HttpMethod.Post)
            {
                // form url encoded content
                if (request.Content is FormUrlEncodedContent)
                {
                    // url encoded string
                    var extraParameter = await request.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);
                    var parsed = Utility.ParseQueryString(extraParameter, true); // url decoded
                    sendParameter = sendParameter.Concat(parsed);

                    request.Content = new FormUrlEncodedContentEx(parsed);
                }
            }

            var headerParams = OAuthUtility.BuildBasicParameters(
                consumerKey, consumerSecret,
                request.RequestUri.OriginalString, request.Method, token,
                sendParameter);
            headerParams = headerParams.Concat(parameters);

            var header = headerParams.Select(p => p.Key + "=" + p.Value.Wrap("\""))
                .ToString(",");
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", header);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        class FormUrlEncodedContentEx : ByteArrayContent
        {
            public FormUrlEncodedContentEx(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
                : base(GetContentByteArray(nameValueCollection))
            {
                Headers.ContentType = new MediaTypeHeaderValue(
                    "application/x-www-form-urlencoded");
            }

            static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (IEnumerator<KeyValuePair<string, string>> enumerator = nameValueCollection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<string, string> current = enumerator.Current;
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append('&');
                        }
                        stringBuilder.Append(Encode(current.Key));
                        stringBuilder.Append('=');
                        stringBuilder.Append(Encode(current.Value));
                    }
                }
                return Encoding.UTF8.GetBytes(stringBuilder.ToString());
            }

            static string Encode(string data)
            {
                if (string.IsNullOrEmpty(data))
                {
                    return string.Empty;
                }
                return data.UrlEncode().Replace("%20", "+");
            }
        }
    }
}
