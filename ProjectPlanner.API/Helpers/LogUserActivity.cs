using Microsoft.AspNetCore.Mvc.Filters;
using ProjectPlanner.API.Data;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectPlanner.API.Helpers
{
    public class LogUserActivity: IAsyncActionFilter
    {
        // Action filter to update the Last Active property of a user
        // making a call to a controller that use an instance of this extension.
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get the context after the call was made.
            var resultContext = await next();

            // Get the id of the user making the call.
            var userId = resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Get the repository to update the property.
            var repository = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();

            // Get the current user.
            var user = await repository.GetUser(userId);

            user.LastActive = DateTime.Now;

            await repository.SaveAll();
        }
    }
}
