using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MonResto.Domain.DTOs;
using MonResto.Domain.Entities;
using MonResto.Domain.Interfaces;

namespace MonResto.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuRepository _repository;
    private readonly IMapper _mapper;

    public MenuController(IMenuRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuDto>>> Get()
    {
        var menus = await _repository.GetAll();
        return Ok(_mapper.Map<IEnumerable<MenuDto>>(menus));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MenuDto>> Get(int id)
    {
        var menu = await _repository.GetById(id);
        return menu is null ? NotFound() : Ok(_mapper.Map<MenuDto>(menu));
    }

    [HttpPost]
    public async Task<ActionResult<MenuDto>> Post([FromBody] MenuCreateDto dto)
    {
        var menu = await _repository.Add(_mapper.Map<Menu>(dto));
        foreach (var articleId in dto.ArticleIds)
        {
            await _repository.AddArticle(menu.MenuId, articleId);
        }
        menu = await _repository.GetById(menu.MenuId) ?? menu;
        return CreatedAtAction(nameof(Get), new { id = menu.MenuId }, _mapper.Map<MenuDto>(menu));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MenuDto>> Put(int id, [FromBody] MenuCreateDto dto)
    {
        var menu = await _repository.GetById(id);
        if (menu is null) return NotFound();
        _mapper.Map(dto, menu);
        await _repository.Update(menu);
        foreach (var link in menu.MenuArticles.ToList())
        {
            await _repository.RemoveArticle(id, link.ArticleId);
        }
        foreach (var articleId in dto.ArticleIds)
        {
            await _repository.AddArticle(id, articleId);
        }
        menu = await _repository.GetById(id) ?? menu;
        return Ok(_mapper.Map<MenuDto>(menu));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.Delete(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{menuId:int}/articles/{articleId:int}")]
    public async Task<IActionResult> AddArticle(int menuId, int articleId)
    {
        var success = await _repository.AddArticle(menuId, articleId);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{menuId:int}/articles/{articleId:int}")]
    public async Task<IActionResult> RemoveArticle(int menuId, int articleId)
    {
        var success = await _repository.RemoveArticle(menuId, articleId);
        return success ? NoContent() : NotFound();
    }
}
