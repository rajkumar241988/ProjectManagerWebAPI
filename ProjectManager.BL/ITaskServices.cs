using ProjectManager.BusinessEntities;
using System.Collections.Generic;

namespace ProjectManager.BL
{
    public interface ITaskServices
    {
        TaskEntity GetTaskById(int taskId);
        IEnumerable<TaskEntity> GetAllTasks();
        int CreateTask(TaskEntity taskEntity);
        bool UpdateTask(int taskId, TaskEntity taskEntity);
        bool DeleteTask(int taskId);
        IEnumerable<vw_TaskSearchEntity> GetTaskSearch();
        void UpdateExistingUsersTask(int id);
    }
}
