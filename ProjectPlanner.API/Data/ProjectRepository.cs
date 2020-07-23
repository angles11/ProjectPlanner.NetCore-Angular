
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

        public async Task<Collaboration> GetCollaboration(int projectId, string collaboratorId)
        {
            return  await _dataContext.Collaborations.FindAsync(collaboratorId, projectId);
        }

        public async Task<Project> GetProject(int projectId)
        {
            return await _dataContext.Projects.Include(p => p.Owner).Include(p => p.Todos).ThenInclude(t => t.Messages).ThenInclude(m => m.User)
                .Include(p => p.Collaborations).ThenInclude(c => c.User).FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public async Task<Todo> GetTodo(int todoId)
        {
            return await _dataContext.Todos.Include(t => t.Project).Include(t => t.Messages).ThenInclude(m => m.User).FirstOrDefaultAsync(t => t.Id == todoId);
        }

        public async Task<ICollection<Project>> GetProjects(string userId, ProjectParams projectParams)
        {
            var ownedProjects = await _dataContext.Projects.Include(p => p.Owner)
                                                           .Include(p => p.Collaborations)
                                                                .ThenInclude(c => c.User)
                                                            .Where(p => p.OwnerId == userId)
                                                            .ToListAsync();

            var collaboratedProjects = await _dataContext.Collaborations.Include(c => c.Project)
                                                                            .ThenInclude(p => p.Owner)
                                                                        .Include(c => c.Project)
                                                                            .ThenInclude(p => p.Collaborations)
                                                                                .ThenInclude(c => c.User)
                                                                        .Where(c => c.UserId == userId)
                                                                        .Select(c => c.Project)
                                                                        .ToListAsync();

            var projects = ownedProjects.Concat(collaboratedProjects);

            if (!string.IsNullOrEmpty(projectParams.SearchTerm))
            {
                var searchTerm = projectParams.SearchTerm.ToUpperInvariant();
                projects = projects.Where(p => p.Title.ToUpperInvariant().Contains(searchTerm));
            }

            return projects.ToList();

        }

       

        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<TodoMessage> GetMessage(int messageId)
        {
            return await _dataContext.TodoMessages.FirstOrDefaultAsync(m => m.Id == messageId);
        }
    }
}
