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
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDTO>()
                .ForMember(dest => dest.Replies, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore());

            CreateMap<CommentCreateDTO, Comment>();
        }
    }
}
