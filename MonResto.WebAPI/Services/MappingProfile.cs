using AutoMapper;
using MonResto.Domain.DTOs;
using MonResto.Domain.Entities;

namespace MonResto.WebAPI.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryCreateDto, Category>();

        CreateMap<Article, ArticleDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
        CreateMap<ArticleCreateDto, Article>();

        CreateMap<Menu, MenuDto>()
            .ForMember(dest => dest.Articles, opt => opt.MapFrom(src => src.MenuArticles.Select(ma => ma.Article)));
        CreateMap<MenuCreateDto, Menu>();

        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.Article != null ? src.Article.Name : string.Empty))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Article != null ? src.Article.Price : 0));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.Article != null ? src.Article.Name : string.Empty));
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));
    }
}
