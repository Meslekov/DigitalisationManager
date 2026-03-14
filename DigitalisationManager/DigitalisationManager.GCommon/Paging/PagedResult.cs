namespace DigitalisationManager.GCommon.Paging
{
    public class PagedResult<T>
    {
        public PagedResult()
        {
            this.Items = new List<T>();
        }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public bool HasPrevious => this.Page > 1;

        public bool HasNext => this.Page < this.TotalPages;

        public List<T> Items { get; set; }
    }
}