namespace CoworkingApp.Models.DTOModels;

public class PaginationRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PaginationResponseDto 
{
    public required int PageNumber { get; set; } 
    public required int PageSize { get; set; } 
    public required int TotalCount { get; set; }
}
