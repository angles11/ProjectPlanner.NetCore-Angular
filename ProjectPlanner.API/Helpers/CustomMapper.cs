using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Helpers
{
    public class CustomMapper
    {
        public  ICollection<Friend> MapFriends (ICollection<Friendship> friendships, string userId)
        {
            var result = new List<Friend>();

            foreach (var friendship in friendships)
            {
                if(friendship.SenderId == userId)
                {
                    var friend = new Friend()
                    {
                        UserFriend = friendship.Recipient,
                        Since = friendship.Since,
                        Status = friendship.Status,
                        SentById = friendship.SenderId
                    };
                    result.Add(friend);
                }
                else if (friendship.RecipientId == userId){
                    var friend = new Friend() { 
                        UserFriend = friendship.Sender,
                        Since = friendship.Since,
                        Status = friendship.Status,
                        SentById = friendship.SenderId
                    };
                    result.Add(friend);
                }
            }

            return result;
        }
    }
}
