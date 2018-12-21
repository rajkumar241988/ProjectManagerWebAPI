using AutoMapper;
using Moq;
using NUnit.Framework;
using ProjectManager.BL;
using ProjectManager.BusinessEntities;
using ProjectManager.DAL;
using ProjectManager.DAL.GenericRepository;
using ProjectManager.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using TestHelper;
using Task = ProjectManager.DAL.Task;

namespace BusinessLayer.Tests
{
    public class TaskServicesTests
    {
        #region Variables  
        private ITaskServices _taskService;
        private IUnitOfWork _unitOfWork;
        private List<Task> _task;
        private GenericRepository<Task> _taskRepository;
        private ProjectManagerEntities _dbEntities;
        #endregion

        #region Setup  
        ///<summary>  
        /// Re-initializes test.  
        ///</summary>  
        [SetUp]
        public void ReInitializeTest()
        {
            _dbEntities = new Mock<ProjectManagerEntities>().Object;
            _taskRepository = SetUpTaskRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.TaskRepository).Returns(_taskRepository);
            _unitOfWork = unitOfWork.Object;
            _taskService = new TaskServices();
        }

        #endregion

        private GenericRepository<Task> SetUpTaskRepository()
        {

            // Initialise repository  
            var mockRepo = new Mock<GenericRepository<Task>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior  
            mockRepo.Setup(p => p.GetAll()).Returns(_task);

            mockRepo.Setup(p => p.GetByID(It.IsAny<int>()))
                .Returns(new Func<int, Task>(
                    id => _task.Find(p => p.Task_ID.Equals(id))));


            mockRepo.Setup(p => p.Insert((It.IsAny<Task>())))
                .Callback(new Action<Task>(newTask =>
                {
                    _task.Add(newTask);
                }));


            mockRepo.Setup(p => p.Update(It.IsAny<Task>()))
                .Callback(new Action<Task>(tsk =>
                {
                    var oldTask = _task.Find(a => a.Task_ID == tsk.Task_ID);
                    oldTask = tsk;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Task>()))
                .Callback(new Action<Task>(tsk =>
                {
                    var taskToRemove =
                        _task.Find(a => a.Task_ID == tsk.Task_ID);

                    if (taskToRemove != null)
                        _task.Remove(taskToRemove);
                }));

            // Return mock implementation object  
            return mockRepo.Object;
        }
        [TearDown]
        public void DisposeTest()
        {
            _taskService = null;
            _unitOfWork = null;
            _taskRepository = null;
            if (_dbEntities != null)
                _dbEntities.Dispose();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _task = SetUpTasks();
        }

        private static List<Task> SetUpTasks()
        {
            var tasks = DataInitializer.GetAllTasks();
            return tasks;
        }
        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _task = null;
        }

        #region Task Test Methods

        [Test]
        public void GetAllTaskTest()
        {
            var tasks = _taskService.GetAllTasks();
            var taskList =
                tasks.Select(
                    taskEntity =>
                    new Task
                    {
                        Task_ID = taskEntity.Task_ID,
                        Parent_ID = taskEntity.Parent_ID,
                        Project_ID = taskEntity.Project_ID,
                        Task1 = taskEntity.Task1,
                        Start_Date = taskEntity.Start_Date,
                        End_Date = taskEntity.End_Date,
                        Priority = taskEntity.Priority,
                        Status = taskEntity.Status
                    }).ToList();
            var comparer = new TaskComparer();
            CollectionAssert.AreEqual(
                taskList.OrderBy(task => task, comparer),
                _task.OrderBy(task => task, comparer), comparer);
        }

        ///<summary>  
        /// Service should return null  
        ///</summary>  
        [Test]
        public void GetAllTasksTestForNull()
        {
            _task.Clear();
            var tasks = _taskService.GetAllTasks();
            Assert.IsEmpty(tasks);
            SetUpTasks();
        }

        ///<summary>  
        /// Service should return task if correct id is supplied  
        ///</summary>  
        [Test]
        public void GetTaskByRightIdTest()
        {
            var task = _taskService.GetTaskById(1);

            if (task != null)
            {
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<TaskEntity, Task>();
                });
                IMapper mapper = config.CreateMapper();
                var taskmodel = mapper.Map<TaskEntity, Task>(task);

                AssertObjects.PropertyValuesAreEquals(taskmodel,
                    _task.Find(a => a.Task1.Contains("DAL Layer -1")));
            }
        }
        ///<summary>  
        /// Service should return null  
        ///</summary>  
        [Test]
        public void GetTaskByWrongIdTest()
        {
            var task = _taskService.GetTaskById(0);
            Assert.Null(task);
        }

        ///<summary>  
        /// Add new task test  
        ///</summary>  
        [Test]
        public void AddNewTaskTest()
        {
            var newTask = new TaskEntity()
            {
                Task_ID = 2,
                Parent_ID = 2,
                Project_ID = 2,
                Task1 = "DAL Layer -2",
                Start_Date = Convert.ToDateTime("2018-12-28"),
                End_Date = Convert.ToDateTime("2018-12-31"),
                Priority = "20",
                Status = null
            };

            var maxTaskIDBeforeAdd = _task.Max(a => a.Task_ID);
            newTask.Task_ID = maxTaskIDBeforeAdd + 1;

            _taskService.CreateTask(newTask);
            var addedTask = new Task()
            {
                Task_ID = newTask.Task_ID,
                Parent_ID = newTask.Parent_ID,
                Project_ID = newTask.Project_ID,
                Task1 = newTask.Task1,
                Start_Date = newTask.Start_Date,
                End_Date = newTask.End_Date,
                Priority = newTask.Priority,
                Status = newTask.Status
            };
            AssertObjects.PropertyValuesAreEquals(addedTask, _task.Last());
            Assert.That(maxTaskIDBeforeAdd + 1, Is.EqualTo(newTask.Task_ID));
        }

        ///<summary>  
        /// Update task test  
        ///</summary>  
        [Test]
        public void UpdateTaskTest()
        {
            var firstTask = _task.First();
            firstTask.Task1 = "DAL Layer -Updated";
            var updatedTask = new TaskEntity()
            {
                Task_ID = firstTask.Task_ID,
                Parent_ID = firstTask.Parent_ID,
                Project_ID = firstTask.Project_ID,
                Task1 = firstTask.Task1,
                Start_Date = firstTask.Start_Date,
                End_Date = firstTask.End_Date,
                Priority = firstTask.Priority,
                Status = firstTask.Status
            };
            _taskService.UpdateTask(firstTask.Task_ID, updatedTask);
            Assert.That(firstTask.Task_ID, Is.EqualTo(2)); // hasn't changed  
            Assert.That(firstTask.Task1, Is.EqualTo("DAL Layer -Updated")); // Task name changed  

        }

        ///<summary>  
        /// Delete Task test  
        ///</summary>  
        [Test]
        public void DeleteTaskTest()
        {
            int maxID = _task.Max(a => a.Task_ID); // Before removal  
            var lastTask = _task.Last();

            // Remove last Task  
            _taskService.DeleteTask(lastTask.Task_ID);
            var task = _taskService.GetTaskById(maxID - 1);
            if(task != null)
                Assert.That(maxID, Is.GreaterThan(task.Task_ID)); // Max id reduced by 1  
        }
        #endregion
    }
}
