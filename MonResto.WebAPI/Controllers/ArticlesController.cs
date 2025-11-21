using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MonResto.Domain.DTOs;
using MonResto.Domain.Entities;
using MonResto.Domain.Interfaces;

namespace MonResto.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleRepository _repository;
    private readonly IMapper _mapper;

    public ArticlesController(IArticleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> Get()
    {
        var items = await _repository.GetAll();
        return Ok(_mapper.Map<IEnumerable<ArticleDto>>(items));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ArticleDto>> Get(int id)
    {
        var article = await _repository.GetById(id);
        return article is null ? NotFound() : Ok(_mapper.Map<ArticleDto>(article));
    }

    [HttpGet("by-category/{categoryId:int}")]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> ByCategory(int categoryId)
    {
        var items = await _repository.GetByCategory(categoryId);
        return Ok(_mapper.Map<IEnumerable<ArticleDto>>(items));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> Search([FromQuery] string term)
    {
        var items = await _repository.SearchByName(term);
        return Ok(_mapper.Map<IEnumerable<ArticleDto>>(items));
    }

    [HttpPost]
    public async Task<ActionResult<ArticleDto>> Post([FromBody] ArticleCreateDto dto)
    {
        var created = await _repository.Add(_mapper.Map<Article>(dto));
        return CreatedAtAction(nameof(Get), new { id = created.ArticleId }, _mapper.Map<ArticleDto>(created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ArticleDto>> Put(int id, [FromBody] ArticleCreateDto dto)
    {
        var article = await _repository.GetById(id);
        if (article is null) return NotFound();
        _mapper.Map(dto, article);
        var updated = await _repository.Update(article);
        return Ok(_mapper.Map<ArticleDto>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.Delete(id);
        return deleted ? NoContent() : NotFound();
    }
}
