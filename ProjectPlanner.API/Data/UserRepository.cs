using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ProjectPlanner.API.Helpers;
using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;

        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public  async Task AcceptFriendship(string userId, string senderId)
        {
            var friendship = await _dataContext.Friendships.FindAsync(senderId, userId);

            friendship.Status = Status.Accepted;

             _dataContext.Friendships.Update(friendship);
        }

        public async Task AddFriend(Friendship friendship)
        {

            await _dataContext.Friendships.AddAsync(friendship);
          
        }

        public async Task<bool> AlreadyFriends(string userId1, string userId2)
        {
            if (await _dataContext.Friendships.FindAsync(userId1, userId2) != null)
                return true;
            if (await _dataContext.Friendships.FindAsync(userId2, userId1) != null)
                return true;
            return false;

        }

        public async Task<ICollection<Friendship>> GetFriendships(string userId)
        {
            var friendships = await _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient).Where(f => f.SenderId == userId || f.RecipientId == userId).ToListAsync();

            return friendships;
        }

        //public  Task<ICollection<Friend>> GetFriends(string userId)
        //{
        //    var friends = new List<Friend>();

        //    //var acceptedFriends = await _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
        //    //    .Where(f => f.SenderId == userId && f.Status == Status.Accepted).Select(f => f.Recipient).ToListAsync();
        //    //acceptedFriends.AddRange(_dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
        //    //    .Where(f => f.RecipientId == userId && f.Status == Status.Accepted).Select(f => f.Sender));

        //    //if(acceptedFriends.Any())
        //    //{
        //    //    foreach (var friend in acceptedFriends)
        //    //    {
        //    //        var friendToAdd = new Friend()
        //    //        {
        //    //            FriendId = friend.Id,
        //    //            KnownAs = friend.KnownAs,
        //    //            LastActive = friend.LastActive,
        //    //            PhotoUrl = friend.PhotoUrl,
        //    //            Status = Status.Accepted,
        //    //            Position = friend.Position,
        //    //            Employer = friend.Employer,
        //    //            Experience = friend.Experience,
        //    //            Country = friend.Country
        //    //            Since  = 
        //    //        };
        //    //        friends.Add(friendToAdd);
        //    //    }
        //    //}

        //    //var pendingFriends = await _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
        //    //    .Where(f => f.RecipientId == userId && f.Status == Status.Pending).Select(f => f.Sender).ToListAsync();

        //    //if (pendingFriends.Any())
        //    //{
        //    //    foreach (var friend in pendingFriends)
        //    //    {
        //    //        var friendToAdd = new Friend()
        //    //        {
        //    //            FriendId = friend.Id,
        //    //            KnownAs = friend.KnownAs,
        //    //            LastActive = friend.LastActive,
        //    //            PhotoUrl = friend.PhotoUrl,
        //    //            Status = Status.Pending,
        //    //            Position = friend.Position,
        //    //            Employer = friend.Employer,
        //    //            Experience = friend.Experience,
        //    //            Country = friend.Country
        //    //        };
        //    //        friends.Add(friendToAdd);
        //    //    }
        //    //}

        //    //var blockedFriends = await _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
        //    //     .Where(f => f.SenderId == userId && f.Status == Status.Blocked && f.ActionUserId == userId).Select(f => f.Recipient).ToListAsync();
        //    //blockedFriends.AddRange( _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
        //    //    .Where(f => f.RecipientId == userId && f.Status == Status.Blocked && f.ActionUserId == userId).Select(f => f.Sender));

        //    //if (blockedFriends.Any())
        //    //{
        //    //    foreach (var friend in blockedFriends)
        //    //    {
        //    //        var friendToAdd = new Friend()
        //    //        {
        //    //            FriendId = friend.Id,
        //    //            KnownAs = friend.KnownAs,
        //    //            LastActive = friend.LastActive,
        //    //            PhotoUrl = friend.PhotoUrl,
        //    //            Status = Status.Blocked,
        //    //            Position = friend.Position,
        //    //            Employer = friend.Employer,
        //    //            Experience = friend.Experience,
        //    //            Country = friend.Country
        //    //        };
        //    //        friends.Add(friendToAdd);
        //    //    }
        //    //}
        //    return friends.OrderBy(f => f.KnownAs).ToList();
        //}

        public async Task<Friendship> GetFriendship(string userId1, string userId2)
        {
            var friendship = await _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
                .FirstOrDefaultAsync(f => f.SenderId == userId1 && f.RecipientId == userId2);

            if (friendship != null)
                return friendship;

            return await _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
                .FirstOrDefaultAsync(f => f.RecipientId == userId1 && f.SenderId == userId2);
        }

        public async Task<User> GetUser(string userId)
        {
            return await _dataContext.Users.Include(u => u.OwnedProjects).Include(u => u.CollaboratedProjects)
                .Include(u => u.FriendshipSent).Include(u => u.FriendshipReceived)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<PagedList<User>> GetUsers(string userId, UserParams userParams)
        {
            var friends = await _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
                .Where(f => f.SenderId == userId).Select(f => f.Recipient).ToListAsync();
            friends.AddRange(_dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient).Where(f => f.RecipientId == userId).Select(f => f.Sender));

            // Get all users with no friendship with the current User
            var users =  await _dataContext.Users.ToListAsync();

            var filteredUsers = users.Where(u => friends.All(f => f.Id != u.Id) && u.Id != userId);

            if (!string.IsNullOrEmpty(userParams.Gender) && userParams.Gender != "all")
            {
                filteredUsers = filteredUsers.Where(u => u.Gender == userParams.Gender);
            }

            if (!string.IsNullOrEmpty(userParams.SearchTerm))
            {
               var searchTerm = userParams.SearchTerm.ToUpperInvariant();
                filteredUsers = filteredUsers.Where(u => u.KnownAs.ToUpperInvariant().Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy)){
                switch (userParams.OrderBy)
                {
                    case "nameAscending":
                        filteredUsers = filteredUsers.OrderBy(u => u.KnownAs);
                        break;
                    case "nameDescending":
                        filteredUsers = filteredUsers.OrderByDescending(u => u.KnownAs);
                        break;
                    case "positionAscending":
                        filteredUsers = filteredUsers.OrderBy(u => u.Position);
                        break;
                    case "positionDescending":
                        filteredUsers = filteredUsers.OrderByDescending(u => u.Position);
                        break;
                    case "employerAscending":
                        filteredUsers = filteredUsers.OrderBy(u => u.Employer);
                        break;
                    case "employerDescending":
                        filteredUsers = filteredUsers.OrderByDescending(u => u.Employer);
                        break;
                    case "countryAscending":
                        filteredUsers = filteredUsers.OrderBy(u => u.Country);
                        break;
                    case "countryDescending":
                        filteredUsers = filteredUsers.OrderByDescending(u => u.Country);
                        break;                        
                }
            }

            return   PagedList<User>.Create(filteredUsers.ToList(), userParams.PageIndex, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public void DeleteFriendship(Friendship friendship)
        {
            _dataContext.Friendships.Remove(friendship);
        }
    }
}
