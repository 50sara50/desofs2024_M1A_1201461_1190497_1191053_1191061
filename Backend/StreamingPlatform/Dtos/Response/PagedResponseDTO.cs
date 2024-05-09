
namespace StreamingPlatform.Dtos.Response
{
    public record PagedResponseDTO<T>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public int TotalRecords { get; init; }

        public int TotalPages { get; init; }

        public bool HasNextPage { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public List<T> Data { get; init; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
