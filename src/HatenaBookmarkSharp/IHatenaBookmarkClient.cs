#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HatenaBookmarkSharp
{
    public interface IHatenaBookmarkClient
    {
        Task<Bookmark> GetBookmarkAsync(Uri uri, CancellationToken cancellationToken = default);

        Task<Bookmark> PostBookmarkAsync(PostRequest parameter, CancellationToken cancellationToken = default);

        Task DeleteBookmarkAsync(Uri uri, CancellationToken cancellationToken = default);

        Task<Entry> GetEntryAsync(Uri uri, CancellationToken cancellationToken = default);

        Task<TagsResponse> GetMyTagsAsync(CancellationToken cancellationToken = default);

        Task<User> GetMyAsync(CancellationToken cancellationToken = default);
    }
}
