
using ProjectPlanner.API.Helpers;
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

        public Task<ICollection<Project>> GetProjects(string userId, ProjectParams projectParams);
        public Task<bool> SaveAll();

        public void EditProject(Project project);
        public void DeleteProject(Project project);

        public void DeleteCollaboration(Collaboration collaboration);

        public void AddCollaboration(Collaboration collaboration);

        public Task<Collaboration> GetCollaboration(int projectId, string collaboratorId);

    }
}
