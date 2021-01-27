# HatenaBookmarkSharp

Hatena Bookmark Web API Client for C#.

## Install

```sh
PM> Install-Package HatenaBookmarkSharp
```

```sh
$ dotnet add package HatenaBookmarkSharp
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
var createdBookmark = await client.CreateBookmarkAsync(
    new NewBookmark(url)
    {
        Comment = "[blog]Test",
    });

var bookmark = await client.GetBookmarkAsync(url);

var entry = await client.GetEntryAsync(url);

await client.DeleteBookmarkAsync(url);
```

## Contribution

1. Fork it ( https://github.com/tnakamura/HatenaBookmarkSharp )
2. Create your feature branch (git checkout -b my-new-feature)
3. Commit your changes (git commit -am 'Add some feature')
4. Push to the branch (git push origin my-new-feature)
5. Create new Pull Request

## License

[MIT](https://raw.githubusercontent.com/tnakamura/HatenaBookmarkSharp/main/LICENSE)

## Author

[tnakamura](https://github.com/tnakamura)
