using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;
using ProjectPlanner.API.Data;
using ProjectPlanner.API.Dtos;
using ProjectPlanner.API.Helpers;
using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;



        public UserController(UserManager<User> userManager, IMapper mapper, IUserRepository userRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string userId)
        {


            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var userFromRepo = await _userRepository.GetUser(userId);

            if (userFromRepo.Id != user.Id)
            {
                return Unauthorized("Error");
            }

            var userToReturn = _mapper.Map<UserForReturnDto>(user);

            return Ok(userToReturn);
        }

        [HttpGet("{userId}/friends")]
        public async Task<IActionResult> GetFriends(string userId)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            var friends = await _userRepository.GetFriends(userId);

            var friendsToReturn = _mapper.Map<IEnumerable<FriendToReturnDto>>(friends);

            return Ok(friendsToReturn);
        }

        [HttpGet("{userId}/users")]
        public async Task<IActionResult> GetUsers(string userId, [FromQuery]UserParams userParams)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            var users = await _userRepository.GetUsers(userId, userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{userId}/friends/{recipientId}")]
        public async Task<IActionResult> GetFriendship(string userId, string recipientId)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();
            
            if(await _userRepository.AlreadyFriends(userId, recipientId))
            {
                var friendship = await _userRepository.GetFriendship(userId, recipientId);

                var friendshipToReturn = _mapper.Map<FriendshipForReturnDto>(friendship);

                return Ok(friendshipToReturn);
            }

            return BadRequest();

        }

        [HttpPost("{userId}/friends/{recipientId}")]
        public async Task<IActionResult> AddFriend(string userId, string recipientId)
        {
            var user = await _userRepository.GetUser(userId);

            var recipient = await _userRepository.GetUser(recipientId);

            if (user == null | recipient == null)
                return NotFound();

            if (user.Id != _userManager.GetUserId(User))
                return Unauthorized();

            var friendship = new Friendship()
            {
                Sender = user,
                SenderId = userId,
                Recipient = recipient,
                RecipientId = recipientId,
                ActionUserId = userId,
            };

            if (await _userRepository.AlreadyFriends(userId, recipientId))
                return BadRequest("You are already friend with " + recipient.KnownAs);

            await _userRepository.AddFriend(friendship);

            if (await _userRepository.SaveAll())
                return Ok();

            return NoContent();
        }

        [HttpDelete("{userId}/friends/{userId2}")]
        public async Task<IActionResult> DeleteFriendship(string userId, string userId2)
        {
            var friendship = await _userRepository.GetFriendship(userId, userId2);

            if (friendship == null)
            {
                return NotFound();
            }

             _userRepository.DeleteFriendship(friendship);

            if (await _userRepository.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Couldn't remove the friend request. Try again"); 
          
        }

        [HttpPut("{userId}/friends/{senderId}")]

        public async Task<IActionResult> ChangeFriendhipStatus(string userId, string senderId, [FromQuery]string status)
        {
            if (userId != _userManager.GetUserId(User))
                return Unauthorized();

            if (!await _userRepository.AlreadyFriends(userId, senderId))
                return BadRequest("There is no friend request from " + senderId);

            await _userRepository.ChangeFriendshipStatus(userId, senderId, status);

            if (await _userRepository.SaveAll())
                return NoContent();

            return BadRequest("Something happened, try again.");
        }
    }
}
