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
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            CreateMap<Subscription, UserProfileDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Following.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Following.Username))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Following.AvatarUrl))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Following.Bio));
        }
    }
}
