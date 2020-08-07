using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
using ProjectPlanner.API.Data;
using ProjectPlanner.API.Dtos;
using ProjectPlanner.API.Models;

namespace ProjectPlanner.API.Controllers
{
    [Route("api/{userId}/projects/{projectId}/[controller]")]
    [Authorize]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public TodoController(IProjectRepository projectRepository, UserManager<User> userManager, IMapper mapper, IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        /// <summary>
        ///  Get a todo that matches the id.
        /// </summary>
        /// <param name="userId"> Id of the current user. </param>
        /// <param name="todoId"> Id of the todo to get. </param>
        /// <returns>
        ///  A DTO of the todo obtained.
        /// </returns>
        [HttpGet("{todoId}", Name = "GetTodo")]
        public async Task<IActionResult> GetTodo(string userId, int todoId)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            var todo = await _projectRepository.GetTodo(todoId);

            if (todo == null)
                return NotFound();

            var todoToReturn = _mapper.Map<TodoForListDto>(todo);

            return Ok(todoToReturn);
        }


        /// <summary>
        ///  Creates a todo entity.
        /// </summary>
        /// <param name="userId"> Id of the current user. </param>
        /// <param name="projectId"> Id of the current project. </param>
        /// <param name="todoForCreationDto"> DTO with the properties for creating the todo.</param>
        /// <returns>
        ///  A CreatedAtRoute result with:
        ///   - The route for retrieve the todo.
        ///   - The parameters needed to retrieve it.
        ///   - The returning DTO of the todo.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateTodo(string userId, int projectId, TodoForCreationDto todoForCreationDto)
        {
            if (userId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            var project = await _projectRepository.GetProject(projectId);

            // Check if the project exist.
            if (project == null)
                return BadRequest("The project cannot be found");

            // Check if the user is the owner or collaborator of the project.
            if (project.OwnerId != userId)
                return Unauthorized("You must be the owner of the project to add Todos");

            todoForCreationDto.ProjectId = projectId;

            var todo = _mapper.Map<Todo>(todoForCreationDto);

            _projectRepository.Add(todo);

            // Update the Modified property of the project.
            _projectRepository.UpdateLastModified(project);

            if (await _projectRepository.SaveAll())
            {
                var todoForReturn = _mapper.Map<TodoForListDto>(todo);

                return CreatedAtRoute(
                    routeName: "GetTodo",
                    routeValues: new { userId = userId, projectId, todoId = todo.Id },
                    value: todoForReturn
                    );
            }
                

            return BadRequest();
        }
            

        /// <summary>
        /// Delete a Todo that matches the Id.
        /// </summary>
        /// <param name="userId"> Id of the user. </param>
        /// <param name="projectId"> Id of the project containing the Todo. </param>
        /// <param name="todoId"> Id of the Todo to be deleted. </param>
        /// <returns> No Content Result. </returns>
        [HttpDelete("{todoId}")]
        public async Task<IActionResult> DeleteTodo(string userId, int projectId, int todoId)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            var project = await _projectRepository.GetProject(projectId);

            var todo = await _projectRepository.GetTodo(todoId);

            if (project == null || todo == null)
                return NotFound();

            if (project.OwnerId != userId)
                return Unauthorized("You have no permission to perform this action.");

            _projectRepository.Delete(todo);

            if (await _projectRepository.SaveAll())
                return NoContent();

            return BadRequest("Something happened.");
        }

        /// <summary>
        ///  Creates a message entity between a Todo and a user.
        /// </summary>
        /// <param name="userId"> Id of the user creating the message. </param>
        /// <param name="projectId"> Id of the project containing the Todo. </param>
        /// <param name="todoId"> Id of the Todo. </param>
        /// <param name="message"> Message as a string. </param>
        /// <returns>
        ///   A CreatedAtRoute result containing:
        ///   - The route for retrieve the message.
        ///   - The paramaters needed to retrieve it.
        ///   - The returning DTO of the message.
        /// </returns>
        [HttpPost("{todoId}")]
        public async Task<IActionResult> CreateMessage (string userId, int projectId, int todoId, [FromBody]string message)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            // Check for authorization.
            if (!await isOwnerOrCollaborator(userId, projectId))
                return Unauthorized("Yo have no permission for this action");

            var todo = await _projectRepository.GetTodo(todoId);
            var user = await _userRepository.GetUser(userId);


            var messageToCreate = new TodoMessage()
            {
                Message = message,
                UserId = userId,
                TodoId = todoId,
                User = user,
                Todo = todo
            };

            _projectRepository.Add(messageToCreate);


            var messageToReturn = _mapper.Map<TodoMessageForListDto>(messageToCreate);
            if (await _projectRepository.SaveAll())
            {
                return CreatedAtRoute(
                    routeName: "GetMessage",
                    routeValues: new { userId, projectId, todoId, messageId = messageToCreate.Id },
                    value: messageToReturn
                    );
            }
            return BadRequest("Something happened, try again");
        }


        /// <summary>
        ///  Get the message entity
        /// </summary>
        /// <param name="userId"> Id of the user that created the message. </param>
        /// <param name="projectId"> Id of the project containing the Todo of the message. </param>
        /// <param name="todoId"> Id of the Todo that contains the message. </param>
        /// <param name="messageId"> Id of the message. </param>
        /// <returns> A DTO of the message. </returns>
        [HttpGet("{todoId}/message/{messageId}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(string userId, int projectId, int todoId, int messageId)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            // Check for authorization.
            if (!await isOwnerOrCollaborator(userId, projectId))
                return Unauthorized();

            // Get the message from the repository.
            var message = await _projectRepository.GetMessage(messageId);


            if (message == null)
                return NotFound();

            var messageToReturn = _mapper.Map<TodoMessageForListDto>(message);
            return Ok(messageToReturn);
        }



        /// <summary>
        ///  Partially updates a Todo, changing the status.
        /// </summary>
        /// <param name="userId"> Id of the current user. </param>
        /// <param name="projectId"> Id of the project containing the Todo. </param>
        /// <param name="todoId"> Id of the Todo. </param>
        /// <param name="status"> New status as a string. </param>
        /// <returns></returns>
        [HttpPatch("{todoId}")]
        public async Task<IActionResult> ChangeStatus(string userId, int projectId, int todoId, [FromBody]string status)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();
            

            if (!await isOwnerOrCollaborator(userId, projectId))
                return Unauthorized();

            var todo = await _projectRepository.GetTodo(todoId);

            if (todo == null)
                return NotFound();

            todo.Status = status;

            _projectRepository.Edit(todo);

            if (await _projectRepository.SaveAll())
                return NoContent();

            return BadRequest();
        }

        private async Task<bool> isOwnerOrCollaborator(string userId, int projectId)
        {
            var project = await _projectRepository.GetProject(projectId);

            if (project.OwnerId == userId)
                return true;

            foreach (var collaborator in project.Collaborations)
            {
                if (collaborator.UserId == userId)
                    return true;
            }

            return false;
        }

    }
}
