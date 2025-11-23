using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonResto.Domain.DTOs;
using MonResto.Domain.Entities;
using MonResto.Domain.Enums;
using MonResto.Domain.Interfaces;

namespace MonResto.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IMapper _mapper;

    public OrderController(IOrderRepository orderRepository, ICartRepository cartRepository, IArticleRepository articleRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _articleRepository = articleRepository;
        _mapper = mapper;
    }

    [HttpPatch("{orderId:int}/status"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
    {
        var existing = await _orderRepository.GetById(orderId);
        if (existing is null) return NotFound();

        var updated = await _orderRepository.UpdateStatus(orderId, dto.Status);
        var reloaded = await _orderRepository.GetById(updated.OrderId) ?? updated;
        return Ok(_mapper.Map<OrderDto>(reloaded));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> History()
    {
        var userId = User.Identity?.Name;
        if (userId is null) return Unauthorized();
        var orders = await _orderRepository.GetByUser(userId);
        return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create()
    {
        var userId = User.Identity?.Name;
        if (userId is null) return Unauthorized();

        var cartItems = await _cartRepository.GetCart(userId);
        if (!cartItems.Any()) return BadRequest("Panier vide");

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        foreach (var cart in cartItems)
        {
            var article = cart.Article ?? await _articleRepository.GetById(cart.ArticleId);
            if (article is null) continue;
            order.OrderItems.Add(new OrderItem
            {
                ArticleId = article.ArticleId,
                Quantity = cart.Quantity,
                UnitPrice = article.Price
            });
        }

        order.TotalPrice = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
        await _orderRepository.Add(order);
        await _cartRepository.ClearCart(userId);

        var created = await _orderRepository.GetById(order.OrderId) ?? order;
        return CreatedAtAction(nameof(History), _mapper.Map<OrderDto>(created));
    }
}
