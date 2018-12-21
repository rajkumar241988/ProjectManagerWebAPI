using ProjectManager.BusinessEntities;
using System.Collections.Generic;

namespace ProjectManager.BL
{
    public interface IParentTaskServices
    {
        ParentTaskEntity GetParentTaskById(int taskId);
        IEnumerable<ParentTaskEntity> GetAllParentTasks();
        int CreateParentTask(ParentTaskEntity taskEntity);
        bool UpdateParentTask(int parentTaskId, ParentTaskEntity taskEntity);
        bool DeleteParentTask(int parentTaskId);
    }
}
