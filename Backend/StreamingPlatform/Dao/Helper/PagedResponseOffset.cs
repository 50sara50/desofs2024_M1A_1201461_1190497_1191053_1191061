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
        }

        public int PageNumber { get; }

        public int PageSize { get; }

        public int TotalRecords { get; }

        public int TotalPages { get; }

        public List<T> Data { get; }
    }

}
