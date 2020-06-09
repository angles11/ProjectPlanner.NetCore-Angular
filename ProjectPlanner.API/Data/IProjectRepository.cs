
using ProjectPlanner.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Data
{
    public interface IProjectRepository
    {
        public void CreateProject(Project project);

        public Task<Project> GetProject(int projectId);

        public Task<ICollection<Project>> GetProjects(string userId);
        public Task<bool> SaveAll();

        public void DeleteProject(int projectId);

    }
}
