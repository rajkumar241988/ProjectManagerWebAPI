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

namespace ProjectManager.BL
{
    public class ParentTaskServices : IParentTaskServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParentTaskServices()
        {
            _unitOfWork = new UnitOfWork();
        }
        public int CreateParentTask(ParentTaskEntity taskEntity)
        {
            using (var scope = new TransactionScope())
            {
                var task = new ParentTask
                {
                    Parent_ID = taskEntity.Parent_ID,
                    Parent_Task = taskEntity.Parent_Task
                };
                _unitOfWork.ParentTaskRepository.Insert(task);
                _unitOfWork.Save();
                scope.Complete();
                return task.Parent_ID;
            }
        }

        public bool DeleteParentTask(int parentTaskId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ParentTaskEntity> GetAllParentTasks()
        {
            var tasks = _unitOfWork.ParentTaskRepository.GetAll().ToList();
            if (tasks != null)
            {
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<ParentTask, ParentTaskEntity>();
                });
                IMapper mapper = config.CreateMapper();
                var taskModel = mapper.Map<List<ParentTask>, List<ParentTaskEntity>>(tasks);
                return taskModel;
            }
            return null;
        }

        public ParentTaskEntity GetParentTaskById(int taskId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateParentTask(int parentTaskId, ParentTaskEntity taskEntity)
        {
            throw new NotImplementedException();
        }
    }
}
