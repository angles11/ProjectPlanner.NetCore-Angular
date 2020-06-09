using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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

        public async Task ChangeFriendshipStatus(string userId, string senderId, string status)
        {
            var friendship = await _dataContext.Friendships.FindAsync(senderId, userId);

            switch (status)
            {
                case "Accepted":
                    friendship.Status = Status.Accepted;
                     _dataContext.Update(friendship);
                    break;
                case "Blocked":
                    friendship.Status = Status.Blocked;
                    _dataContext.Update(friendship);
                    break;
                case "Declined":
                     _dataContext.Friendships.Remove(friendship);
                    break;
            }

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

        public async Task<ICollection<Friendship>> GetFriends(string userId)
        {
            var friends = await _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
                .Where(f => f.SenderId == userId).Where(f => f.Status == Status.Accepted).ToListAsync();

            friends.AddRange(_dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
                .Where(f => f.RecipientId == userId).Where(f => f.Status == Status.Accepted));

            return friends;
        }

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

        public async Task<ICollection<User>> GetUsers(string userId)
        {
            return await _dataContext.Users.Where(u => u.Id != userId).OrderBy(u => u.KnownAs).ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
