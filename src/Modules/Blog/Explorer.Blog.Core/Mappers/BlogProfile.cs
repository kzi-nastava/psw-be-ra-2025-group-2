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
        
        CreateMap<Comment, CommentDto>()
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedAt));

        CreateMap<CommentDto, Comment>()
            .ForMember(dest => dest.LastModifiedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // CommentDto → Comment (MORA koristiti konstruktor)
        CreateMap<CommentDto, Comment>()
            .ConstructUsing(dto => new Comment(dto.BlogPostId, dto.UserId, dto.Text))
            .ForAllMembers(opts => opts.Ignore());
    }
}