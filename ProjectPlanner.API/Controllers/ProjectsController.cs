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
using ProjectPlanner.API.Helpers;
using ProjectPlanner.API.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectPlanner.API.Controllers
{
    [Route("api/{userId}/[controller]")]
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
        public async Task<IActionResult> GetProjects(string userId, [FromQuery]ProjectParams projectParams)
        {
            if (userId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            var projects = await _projectRepository.GetProjects(userId, projectParams);

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

        [HttpPut("{projectId}")]
        public async Task<IActionResult> EditProject(string userId, int projectId, ProjectForCreationDto project)
        {
            var projectFromRepo = await _projectRepository.GetProject(projectId);
            if (projectFromRepo == null)
                return BadRequest("The project you are trying to edit does not exist");

            projectFromRepo.Title = project.Title;
            projectFromRepo.ShortDescription = project.ShortDescription;
            projectFromRepo.LongDescription = project.LongDescription;
            projectFromRepo.EstimatedDate = project.EstimatedDate;
            projectFromRepo.Modified = DateTime.Now;

            _projectRepository.EditProject(projectFromRepo);

            if (await _projectRepository.SaveAll())
                return NoContent();
            return BadRequest();
        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject (string userId, int projectId)
        {
            if (userId !=  _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            var project = await _projectRepository.GetProject(projectId);

            if (project.OwnerId != userId)
                return Unauthorized("You are not the owner of this project");

             _projectRepository.DeleteProject(project);

            if (await _projectRepository.SaveAll())
                return NoContent();

            return BadRequest("Error deleting the project");
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

            if (!isOwnerOrCollaborator(userId, project))
                return Unauthorized("You have no permission to see this project");

            var projectToReturn = _mapper.Map<ProjectForListDto>(project);

            return Ok(projectToReturn);

        }

        [HttpPost("{projectId}/collaborators/{collaboratorId}")]
        public async Task<IActionResult> AddCollaboration(string userId, int projectId, string collaboratorId)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            var project = await _projectRepository.GetProject(projectId);

            if (project.OwnerId != userId)
                return Unauthorized();
            if (await _projectRepository.GetCollaboration(projectId, collaboratorId) != null)
                return BadRequest("The user is already a collaborator in this project");

            var collaboration = new Collaboration
            {
                UserId = collaboratorId,
                ProjectId = projectId
            };

            _projectRepository.AddCollaboration(collaboration);         

            if (await _projectRepository.SaveAll())
                return CreatedAtRoute("GetCollaboration", new { userId, projectId, collaboratorId }, collaboration);

            return BadRequest();
        }

        [HttpGet("{projectId}/collaborators/{collaboratorId}", Name = "GetCollaboration")]

        public async Task<IActionResult> GetCollaboration(int projectId, string collaboratorId)
        {
            var collaboration = await _projectRepository.GetCollaboration(projectId, collaboratorId);

            if (collaboration != null)
                return Ok(collaboration);

            return NotFound();
        }

        [HttpDelete("{projectId}/collaborators/{collaboratorId}")]
        public async Task<IActionResult> DeleteCollaboration(string userId, int projectId, string collaboratorId)
        {
            var project = await _projectRepository.GetProject(projectId);

            if (project == null)
                return NotFound();

            else if (project.OwnerId != userId)
                return Unauthorized();

            var collaboration = await _projectRepository.GetCollaboration(projectId, collaboratorId);

            if (collaboration == null)
                return NotFound();

             _projectRepository.DeleteCollaboration(collaboration);

            if (await _projectRepository.SaveAll())
                return NoContent();

            return BadRequest();
        }

        public bool isOwnerOrCollaborator(string userId, Project project)
        {
            if (project.OwnerId == userId)
                return true;
            
            foreach(var collaborator in project.Collaborations)
            {
                if (collaborator.UserId == userId)
                    return true;
            }

            return false;
        }
    }
}
