using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs
{
    public class CategoryDTO
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class NewsArticleDTO
    {
        public string NewsArticleId { get; set; }
        public string? NewsTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public CategoryDTO? Category { get; set; }
    }
}
