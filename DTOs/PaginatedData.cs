namespace FoodioAPI.DTOs;

/// <summary>
/// Generic class for paginated data responses
/// </summary>
/// <typeparam name="T">Type of the data items</typeparam>
public class PaginatedData<T>
{
    /// <summary>
    /// The data items for the current page
    /// </summary>
    public IEnumerable<T> Data { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Constructor for backward compatibility
    /// </summary>
    /// <param name="data">The data items</param>
    /// <param name="totalCount">Total number of items</param>
    public PaginatedData(IEnumerable<T> data, int totalCount)
    {
        Data = data;
        TotalCount = totalCount;
        Page = 1;
        PageSize = data.Count();
        TotalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / PageSize) : 0;
    }

    /// <summary>
    /// Constructor with full pagination information
    /// </summary>
    /// <param name="data">The data items for current page</param>
    /// <param name="totalCount">Total number of items</param>
    /// <param name="page">Current page number</param>
    /// <param name="pageSize">Number of items per page</param>
    public PaginatedData(IEnumerable<T> data, int totalCount, int page, int pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;
    }

    /// <summary>
    /// Default constructor for object initialization
    /// </summary>
    public PaginatedData()
    {
        Data = new List<T>();
        TotalCount = 0;
        Page = 1;
        PageSize = 10;
        TotalPages = 0;
    }
}
