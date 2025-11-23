using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonResto.Domain.DTOs;
using MonResto.Domain.Entities;
using MonResto.Domain.Interfaces;

namespace MonResto.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IMapper _mapper;

    public CartController(ICartRepository cartRepository, IArticleRepository articleRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _articleRepository = articleRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItemDto>>> Get()
    {
        var userId = User.Identity?.Name;
        if (userId is null) return Unauthorized();
        var items = await _cartRepository.GetCart(userId);
        return Ok(_mapper.Map<IEnumerable<CartItemDto>>(items));
    }

    [HttpGet("summary")]
    public async Task<ActionResult<CartSummaryDto>> Summary()
    {
        var userId = User.Identity?.Name;
        if (userId is null) return Unauthorized();
        var items = await _cartRepository.GetCart(userId);
        var totalQuantity = items.Sum(ci => ci.Quantity);
        var totalPrice = items.Sum(ci => (ci.Article?.Price ?? 0) * ci.Quantity);
        return Ok(new CartSummaryDto(totalQuantity, totalPrice));
    }

    [HttpPost]
    public async Task<ActionResult<CartItemDto>> Post([FromBody] CartItemCreateDto dto)
    {
        var userId = User.Identity?.Name;
        if (userId is null) return Unauthorized();

        var existing = await _cartRepository.GetItem(userId, dto.ArticleId);
        if (existing != null)
        {
            existing.Quantity += dto.Quantity;
            await _cartRepository.Update(existing);
            return Ok(_mapper.Map<CartItemDto>(existing));
        }

        var item = new CartItem { ArticleId = dto.ArticleId, Quantity = dto.Quantity, UserId = userId };
        await _cartRepository.Add(item);
        var loaded = await _cartRepository.GetItem(userId, dto.ArticleId);
        return CreatedAtAction(nameof(Get), _mapper.Map<CartItemDto>(loaded));
    }

    [HttpPut("{cartItemId:int}")]
    public async Task<ActionResult<CartItemDto>> Put(int cartItemId, [FromBody] CartItemCreateDto dto)
    {
        var userId = User.Identity?.Name;
        if (userId is null) return Unauthorized();
        var item = await _cartRepository.GetById(userId, cartItemId);
        if (item is null) return NotFound();
        item.Quantity = dto.Quantity;
        await _cartRepository.Update(item);
        return Ok(_mapper.Map<CartItemDto>(item));
    }

    [HttpDelete("{cartItemId:int}")]
    public async Task<IActionResult> Delete(int cartItemId)
    {
        var deleted = await _cartRepository.Delete(cartItemId);
        return deleted ? NoContent() : NotFound();
    }
}
