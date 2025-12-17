using System.Collections.Generic;

namespace MonResto.Domain.DTOs;

public record MenuDto
{
    public int MenuId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IEnumerable<ArticleDto> Articles { get; init; } = new List<ArticleDto>();
}

public record MenuCreateDto(string Title, string? Description, IEnumerable<int> ArticleIds);
