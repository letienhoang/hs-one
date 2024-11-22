using HSOne.Core.SeedWorks;

namespace HSOne.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HSOneContext _context;

        public UnitOfWork(HSOneContext context)
        {
            _context = context;
        }

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