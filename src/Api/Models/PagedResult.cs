using System.Collections.Generic;

namespace The9Books.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int Start { get; set; }
        public int Size { get; set; }
        public bool HasMore { get; set; }
    }
}
