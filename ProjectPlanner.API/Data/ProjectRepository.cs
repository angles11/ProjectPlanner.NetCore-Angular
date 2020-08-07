
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.API.Helpers;
using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Data
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DataContext _dataContext;

        public ProjectRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public void Add<T>(T entity) where T : class
        {
            _dataContext.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {   
            _dataContext.Remove(entity);
        }

        public void Edit<T>(T entity) where T : class
        {
            _dataContext.Update(entity);
        }

        // Returns the collaboration with the composite key from 
        // the id of the project and the id of the user 
        // if there is no match, returns null.
        public async Task<Collaboration> GetCollaboration(int projectId, string collaboratorId)
        {
            return await _dataContext.Collaborations.FindAsync(collaboratorId, projectId);
        }


        // Returns the project with id equals to the provided project's id 
        // if there is no match, returns null.
        public async Task<Project> GetProject(int projectId)
        {

            return await _dataContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        }

        //Returns the Todo with id equals to the provided todo's id
        // if there is no match, returns null.
        public async Task<Todo> GetTodo(int todoId)
        {
            return await _dataContext.Todos.FirstOrDefaultAsync(t => t.Id == todoId);
        }


        /// <summary>
        ///  Gets the list of projects where a user is the owner or one of the collaborators,
        ///  then applies the filter parameters.
        /// </summary>
        /// 
        /// <param name="userId"> Id of the user</param>
        /// <param name="projectParams"> Parameters for querying</param>
        /// 
        /// <returns>A Collection of projects</returns>
        public async Task<ICollection<Project>> GetProjects(string userId, ProjectParams projectParams)
        {
            // Get all the projects that the user owns.
            var ownedProjects = await _dataContext.Projects.Where(p => p.OwnerId == userId).ToListAsync();

            // Get all the projects where the user is a collaborator.
            var collaboratedProjects = await _dataContext.Collaborations.Where(c => c.UserId == userId).Select(c => c.Project).ToListAsync();

            // Concat the two lists.
            var projects = ownedProjects.Concat(collaboratedProjects);

            // Filter the projects whose titles doesn't contain the searching string.
            if (!string.IsNullOrEmpty(projectParams.SearchTerm))
            {
                var searchTerm = projectParams.SearchTerm.ToUpperInvariant();
                projects = projects.Where(p => p.Title.ToUpperInvariant().Contains(searchTerm));
            }

            return projects.ToList();

        }


        // If the async save on the database is successful will return true,
        // otherwise will return false.
        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<TodoMessage> GetMessage(int messageId)
        {
            return await _dataContext.TodoMessages.FirstOrDefaultAsync(m => m.Id == messageId);
        }

        // Update the property Modified of a project
        // and then, save it.
        public void UpdateLastModified(Project project)
        {
            project.Modified = DateTime.Now;

            Edit(project);

        }
    }
}
