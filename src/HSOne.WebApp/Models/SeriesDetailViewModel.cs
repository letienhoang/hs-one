using HSOne.Core.Models;
using HSOne.Core.Models.Content;
using HSOne.Data.Repositories;

namespace HSOne.WebApp.Models
{
    public class SeriesDetailViewModel
    {
        public required SeriesDto Series { get; set; }
        public required PagedResult<PostInListDto> Posts { get; set; }
    }
}
