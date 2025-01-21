using AutoMapper;
using HSOne.Core.Domain.Identity;
using HSOne.Core.Repositories;
using HSOne.Core.SeedWorks;
using HSOne.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace HSOne.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HSOneContext _context;

        public UnitOfWork(HSOneContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _context = context;
            Posts = new PostRepository(_context, mapper, userManager);
            PostCategories = new PostCategoryRepository(_context, mapper);
            Series = new SeriesRepository(_context, mapper);
            PostInSeries = new PostInSeriesRepository(_context, mapper);
            Transactions = new TransactionRepository(_context, mapper);
            Tags = new TagRepository(_context, mapper);
        }

        public IPostRepository Posts { get; private set; }
        public IPostCategoryRepository PostCategories { get; private set; }
        public ISeriesRepository Series { get; private set; }
        public IPostInSeriesRepository PostInSeries { get; private set; }
        public ITransactionRepository Transactions { get; private set; }
        public ITagRepository Tags { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}