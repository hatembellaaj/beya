using System.Collections.Generic;

namespace MonResto.Domain.DTOs;

public record MenuDto(int MenuId, string Title, string? Description, IEnumerable<ArticleDto> Articles);
public record MenuCreateDto(string Title, string? Description, IEnumerable<int> ArticleIds);
