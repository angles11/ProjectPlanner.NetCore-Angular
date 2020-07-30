using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Helpers
{
    public static class Extensions
    {

        // Adds the Error Headers to the response.
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

       /// <summary>
       ///  Adds the pagination headers to the response.
       /// </summary>
       /// <param name="response"> Response to be extended. </param>
       /// <param name="pageIndex"> Index of the current page. </param>
       /// <param name="pageSize"> Number of elements for each page. </param>
       /// <param name="totalItems"> Total number of elements. </param>
       /// <param name="totalPages"> Total number of pages. </param>
        public static void AddPagination(this HttpResponse response, int pageIndex, int pageSize, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(pageIndex, pageSize, totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

        /// <summary>
        /// Calculates the percentage of "Completed" Todos in a project.
        /// </summary>
        /// <param name="todos"> Collection of Todos</param>
        /// <returns> 
        /// An integer with the percentage.
        /// </returns>
        public static  int CalculatePercentage(this ICollection<Todo> todos)
        {
            // Get all the "Completed" todos.
            var completedTodosCount =  todos.Where(t => t.Status == "Completed").ToList().Count();


            if (completedTodosCount == 0)
                return 0;
            // Calculate the percentage.
            var percentage = (completedTodosCount * 100) / todos.Count();

            return percentage;
        }


        /// <summary>
        ///  Get the most recently TodoMessage added in a Project.
        /// </summary>
        /// <param name="todos"> Collection of Todos from a project. </param>
        /// <returns>
        ///  A message entity.
        /// </returns>
        public static TodoMessage GetLastMessage(this ICollection<Todo> todos)
        {
            // Iterate from each todo within a project,
            // and select the messages within the todo.
            var messages = from todo in todos
                           from m in todo.Messages
                           select m;

            // Check that the messages Collection isn't empty.
            if (messages.Any())
            {
                // Order the list from the most recent created to the least recent.
                // Return the most recent.
                var message = messages.OrderByDescending(m => m.Created).First();
                return message;
            }

            // Return null if there are no messages.
            return null;
        }
    }
}
