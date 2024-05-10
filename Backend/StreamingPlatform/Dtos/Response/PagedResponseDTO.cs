namespace StreamingPlatform.Dtos.Response
{
    /// <summary>
    /// Represents a response containing a paginated list of data.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the list.</typeparam>
    public record PagedResponseDTO<T>
    {
        /// <summary>
        /// The current page number in the paged response.
        /// </summary>
        public int PageNumber { get; init; }

        /// <summary>
        /// The number of items included in each page of the response.
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// The total number of records available for the data being paged.
        /// </summary>
        public int TotalRecords { get; init; }

        /// <summary>
        /// The total number of pages available based on the PageSize and TotalRecords.
        /// </summary>
        public int TotalPages { get; init; }

        /// <summary>
        /// Indicates whether there are more pages available after the current page.
        /// </summary>
        public bool HasNextPage { get; init; }

        /// <summary>
        /// List of data items for the current page.
        /// 
        /// **NOTE:** This field is marked `get; init;` to allow initialization after construction 
        /// (e.g., by a service layer populating the data). 
        /// 
        /// The warning directives (`#pragma warning`) are used to suppress a compiler warning 
        /// because the field is non-nullable but can be left uninitialized during construction.
        /// </summary>
#pragma warning disable CS8618
        public List<T> Data { get; init; }
#pragma warning restore CS8618 
    }
}