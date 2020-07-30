
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

        public void Add<T>(T entity) where T : class;
        public void Edit<T>(T entity) where T : class;
        public void Delete<T>(T entity) where T : class;

        public void UpdateLastModified(Project project);
        public Task<Project> GetProject(int projectId);

        public Task<Todo> GetTodo(int todoId);
        public Task<Collaboration> GetCollaboration(int projectId, string collaboratorId);
        public Task<ICollection<Project>> GetProjects(string userId, ProjectParams projectParams);

        public Task<TodoMessage> GetMessage(int messageId);
        public Task<bool> SaveAll();

     

    }
}
