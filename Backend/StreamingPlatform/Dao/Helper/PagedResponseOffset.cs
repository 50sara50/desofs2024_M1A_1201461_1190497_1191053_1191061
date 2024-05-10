namespace StreamingPlatform.Dao.Helper
{
    public class PagedResponseOffset<T>
        where T : class
    {
        public PagedResponseOffset(List<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            this.Data = data;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.TotalRecords = totalRecords;
            this.TotalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)pageSize);
            this.HasNextPage = this.PageNumber < this.TotalPages;
        }

        public int PageNumber { get; init; }

        public int PageSize { get; init; }

        public int TotalRecords { get; init; }

        public int TotalPages { get; init; }

        public bool HasNextPage { get; init; }

        public List<T> Data { get; init; }
    }
}
