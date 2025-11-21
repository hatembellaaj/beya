namespace MonResto.Domain.DTOs;

public record CategoryDto(int CategoryId, string Name, string? Description);
public record CategoryCreateDto(string Name, string? Description);
