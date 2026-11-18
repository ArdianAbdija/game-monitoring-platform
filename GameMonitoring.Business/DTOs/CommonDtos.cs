namespace GameMonitoring.Business.DTOs;


/// Generic DTO used to return paginated results together with paging metadata such as total item count and current page.

public class PagedResultDto<T>
{
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int Count { get; set; }
    
    /// Performs the new operation for this class.
   
    public List<T> Items { get; set; } = new();
}


/// Small DTO used for simple lookup values, such as genres and game types, where only an ID and name are needed.

public class LookupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
