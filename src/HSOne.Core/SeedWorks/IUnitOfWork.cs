using HSOne.Core.Repositories;

namespace HSOne.Core.SeedWorks
{
    public interface IUnitOfWork
    {
        IPostRepository Posts { get; }
        IPostCategoryRepository PostCategories { get; }
        ISeriesRepository Series { get; }
        IPostInSeriesRepository PostInSeries { get; }
        Task<int> CompleteAsync();
    }
}