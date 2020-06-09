using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectPlanner.API.Data;
using ProjectPlanner.API.Dtos;
using ProjectPlanner.API.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectPlanner.API.Controllers
{
    [Route("api/user/{userId}/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public ProjectsController(IProjectRepository projectRepository, IMapper mapper, UserManager<User> userManager, IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _userManager = userManager;
            _userRepository = userRepository;
        }            

        [HttpGet]
        public async Task<IActionResult> GetProjects(string userId)
        {
            var user =  _userManager.GetUserId(User);

            if (userId != _userManager.GetUserId(User))
            {
                return Unauthorized(userId + " " + user);
            }

            var projects = await _projectRepository.GetProjects(userId);

            var projectsToReturn = _mapper.Map<IEnumerable<ProjectForListDto>>(projects);

            return Ok(projectsToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject (string userId, ProjectForCreationDto projectForCreationDto)
        {
            if (userId !=  _userManager.GetUserId(User))
                return Unauthorized();
           

            projectForCreationDto.OwnerId = userId;

            var project = _mapper.Map<Project>(projectForCreationDto);

            project.Owner = await _userRepository.GetUser(userId);

            _projectRepository.CreateProject(project);

            if (await _projectRepository.SaveAll())
            {
                return CreatedAtRoute("GetProject", new { controller = "Projects", userId, projectId = project.Id }, project);
            }

            return BadRequest("Something happened");
            

        }

        [HttpGet("{projectId}", Name = "GetProject")]
        public async Task<IActionResult> GetProject(string userId, int projectId)
        {
            if (userId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            var project = await _projectRepository.GetProject(projectId);

            if (project == null)
                return NotFound();

            return Ok(project);

        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject (string userId, int projectId)
        {
            if (userId !=  _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

             _projectRepository.DeleteProject(projectId);

            if (await _projectRepository.SaveAll())
                return NoContent();

            return BadRequest("Error deleting the project");
        }
    }
}
