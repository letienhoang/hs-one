﻿using HSOne.Core.Domain.Content;
using HSOne.Core.Models.Content;
using HSOne.Core.SeedWorks;

namespace HSOne.Core.Repositories
{
    public interface ITagRepository : IRepository<Tag, Guid>
    {
        Task<bool> IsExistsPostTagAsync(Guid postId, Guid tagId);
        Task<bool> IsExistsTagOfPostAsync(Guid postId);
        Task<List<TagDto>> GetPostTagsAsync(Guid postId);
        Task<List<string>> GetAllNameTagsAsync();
        Task<TagDto?> GetTagBySlugAsync(string slug);
        Task AddPostTagAsync(Guid postId, Guid tagId);
        Task RemoveTagsOfPostAsync(Guid postId);
    }
}
