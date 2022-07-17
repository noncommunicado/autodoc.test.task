namespace autodoc.tasks.domain.Common;

public abstract class BasePaginationRequest
{
	public int PageSize { get; set; } = 10;
	public int Page { get; set; } = 1;
}