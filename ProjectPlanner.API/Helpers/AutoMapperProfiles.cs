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
            CreateMap<User, UserForDetailedDto>();
            CreateMap<User, UserForListDto>();

            CreateMap<Friend, FriendToReturnDto>();

            CreateMap<ProjectForCreationDto, Project>();
            CreateMap<Project, ProjectForListDto>()
                .ForMember(dest => dest.Collaborators, opt => opt.MapFrom(src => src.Collaborations.Select(c => c.User).ToList()))
                .ForMember(dest => dest.CompletedPercentage, opt => opt.MapFrom(src => src.Todos.CalculatePercentage())) // Get the percentage of completed todos.
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.Todos.GetLastMessage())); // Get the last message created between all todos.

            CreateMap<Todo, TodoForListDto>();         
            CreateMap<TodoForCreationDto, Todo>();

            CreateMap<TodoMessage, TodoMessageForListDto>()
                .ForMember(dest => dest.UserKnownAs, opt => opt.MapFrom(src => src.User.KnownAs))
                .ForMember(dest => dest.UserPhotoUrl, opt => opt.MapFrom(src => src.User.PhotoUrl));
        }
    }
}
