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
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostDTO>()
                .ForMember(dest => dest.IsLiked, opt => opt.Ignore())
                .ForMember(dest => dest.IsBookmarked, opt => opt.Ignore())
                .ForMember(dest => dest.IsReposted, opt => opt.Ignore());

            CreateMap<PostCreateDTO, Post>()
                .ForMember(dest => dest.MediaUrls, opt =>
                    opt.MapFrom(src => src.MediaUrls ?? new List<string>()));
        }
    }
}
