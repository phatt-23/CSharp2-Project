namespace CoworkingApp.Models.DTOModels;

public class PaginationResponseDto 
{
    public required int PageNumber { get; set; } 
    public required int PageSize { get; set; } 
    public required int TotalCount { get; set; }
}
