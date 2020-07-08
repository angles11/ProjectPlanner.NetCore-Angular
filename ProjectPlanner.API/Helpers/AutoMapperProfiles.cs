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
            CreateMap<ProjectForCreationDto, Project>();
            CreateMap<Project, ProjectForListDto>();

            CreateMap<Friend, FriendToReturnDto>();
        }
    }
}
