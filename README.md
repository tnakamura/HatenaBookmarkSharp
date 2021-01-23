# HatenaBookmarkSharp

Hatena Bookmark Web API Client for C#.

## Installation

```sh
PM> Install-Package HatenaBookmarkSharp
```

```sh
dotnet add package HatenaBookmarkSharp
```

## Usage

```cs
var authorizer = new HatenaAuthorizer(
    consumerKey: consumerKey,
    consumerSecret: consumerSecret);

var requestToken = await authorizer.GetRequestTokenAsync(
    scope: Scopes.All);

var authenticationUri = authorizer.GenerateAuthenticationUri(
    requestToken.OAuthToken);
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

var tags = await client.GetMyTagsAsync();

var url = new Uri("https://tnakamura.hatenablog.com");
var postedBookmark = await client.CreateBookmarkAsync(
    new NewBookmark(url)
    {
        Comment = "[blog]Test",
    });

var bookmark = await client.GetBookmarkAsync(url);

var entry = await client.GetEntryAsync(url);

await client.DeleteBookmarkAsync(url);
```

## License

MIT

