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

namespace BusinessLayer.Tests
{
    public class ParentTaskServicesTests
    {
        #region Variables  
        private IParentTaskServices _parentTaskService;
        private IUnitOfWork _unitOfWork;
        private List<ParentTask> _parentTask;
        private GenericRepository<ParentTask> _parentTaskRepository;
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
            _parentTaskRepository = SetUpParentTaskRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.ParentTaskRepository).Returns(_parentTaskRepository);
            _unitOfWork = unitOfWork.Object;
            _parentTaskService = new ParentTaskServices();
        }

        #endregion

        private GenericRepository<ParentTask> SetUpParentTaskRepository()
        {

            // Initialise repository  
            var mockRepo = new Mock<GenericRepository<ParentTask>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior  
            mockRepo.Setup(p => p.GetAll()).Returns(_parentTask);

            mockRepo.Setup(p => p.GetByID(It.IsAny<int>()))
                .Returns(new Func<int, ParentTask>(
                    id => _parentTask.Find(p => p.Parent_ID.Equals(id))));


            mockRepo.Setup(p => p.Insert((It.IsAny<ParentTask>())))
                .Callback(new Action<ParentTask>(newParent =>
                {
                    _parentTask.Add(newParent);
                }));


            mockRepo.Setup(p => p.Update(It.IsAny<ParentTask>()))
                .Callback(new Action<ParentTask>(prnt =>
                {
                    var oldParent = _parentTask.Find(a => a.Parent_ID == prnt.Parent_ID);
                    oldParent = prnt;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<ParentTask>()))
                .Callback(new Action<ParentTask>(prnt =>
                {
                    var parentTaskToRemove =
                        _parentTask.Find(a => a.Parent_ID == prnt.Parent_ID);

                    if (parentTaskToRemove != null)
                        _parentTask.Remove(parentTaskToRemove);
                }));

            // Return mock implementation object  
            return mockRepo.Object;
        }
        [TearDown]
        public void DisposeTest()
        {
            _parentTaskService = null;
            _unitOfWork = null;
            _parentTaskRepository = null;
            if (_dbEntities != null)
                _dbEntities.Dispose();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _parentTask = SetUpParentTask();
        }

        private static List<ParentTask> SetUpParentTask()
        {
            var parentTasks = DataInitializer.GetAllParentTasks();
            return parentTasks;
        }
        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _parentTask = null;
        }

        #region Parent Task Test Methods

        [Test]
        public void GetAllParentTaskTest()
        {
            var prntTasks = _parentTaskService.GetAllParentTasks();
            var prntTaskList =
                prntTasks.Select(
                    parentTaskEntity =>
                    new ParentTask
                    {
                        Parent_ID = parentTaskEntity.Parent_ID,
                        Parent_Task = parentTaskEntity.Parent_Task
                    }).ToList();
            var comparer = new ParentTaskComparer();
            CollectionAssert.AreEqual(
                prntTaskList.OrderBy(prntTask => prntTask, comparer),
                _parentTask.OrderBy(prntTask => prntTask, comparer), comparer);
        }

        ///<summary>  
        /// Service should return null  
        ///</summary>  
        [Test]
        public void GetAllParentTaskTestForNull()
        {
            _parentTask.Clear();
            var tasks = _parentTaskService.GetAllParentTasks();
            Assert.IsEmpty(tasks);
            SetUpParentTask();
        }

        ///<summary>  
        /// Service should return parent task if correct id is supplied  
        ///</summary>  
        
        [Test]
        public void AddNewParentTaskTest()
        {
            var newTask = new ParentTaskEntity()
            {
                Parent_ID = 3,
                Parent_Task = "Web API Development - 1"
            };

            var maxTaskBeforeAdd = _parentTask.Max(a => a.Parent_ID);
            newTask.Parent_ID = maxTaskBeforeAdd + 1;

            _parentTaskService.CreateParentTask(newTask);
            var addedParentTask = new ParentTask()
            {
                Parent_ID = newTask.Parent_ID,
                Parent_Task = newTask.Parent_Task
            };
            AssertObjects.PropertyValuesAreEquals(addedParentTask, _parentTask.Last());
            Assert.That(maxTaskBeforeAdd + 1, Is.EqualTo(newTask.Parent_ID));
        }

       
        #endregion
    }
}
