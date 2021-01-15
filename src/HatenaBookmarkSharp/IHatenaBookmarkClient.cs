using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HatenaBookmarkSharp.Models;

namespace HatenaBookmarkSharp
{
    public interface IHatenaBookmarkClient
    {
        Task<Bookmark> GetBookmarkAsync(Uri url);

        Task<Bookmark> PostBookmarkAsync(PostRequest parameter);

        Task DeleteBookmarkAsync(Uri url);

        Task<Entry> GetEntryAsync(Uri url);

        Task<IReadOnlyList<Tag>> GetMyTagsAsync();

        Task<User> GetMyAsync();
    }
}
