﻿using AutoMapper;
using HSOne.Core.Repositories;
using HSOne.Core.SeedWorks;
using HSOne.Data.Repositories;

namespace HSOne.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HSOneContext _context;

        public UnitOfWork(HSOneContext context, IMapper mapper)
        {
            _context = context;
            Posts = new PostRepository(_context, mapper);
        }

        public IPostRepository Posts { get; private set; }

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