using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectPlanner.API.Data;
using ProjectPlanner.API.Dtos;
using ProjectPlanner.API.Helpers;
using ProjectPlanner.API.Models;
using ProjectPlanner.API.Services.PhotoUploader;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPhotoUploader _photoUploader;

        public UserController(UserManager<User> userManager, IMapper mapper, IUserRepository userRepository, IPhotoUploader photoUploader)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
            _photoUploader = photoUploader;
        }


        /// <summary>
        ///  Get the user entity.
        /// </summary>
        /// <param name="userId"> Id of the user. </param>
        /// <returns> A DTO of the user. </returns>
        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string userId)
        {

            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            var userFromRepo = await _userRepository.GetUser(userId);

            var userToReturn = _mapper.Map<UserForDetailedDto>(userFromRepo);

            return Ok(userToReturn);
        }

        /// <summary>
        ///     Get the list of friends for the requesting user.
        /// </summary>
        /// <param name="userId"> Id of the user.</param>
        /// <returns>
        ///   A collection of DTOS with the friends.
        /// </returns>    
        [HttpGet("{userId}/friends")]
        public async Task<IActionResult> GetFriends(string userId)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            // Get friendships of the logged user.
            var friendships = await _userRepository.GetFriendships(userId);

            var customMapper = new CustomMapper();
            
            // Map friends from the collection of friendships.
            var friends = customMapper.MapFriends(friendships, userId);

            // Map to the returning DTO.
            var friendsToReturn = _mapper.Map<ICollection<FriendToReturnDto>>(friends);

            return Ok(friendsToReturn);
        }



        /// <summary>
        ///  Gets the list of users with no friend relationship.
        /// </summary>
        /// <param name="userId"> Id of the user. </param>
        /// <param name="userParams"> Query parameters for filtering or sorting. </param>
        /// <returns> 
        /// Collection of DTOs of the users with the Pagination Headers. 
        /// </returns>
        [HttpGet("{userId}/users")]
        public async Task<IActionResult> GetUsers(string userId, [FromQuery]UserParams userParams)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            var users = await _userRepository.GetUsers(userId, userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }



        /// <summary>
        ///  Creates a friendship entity between two users
        ///  with a status of "pending"
        ///  to be accepted by the recipient user.
        /// </summary>
        /// <param name="userId"> The id of the user sending the friend request. </param>
        /// <param name="recipientId"> The id of the user recieving the friend request. </param>
        /// <returns> 
        /// A CreatedAtRoute result with:
        ///  - The route to get the entity.
        ///  - The parameters needed to get that entity.
        ///  - The entity created.
        /// </returns>

        [HttpPost("{userId}/friends/{recipientId}")]
        public async Task<IActionResult> AddFriend(string userId, string recipientId)
        {
            var user = await _userRepository.GetUser(userId);

            var recipient = await _userRepository.GetUser(recipientId);

            if (user == null | recipient == null)
                return NotFound();

            if (user.Id != _userManager.GetUserId(User))
                return Unauthorized();

            // Create a new instance of the friendship class with the needed values. 
            var friendship = new Friendship()
            {
                Sender = user,
                SenderId = userId,
                Recipient = recipient,
                RecipientId = recipientId,
                ActionUserId = userId,
            };

            // Check if there is an existing friendship entity between the users.
            if (await _userRepository.AlreadyFriends(userId, recipientId))
                return BadRequest("You are already friend with " + recipient.KnownAs);

            // Add the friendship entity to the database.
            await _userRepository.AddFriend(friendship);

            // If the entity was successfully added, return a CreatedAtRoute result.
            if (await _userRepository.SaveAll())
                return CreatedAtRoute("GetFriendship", new { controller = "User", userId, userId2 = recipientId }, friendship);

            return NoContent();
        }



        /// <summary>
        ///  Get the friendship entity between two users.
        /// </summary>
        /// <param name="userId"> The first user id. </param>
        /// <param name="userId2"> The second user id. </param>
        /// <returns> A friendship entity. </returns>
        [HttpGet("{userId}/friends/{userId2}", Name = "GetFriendship")]
        public async Task<IActionResult> GetFriendship(string userId, string userId2)
        {
           
            var friendship = await _userRepository.GetFriendship(userId, userId2);
            
            // Check if the friendship entity exist.
            if (friendship == null)
                return NotFound("There is no friendship relationship between these users");

            return Ok(friendship);
        }

        /// <summary>
        ///  Removes a friendship entity between two users.
        /// </summary>
        /// <param name="userId"> The first user id. </param>
        /// <param name="userId2"> The second user id. </param>
        /// <returns> A No Content result. </returns>
        [HttpDelete("{userId}/friends/{userId2}")]
        public async Task<IActionResult> DeleteFriendship(string userId, string userId2)
        {
            var friendship = await _userRepository.GetFriendship(userId, userId2);

            if (friendship == null)
            {
                return NotFound("There is no friendship relationship between these users");
            }

            // Remove the entity from the database.
             _userRepository.DeleteFriendship(friendship);

            // If the removal was successful, return a No Content result.
            if (await _userRepository.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Couldn't remove the friend request. Try again"); 
          
        }



        /// <summary>
        ///  Change the status of the friendship entity between two users to "Accepted".
        /// </summary>
        /// <param name="userId"> Id of the user that received the friend request. </param>
        /// <param name="userId2"> Id of the user that sent the friend request. </param>
        /// <returns></returns>
        [HttpPut("{userId}/friends/{userId2}")]
        public async Task<IActionResult> AcceptFriendship(string userId, string userId2)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            // Check if there is a friendship entity between the users.
            if (!await _userRepository.AlreadyFriends(userId, userId2))
                return BadRequest("There is no friend request from " + userId2);

            // Change the status to "Accepted".
            await _userRepository.AcceptFriendship(userId, userId2);

            // If the change was successful, return a No Content result.
            if (await _userRepository.SaveAll())
                return NoContent();

            return BadRequest("Something happened, try again.");
        }

        /// <summary>
        ///  Updates the information of a user account.
        /// </summary>
        /// <param name="userId"> Id of the user to update. </param>
        /// <param name="userForRegisterDto"> Properties to be changed. </param>
        /// <returns> A No Content Result </returns>
        [HttpPut("{userId}")]
        public async Task<IActionResult> EditAccountInformation(string userId, [FromBody]UserForEditDto dto)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            var user = await _userRepository.GetUser(userId);

            if (user == null)
                return NotFound("The user doesn't exist");

            user.KnownAs = dto.KnownAs;
            user.DateOfBirth = dto.DateOfBirth;
            user.Country = dto.Country;
            user.Position = dto.Position;
            user.Experience = dto.Experience;
            user.Gender = dto.Gender;
            user.Employer = dto.Employer;

             _userRepository.EditUser(user);

            if (await _userRepository.SaveAll())
                return NoContent();

            return BadRequest("Something happened, try again.");
        }

        /// <summary>
        ///  Deletes the last profile picture and uploads the new one.
        /// </summary>
        /// <param name="userId"> Id of the user. </param>
        /// <param name="photo"> Photo to upload as an IFormFile. </param>
        /// <returns></returns>
        [HttpPatch("{userId}")]
        public async Task<IActionResult> ChangeProfilePicture(string userId, [FromForm(Name = "photo")] IFormFile photo)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            var user = await _userRepository.GetUser(userId);

            if (user == null)
                return NotFound("The user doesn't exist");

            // Deletes the photo from the cloud.
            await _photoUploader.DeletePhotoAsync(user.PhotoUrl);

            // Uploads the new one and assigns the Url to the user's property.
            user.PhotoUrl = await _photoUploader.UploadPhotoAsync(photo);

            _userRepository.EditUser(user);

            if (await _userRepository.SaveAll())
                return NoContent();

            return BadRequest("Something happened.");
        }

        /// <summary>
        ///  Change the password of the user's account.
        /// </summary>
        /// <param name="userId"> Id of the user. </param>
        /// <param name="passwordToChangeDto"> Dto with the old and the new password.</param>
        /// <returns> A No Content result. </returns>
        [HttpPatch("{userId}/password")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody]PasswordToChangeDto passwordToChangeDto)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();
            var user = await _userRepository.GetUser(userId);

            if (user == null)
                return NotFound("The user doesn't exist.");

            // Change the user's password through the User Manager.
            var result = await _userManager.ChangePasswordAsync(user, passwordToChangeDto.Password, passwordToChangeDto.NewPassword);

            if (result.Succeeded)
                return NoContent();

            return BadRequest("Something happened.");
        }
    }
}
