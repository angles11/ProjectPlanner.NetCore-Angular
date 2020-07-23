using AutoMapper;
using ProjectPlanner.API.Dtos;
using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserForRegisterDto, User>();
            CreateMap<User, UserForReturnDto>();
            CreateMap<User, UserForListDto>();
            CreateMap<Friend, FriendToReturnDto>();
            CreateMap<ProjectForCreationDto, Project>();
            CreateMap<Project, ProjectForListDto>();           
            //https://stackoverflow.com/questions/6781795/automapper-mapping-a-collection-of-object-to-a-collection-of-strings

            CreateMap<Project, ProjectForListDto>()
                .ForMember(dest => dest.Collaborators, opt => opt.MapFrom(src => src.Collaborations.Select(c => c.User).ToList()));

            CreateMap<Todo, TodoForListDto>();         
            CreateMap<TodoForCreationDto, Todo>();

            CreateMap<TodoMessage, TodoMessageForListDto>()
                .ForMember(dest => dest.UserKnownAs, opt => opt.MapFrom(src => src.User.KnownAs))
                .ForMember(dest => dest.UserPhotoUrl, opt => opt.MapFrom(src => src.User.PhotoUrl));
        }
    }
}
