using AutoMapper;
using HSOne.Core.Domain.Content;
using HSOne.Core.Models.Content;
using HSOne.Core.Repositories;
using HSOne.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace HSOne.Data.Repositories
{
    public class TagRepository : RepositoryBase<Tag, Guid>, ITagRepository
    {
        private readonly IMapper _mapper;
        public TagRepository(HSOneContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }
        public async Task<bool> IsExistsPostTagAsync(Guid postId, Guid tagId)
        {
            return await _context.PostTags.AnyAsync(x => x.PostId == postId && x.TagId == tagId);
        }

        public async Task<bool> IsExistsTagOfPostAsync(Guid postId)
        {
            return await _context.PostTags.AnyAsync(x => x.PostId == postId);
        }

        public async Task<List<TagDto>> GetPostTagsAsync(Guid postId)
        {
            var query = from pt in _context.PostTags
                        join t in _context.Tags on pt.TagId equals t.Id
                        where pt.PostId == postId
                        select t;
            return await _mapper.ProjectTo<TagDto>(query).ToListAsync();
        }

        public async Task<TagDto?> GetTagBySlugAsync(string slug)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(x => x.Slug == slug);
            if (tag == null)
            {
                return null;
            }
            return _mapper.Map<TagDto>(tag);
        }

        public async Task<List<string>> GetAllNameTagsAsync()
        {
            var query = _context.Tags.Select(x => x.Name);
            return await query.ToListAsync();
        }

        public async Task AddPostTagAsync(Guid postId, Guid tagId)
        {
            await _context.PostTags.AddAsync(new PostTag { PostId = postId, TagId = tagId });
        }
        
        public async Task RemoveTagsOfPostAsync(Guid postId)
        {
            var postTags = await _context.PostTags.Where(x => x.PostId == postId).ToListAsync();
            _context.PostTags.RemoveRange(postTags);
        }
    }
}
