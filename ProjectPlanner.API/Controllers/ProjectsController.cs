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


namespace ProjectPlanner.API.Controllers
{
    [Route("api/{userId}/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogUserActivity))] // Whenever a request is made to this controller. The service filter will update the user's LastActive property.
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


        /// <summary>
        ///  Get a list of projects from a user.
        /// </summary>
        /// <param name="userId"> Id of the current user </param>
        /// <param name="projectParams"> Parameters to query the list </param>
        /// <returns> 
        /// A Collection of DTOS of projects.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetProjects(string userId, [FromQuery]ProjectParams projectParams)
        {
            if (userId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            // Get the projects for a user with the query parameters.
            var projects = await _projectRepository.GetProjects(userId, projectParams);

            // Map the projects to the returning DTO.
            var projectsToReturn = _mapper.Map<IEnumerable<ProjectForListDto>>(projects);

            return Ok(projectsToReturn);
        }


        /// <summary>
        ///  Creates a project entity into the database.
        /// </summary>
        /// <param name="userId"> Id of the project's owner. </param>
        /// <param name="projectForCreationDto"> DTO with the properties for creating the project. </param>
        /// <returns>
        /// A CreatedAtRoute result containing:
        ///     -The name of the route for retrieving the project.
        ///     -The parameters needed to retrieve it.
        ///     -The DTO of the created project.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateProject (string userId, ProjectForCreationDto projectForCreationDto)
        {
            if (userId !=  _userManager.GetUserId(User))
                return Unauthorized();
           
            // Assign the user id to the project to establish the relationship.
            projectForCreationDto.OwnerId = userId;

            // Map the project from the recieved DTO.
            var project = _mapper.Map<Project>(projectForCreationDto);


            project.Owner = await _userRepository.GetUser(userId);

            _projectRepository.Add(project);

            // Map the project to the returning DTO.
            var projectToReturn = _mapper.Map<ProjectForListDto>(project);

            if (await _projectRepository.SaveAll())
            {
                return CreatedAtRoute("GetProject", new { controller = "Projects", userId, projectId = project.Id }, projectToReturn);
            }

            return BadRequest("Something happened");
        }      


        /// <summary>
        ///  Edit the project properties.
        /// </summary>
        /// <param name="userId"> Id of the project's owner. </param>
        /// <param name="projectId"> id of the project. </param>
        /// <param name="project"> DTO with the properties to be updated. </param>
        /// <returns>
        /// No Content result.
        /// </returns>
        [HttpPut("{projectId}")]
        public async Task<IActionResult> EditProject(string userId, int projectId, ProjectForCreationDto project)
        {
            if (userId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }
            // Get the project from the repository.
            var projectFromRepo = await _projectRepository.GetProject(projectId);
            if (projectFromRepo == null)
                return BadRequest("The project you are trying to edit does not exist");

            // Manually assign the new values of the properties.
            projectFromRepo.Title = project.Title;
            projectFromRepo.ShortDescription = project.ShortDescription;
            projectFromRepo.LongDescription = project.LongDescription;
            projectFromRepo.EstimatedDate = project.EstimatedDate;
            projectFromRepo.Modified = DateTime.Now;

            _projectRepository.Edit(projectFromRepo);

            // If there was a change in the database, return a No Content result.
            if (await _projectRepository.SaveAll())
                return NoContent();
            return BadRequest();
        }


        /// <summary>
        ///  Removes the project entity from the database.
        /// </summary>
        /// <param name="userId"> Id of the project's owner. </param>
        /// <param name="projectId"> Id of the project to be deleted. </param>
        /// <returns> 
        /// No Content result.
        /// </returns>
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject (string userId, int projectId)
        {
            if (userId !=  _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            // Get the project from the repository.
            var project = await _projectRepository.GetProject(projectId);

            // Check if the user is the project's owner.
            if (project.OwnerId != userId)
                return Unauthorized("You are not the owner of this project");

             _projectRepository.Delete(project);

            // If there was a change in the database, return a No Content result.
            if (await _projectRepository.SaveAll())
                return NoContent();

            return BadRequest("Error deleting the project");
        }



        /// <summary>
        ///  Get a project that matches the id.
        /// </summary>
        /// <param name="userId"> Id of the requesting user. </param>
        /// <param name="projectId"> Id of the project to get. </param>
        /// <returns> 
        /// A DTO of a project.
        /// </returns>
        [HttpGet("{projectId}", Name = "GetProject")]
        public async Task<IActionResult> GetProject(string userId, int projectId)
        {
            if (userId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            // Get the project from the repository.
            var project = await _projectRepository.GetProject(projectId);

            if (project == null)
                return NotFound();

            // Check if the user has permission to get the project.
            if (!isOwnerOrCollaborator(userId, project))
                return Unauthorized("You have no permission to see this project");

            // Map the project into the returning DTO.
            var projectToReturn = _mapper.Map<ProjectForListDto>(project);

            return Ok(projectToReturn);

        }



        /// <summary>
        ///  Create a Collaboration entity between a project and a user.
        /// </summary>
        /// <param name="userId"> Id of the owner of the project. </param>
        /// <param name="projectId"> Id of the project where a colalborator is going to be added. </param>
        /// <param name="collaboratorId"> Id of the user to be added as a collaborator. </param>
        /// <returns> 
        ///     A CreatedAtRoute result with:
        ///     - the route for retrieving the project.
        ///     - the parameters needed to retrieve it.
        ///     - the collaboration entity.
        /// </returns>
        [HttpPost("{projectId}/collaborators/{collaboratorId}")]
        public async Task<IActionResult> AddCollaboration(string userId, int projectId, string collaboratorId)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            // Get project from the repository.
            var project = await _projectRepository.GetProject(projectId);

            // Check if the user sending the request is the owner of the project.
            if (project.OwnerId != userId)
                return Unauthorized("You have no permission for this action");

            // Check if there is an existing Collaboration entity between the project and the collaborator.
            if (await _projectRepository.GetCollaboration(projectId, collaboratorId) != null)
                return BadRequest("The user is already a collaborator in this project");

            var collaboration = new Collaboration
            {
                UserId = collaboratorId,
                ProjectId = projectId
            };

            // Add the Collaboration entity to the database.
            _projectRepository.Add(collaboration);

            // Update the Modified property of the project.
            _projectRepository.UpdateLastModified(project);

            if (await _projectRepository.SaveAll())
            {  
                return CreatedAtRoute("GetCollaboration", new { userId, projectId, collaboratorId }, collaboration);
            }               
            return BadRequest();
        }


        /// <summary>
        ///  Gets the Collaboration entity between a project and a user.
        /// </summary>
        /// <param name="projectId"> Id of the project. </param>
        /// <param name="collaboratorId"> Id of the user as collaborator. </param>
        /// <returns>
        /// A Collaboration entity.
        /// </returns>
        [HttpGet("{projectId}/collaborators/{collaboratorId}", Name = "GetCollaboration")]
        public async Task<IActionResult> GetCollaboration(int projectId, string collaboratorId)
        {
            // Get the collaboration from the repository.
            var collaboration = await _projectRepository.GetCollaboration(projectId, collaboratorId);

            // Check if the collaboration exist.
            if (collaboration != null)
                return Ok(collaboration);

            return NotFound("There is no collaboration between the project and user");
        }


        /// <summary>
        ///  Removes the Collaboration entity from the database.
        /// </summary>
        /// <param name="userId"> Id of the collaboration's project owner. </param>
        /// <param name="projectId"> Id of the collaboration's project. </param>
        /// <param name="collaboratorId"> Id of the project's collaborator. </param>
        /// <returns>
        ///  No Content result.
        /// </returns>
        [HttpDelete("{projectId}/collaborators/{collaboratorId}")]
        public async Task<IActionResult> DeleteCollaboration(string userId, int projectId, string collaboratorId)
        {
            var project = await _projectRepository.GetProject(projectId);

            if (project == null)
                return NotFound();

            // Check if the user sending the request is the owner of the project.
            else if (project.OwnerId != userId)
                return Unauthorized("You have no permission for this action");

            // Get the collaboration from the repository.
            var collaboration = await _projectRepository.GetCollaboration(projectId, collaboratorId);

            // Check if the collaboration exist.
            if (collaboration == null)
                return NotFound();

            // Remove the collaboration from the database.
             _projectRepository.Delete(collaboration);

            // Update the Modified property of the project.
            _projectRepository.UpdateLastModified(project);

            if (await _projectRepository.SaveAll())
            {
                return NoContent();
            }              
            return BadRequest();
        }


        /// <summary>
        ///  Check if the user is owner of collaborator of a provided project.
        /// </summary>
        /// <param name="userId"> Id of the user. </param>
        /// <param name="project"> Project entity. </param>
       
        private bool isOwnerOrCollaborator(string userId, Project project)
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
