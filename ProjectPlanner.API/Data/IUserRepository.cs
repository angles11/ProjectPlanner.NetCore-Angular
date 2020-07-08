using ProjectPlanner.API.Helpers;
using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Data
{
    public interface IUserRepository
    {
        public Task<bool> SaveAll();
        public Task<User> GetUser(string userId);
        public Task<PagedList<User>> GetUsers(string userId, UserParams userParams);
        //public Task<ICollection<Friend>> GetFriends(string userId);
        public Task AddFriend(Friendship friendship);
        public void DeleteFriendship(Friendship friendship);
        public Task<bool> AlreadyFriends(string userId1, string userId2);
        public Task<Friendship> GetFriendship(string userId1, string userId2);
        public Task<ICollection<Friendship>> GetFriendships(string userId);
        public Task AcceptFriendship(string userId, string senderId);
    }
}
