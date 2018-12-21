using ProjectManager.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.DAL.Component
{
    public class Task : ITask
    {
        public List<DAL.Task> AddTask(DAL.Task objTask)
        {
            throw new NotImplementedException();
        }

        public List<DAL.Task> DeleteTask(DAL.Task objTask)
        {
            throw new NotImplementedException();
        }

        public DAL.Task GetParentTaskByTaskID(int iTaskID)
        {
            throw new NotImplementedException();
        }

        public DAL.Task GetTaskByProjectID(int iProjectID)
        {
            throw new NotImplementedException();
        }

        public List<DAL.Task> UpdateTask(DAL.Task objTask)
        {
            throw new NotImplementedException();
        }
    }
}
