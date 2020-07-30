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

        // This method will be call only if the database is empty
        // to provide the initial data.
        public static void SeedUsers(DataContext dataContext, UserManager<User> userManager)
        {
            if (!dataContext.Users.Any())
            {
                // Get the data file in Json format.
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");


                // Convert the Json data into .NET objects.
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                
                // Create an entity of a user for each object provided in the data file.
                foreach (var user in users)
                {
                    userManager.CreateAsync(user, "asdasdas");
                }

                dataContext.SaveChanges();
            }
        }

    }

}
