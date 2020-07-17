
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

        public void AddCollaboration(Collaboration collaboration)
        {
            _dataContext.Add(collaboration);
        }

        public void CreateProject(Project project)
        {
            _dataContext.Add(project);
        }

        public void DeleteCollaboration(Collaboration collaboration)
        {
            _dataContext.Remove(collaboration);
        }

        public   void DeleteProject(Project project)
        {
             _dataContext.Remove(project);
        }

        public void EditProject(Project project)
        {
            _dataContext.Update(project);
        }

        public async Task<Collaboration> GetCollaboration(int projectId, string collaboratorId)
        {
            return  await _dataContext.Collaborations.FindAsync(collaboratorId, projectId);
        }

        public Task<Project> GetProject(int projectId)
        {
            return _dataContext.Projects.Include(p => p.Owner).Include(p => p.Collaborations).ThenInclude(c => c.User).FirstOrDefaultAsync(p => p.Id == projectId);
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
    }
}
