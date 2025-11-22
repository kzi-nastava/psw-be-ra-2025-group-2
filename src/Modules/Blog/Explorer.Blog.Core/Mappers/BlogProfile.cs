using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<BlogPost, BlogPostDto>()
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.Url)));

        CreateMap<CreateBlogPostDto, BlogPost>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageUrls != null ? src.ImageUrls.Select(url => new BlogImage(url)).ToList() : new List<BlogImage>()));

        CreateMap<UpdateBlogPostDto, BlogPost>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageUrls != null ? src.ImageUrls.Select(url => new BlogImage(url)).ToList() : new List<BlogImage>()));
    }
}