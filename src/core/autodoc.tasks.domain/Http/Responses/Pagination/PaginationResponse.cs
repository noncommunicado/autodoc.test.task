namespace autodoc.tasks.domain.Http.Responses.Pagination;

public record PaginationResponse<T> (IEnumerable<T> Items, long Total = 0);