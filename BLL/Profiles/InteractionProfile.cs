using AutoMapper;
using BLL.DTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Profiles
{
    public class InteractionProfile : Profile
    {
        public InteractionProfile()
        {
            CreateMap<Like, UserProfileDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl));

            CreateMap<Bookmark, PostDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Post.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Post.AuthorId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Post.Content))
                .ForMember(dest => dest.MediaUrls, opt => opt.MapFrom(src => src.Post.MediaUrls))
                .ForMember(dest => dest.Hashtags, opt => opt.MapFrom(src => src.Post.Hashtags))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Post.LikeCount))
                .ForMember(dest => dest.RepostCount, opt => opt.MapFrom(src => src.Post.RepostCount))
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Post.CommentCount))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Post.CreatedAt))
                .ForMember(dest => dest.IsBookmarked, opt => opt.MapFrom(_ => true));

            CreateMap<Repost, PostDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Post.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Post.AuthorId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Post.Content))
                .ForMember(dest => dest.MediaUrls, opt => opt.MapFrom(src => src.Post.MediaUrls))
                .ForMember(dest => dest.Hashtags, opt => opt.MapFrom(src => src.Post.Hashtags))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Post.LikeCount))
                .ForMember(dest => dest.RepostCount, opt => opt.MapFrom(src => src.Post.RepostCount))
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Post.CommentCount))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Post.CreatedAt))
                .ForMember(dest => dest.IsReposted, opt => opt.MapFrom(_ => true));
        }
    }
}
