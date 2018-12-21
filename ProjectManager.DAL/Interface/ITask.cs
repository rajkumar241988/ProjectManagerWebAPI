using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.DAL.Interface
{
    public interface ITask
    {
        Task GetTaskByProjectID(int iProjectID);
        Task GetParentTaskByTaskID(int iTaskID);
        List<Task> AddTask(Task objTask);
        List<Task> UpdateTask(Task objTask);
        List<Task> DeleteTask(Task objTask);
    }
}
