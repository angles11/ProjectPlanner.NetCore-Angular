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
        public Task<ICollection<User>> GetUsers(string userId);
        public Task<ICollection<Friendship>> GetFriends(string userId);
        public Task AddFriend(Friendship friendship);
        public Task<bool> AlreadyFriends(string userId1, string userId2);
        public Task<Friendship> GetFriendship(string userId1, string userId2);
        public Task ChangeFriendshipStatus(string userId, string senderId, string status);
    }
}
