using Microsoft.EntityFrameworkCore;
using ProjectPlanner.API.Helpers;
using ProjectPlanner.API.Models;
using System.Collections.Generic;
using System.Linq;
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

        // Change the status of the friendship entity between two users.
        public  async Task AcceptFriendship(string userId, string senderId)
        {
            // Get the friendship.
            var friendship = await _dataContext.Friendships.FindAsync(senderId, userId); //Composite keys

            friendship.Status = Status.Accepted;

             _dataContext.Friendships.Update(friendship);
        }

        public async Task AddFriend(Friendship friendship)
        {

            await _dataContext.Friendships.AddAsync(friendship);
          
        }

       /// <summary>
       ///  Checks if there is an existing friendship entity between two users
       ///  using the users' id. 
       /// </summary>
       /// 
       /// <returns> 
       /// True if there is a friendship entity nad false if there isn't.
       /// </returns>
        public async Task<bool> AlreadyFriends(string userId1, string userId2)
        {
            if (await _dataContext.Friendships.FindAsync(userId1, userId2) != null)
                return true;
            if (await _dataContext.Friendships.FindAsync(userId2, userId1) != null) 
                return true;
            return false;

        }

        /// <summary>
        /// Get all the friendship entities for a user.
        /// </summary>
        /// <param name="userId"> Id of the user. </param>
        /// <returns>
        /// A Collection of friendships. 
        /// </returns>
        public async Task<ICollection<Friendship>> GetFriendships(string userId)
        {
            var friendships = await _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient).Where(f => f.SenderId == userId || f.RecipientId == userId).ToListAsync();

            return friendships;
        }

        /// <summary>
        /// Get the friendship between two users.
        /// </summary>
        /// <param name="userId1"> Id of the first user. </param>
        /// <param name="userId2"> Id of the second user. </param>
        /// <returns> 
        /// A friendship entity or null if there is no friendship.
        /// </returns>
        public async Task<Friendship> GetFriendship(string userId1, string userId2)
        {
            var friendship = await _dataContext.Friendships.FindAsync(userId1, userId2);

            if (friendship != null)
                return friendship;

            return await _dataContext.Friendships.FindAsync(userId1, userId2);
        }


        /// <summary>
        ///  Get the user whose id matches the provided user id. 
        /// </summary>
        /// <param name="userId"> Id of the user to get</param>
        /// <returns>
        /// A user entity or null if there is no match.
        /// </returns>
        public async Task<User> GetUser(string userId)
        {
            return await _dataContext.Users.Include(u => u.OwnedProjects).Include(u => u.CollaboratedProjects)
                .Include(u => u.FriendshipSent).Include(u => u.FriendshipReceived)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }


        /// <summary>
        ///  Gets the list of users with NO friendship relation with the requesting user
        ///  then applies filters/sorting preferences.
        /// </summary>
        /// <param name="userId"> Id of the requesting user. </param>
        /// <param name="userParams"> Query parameters to filter or sort the list of users. </param>
        /// <returns>
        /// A Paged List of users with the pagination headers. 
        /// </returns>
        public async Task<PagedList<User>> GetUsers(string userId, UserParams userParams)
        {
            // Get all the friendships for the user and then for each friendship, select the user's "friend" entity.

            var friends = await _dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient)
                .Where(f => f.SenderId == userId).Select(f => f.Recipient).ToListAsync();
            friends.AddRange(_dataContext.Friendships.Include(f => f.Sender).Include(f => f.Recipient).Where(f => f.RecipientId == userId).Select(f => f.Sender));

            // Get all users from the database..
            var users =  await _dataContext.Users.ToListAsync();

            // Filter the "friends" users and the current user from the list containing all the users in the database.
            // The final list will contain only the users with no friendship entity with the current user.
            var filteredUsers = users.Where(u => friends.All(f => f.Id != u.Id) && u.Id != userId);

            // Filter the users by the gender query string.
            if (!string.IsNullOrEmpty(userParams.Gender) && userParams.Gender != "all")
            {
                filteredUsers = filteredUsers.Where(u => u.Gender == userParams.Gender);
            }

            // Filter the users whose knownAs doesn't contain the search string.
            if (!string.IsNullOrEmpty(userParams.SearchTerm))
            {
               var searchTerm = userParams.SearchTerm.ToUpperInvariant();
                filteredUsers = filteredUsers.Where(u => u.KnownAs.ToUpperInvariant().Contains(searchTerm));
            }
            // Order the list of users by the orderby query string.
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

        // If the async save on the database is successful will return true,
        // otherwise will return false.
        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        // Removes the friendship entity from the database.
        public void DeleteFriendship(Friendship friendship)
        {
            _dataContext.Friendships.Remove(friendship);
        }

        public void EditUser(User user)
        {
            _dataContext.Update(user);
        }
    }
}
