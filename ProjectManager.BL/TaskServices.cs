using AutoMapper;
using ProjectManager.BusinessEntities;
using ProjectManager.DAL;
using ProjectManager.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Task = ProjectManager.DAL.Task;

namespace ProjectManager.BL
{
    public class TaskServices : ITaskServices
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>  
        /// Public constructor.  
        /// </summary>  
        public TaskServices()
        {
            _unitOfWork = new UnitOfWork();
        }
        /// <summary>  
        /// Creates a task  
        /// </summary>  
        /// <param name="TaskEntity"></param>  
        /// <returns></returns>
        public int CreateTask(TaskEntity taskEntity)
        {
            using (var scope = new TransactionScope())
            {
                var task = new Task
                {
                    Parent_ID = taskEntity.Parent_ID,
                    Project_ID = taskEntity.Project_ID,
                    Task1 = taskEntity.Task1,
                    Start_Date = taskEntity.Start_Date,
                    End_Date = taskEntity.End_Date,
                    Priority = taskEntity.Priority,
                    Status = taskEntity.Status
                };
                _unitOfWork.TaskRepository.Insert(task);
                _unitOfWork.Save();
                scope.Complete();
                return task.Task_ID;
            }
        }
        /// <summary>  
        /// Deletes a particular task  
        /// </summary>  
        /// <param name="projectId"></param>  
        /// <returns></returns>
        public bool DeleteTask(int taskId)
        {
            var success = false;
            if (taskId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var task = _unitOfWork.TaskRepository.GetByID(taskId);
                    if (task != null)
                    {
                        _unitOfWork.TaskRepository.Delete(task);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        public IEnumerable<vw_TaskSearchEntity> GetTaskSearch()
        {
            var tasks = _unitOfWork.TaskSearchRepository.GetAll().ToList();
            if (tasks.Any())
            {
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<vw_TaskSearch, vw_TaskSearchEntity>();
                });
                IMapper mapper = config.CreateMapper();
                var tasksModel = mapper.Map<List<vw_TaskSearch>, List<vw_TaskSearchEntity>>(tasks);
                return tasksModel;
            }
            return null;
        }
        /// <summary>  
        /// Fetches all the tasks.  
        /// </summary>  
        /// <returns></returns>
        public IEnumerable<TaskEntity> GetAllTasks()
        {
            var tasks = _unitOfWork.TaskRepository.GetAll().ToList();
            if (tasks != null)
            {
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<Task, TaskEntity>();
                });
                IMapper mapper = config.CreateMapper();
                var taskModel = mapper.Map<List<Task>, List<TaskEntity>>(tasks);
                return taskModel;
            }
            return null;
        }
        /// <summary>  
        /// Fetches task details by id  
        /// </summary>  
        /// <param name="taskId"></param>  
        /// <returns></returns>
        public TaskEntity GetTaskById(int taskId)
        {
            var task = _unitOfWork.TaskRepository.GetByID(taskId);
            if (task != null)
            {
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<Task, TaskEntity>();
                });
                IMapper mapper = config.CreateMapper();
                var taskModel = mapper.Map<Task, TaskEntity>(task);
                return taskModel;
            }
            return null;
        }
        /// <summary>  
        /// Updates a task  
        /// </summary>  
        /// <param name="userId"></param>  
        /// <param name="userEntity"></param>  
        /// <returns></returns>
        public bool UpdateTask(int taskId, TaskEntity taskEntity)
        {
            var success = false;
            if (taskEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var task = _unitOfWork.TaskRepository.GetByID(taskId);
                    if (task != null)
                    {
                        task.Parent_ID = taskEntity.Parent_ID;
                        task.Project_ID = taskEntity.Project_ID;
                        task.Task1 = taskEntity.Task1;
                        task.Start_Date = taskEntity.Start_Date;
                        task.End_Date = taskEntity.End_Date;
                        task.Priority = taskEntity.Priority;
                        task.Status = taskEntity.Status;

                        _unitOfWork.TaskRepository.Update(task);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        public void UpdateExistingUsersTask(int id)
        {
            _unitOfWork.UpdateExistingUSersTask(id);
        }
    }
    
}
