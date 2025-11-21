using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MonResto.Domain.DTOs;
using MonResto.Domain.Entities;
using MonResto.Domain.Interfaces;

namespace MonResto.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public CategoriesController(ICategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> Get()
    {
        var categories = await _repository.GetAll();
        return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Get(int id)
    {
        var category = await _repository.GetById(id);
        return category is null ? NotFound() : Ok(_mapper.Map<CategoryDto>(category));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Post([FromBody] CategoryCreateDto dto)
    {
        var created = await _repository.Add(_mapper.Map<Category>(dto));
        return CreatedAtAction(nameof(Get), new { id = created.CategoryId }, _mapper.Map<CategoryDto>(created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Put(int id, [FromBody] CategoryCreateDto dto)
    {
        var category = await _repository.GetById(id);
        if (category is null) return NotFound();
        _mapper.Map(dto, category);
        var updated = await _repository.Update(category);
        return Ok(_mapper.Map<CategoryDto>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.Delete(id);
        return deleted ? NoContent() : NotFound();
    }
}
