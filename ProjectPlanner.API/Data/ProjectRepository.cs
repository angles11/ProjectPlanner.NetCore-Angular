
using Microsoft.EntityFrameworkCore;
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
        public void CreateProject(Project project)
        {
            _dataContext.Add(project);
        }

        public  async void DeleteProject(int projectId)
        {
            var project = await _dataContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

            if (project != null)
                 _dataContext.Remove(project);

        }

        public Task<Project> GetProject(int projectId)
        {
            return _dataContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public async Task<ICollection<Project>> GetProjects(string userId)
        {
            var projects = await _dataContext.Projects.Where(p => p.OwnerId == userId).ToListAsync();

            var collaboratedProjects = await _dataContext.Collaborators.Where(c => c.UserId == userId).Select(c => c.Project).ToListAsync();

           foreach(var project in collaboratedProjects)
            {
                projects.Add(project);
            }
            return projects;
        }

        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
