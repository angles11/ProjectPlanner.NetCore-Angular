using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectPlanner.API.Dtos;
using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Data
{
    public class SeedData
    {

        public static void SeedUsers(DataContext dataContext, UserManager<User> userManager)
        {
            if (!dataContext.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");

                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                

                foreach (var user in users)
                {
                    userManager.CreateAsync(user, "asdasdas");
                }

                dataContext.SaveChanges();
            }
        }

        //private static void CreatePasswordHash(string password, out byte[] passwordHash)
        //{
        //    using (var hmac = new HMACSHA512())
        //    {
        //        passwordHash
        //    }
        //}
    }

}
