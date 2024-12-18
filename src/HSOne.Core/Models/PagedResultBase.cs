using System.ComponentModel.DataAnnotations;

namespace HSOne.Core.Models
{
    public class PagedResultBase
    {
        [Required]
        public int CurrentPage { get; set; }

        [Required]
        public int PageCount
        {
            get
            {
                var pageCount = (double)RowCount / PageSize;
                return (int)Math.Ceiling(pageCount);
            }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                PageCount = value;
            }
        }

        [Required]
        public int PageSize { get; set; }
        [Required]
        public int RowCount { get; set; }
        [Required]
        public int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;
        [Required]
        public int LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);
        [Required]
        public int AdditionalData { get; set; }
    }
}