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
    public class ProjectServicesTests
    {
        #region Variables  
        private IProjectServices _projectService;
        private IUnitOfWork _unitOfWork;
        private List<Project> _projects;
        private GenericRepository<Project> _projectRepository;
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
            _projectRepository = SetUpProjectRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.ProjectRepository).Returns(_projectRepository);
            _unitOfWork = unitOfWork.Object;
            _projectService = new ProjectServices();
        }

        #endregion

        private GenericRepository<Project> SetUpProjectRepository()
        {

            // Initialise repository  
            var mockRepo = new Mock<GenericRepository<Project>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior  
            mockRepo.Setup(p => p.GetAll()).Returns(_projects);

            mockRepo.Setup(p => p.GetByID(It.IsAny<int>()))
                .Returns(new Func<int, Project>(
                    id => _projects.Find(p => p.Project_ID.Equals(id))));


            mockRepo.Setup(p => p.Insert((It.IsAny<Project>())))
                .Callback(new Action<Project>(newProject => 
                {
                    //dynamic maxProject_ID = _projects.Last().Project_ID;
                    //dynamic nextProject_ID = maxProject_ID + 1;
                    //newProject.Project_ID = nextProject_ID;
                    _projects.Add(newProject);
                }));


            mockRepo.Setup(p => p.Update(It.IsAny<Project>()))
                .Callback(new Action<Project>(proj =>
                {
                    var oldProject = _projects.Find(a => a.Project_ID == proj.Project_ID);
                    oldProject = proj;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Project>()))
                .Callback(new Action<Project>(proj =>
                {
                    var projectToRemove =
                        _projects.Find(a => a.Project_ID == proj.Project_ID);

                    if (projectToRemove != null)
                        _projects.Remove(projectToRemove);
                }));

            // Return mock implementation object  
            return mockRepo.Object;
        }
        [TearDown]
        public void DisposeTest()
        {
            _projectService = null;
            _unitOfWork = null;
            _projectRepository = null;
            if (_dbEntities != null)
                _dbEntities.Dispose();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _projects = SetUpProjects();
        }

        private static List<Project> SetUpProjects()
        {
            var projects = DataInitializer.GetAllProjects();
            return projects;
        }
        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _projects = null;
        }

        #region Project Test Methods

        [Test]
        public void GetAllProjectsTest()
        {
            var projects = _projectService.GetAllProjects();
            var projectList =
                projects.Select(
                    projectEntity =>
                    new Project
                    {
                        Project_ID = projectEntity.Project_ID,
                        Project1 = projectEntity.Project1,
                        Priority = projectEntity.Priority,
                        Start_Date = projectEntity.Start_Date,
                        End_Date = projectEntity.End_Date
                    }).ToList();
            var comparer = new ProjectComparer();
            CollectionAssert.AreEqual(
                projectList.OrderBy(project => project, comparer),
                _projects.OrderBy(project => project, comparer), comparer);
        }

        ///<summary>  
        /// Service should return null  
        ///</summary>  
        [Test]
        public void GetAllProjectsTestForNull()
        {
            _projects.Clear();
            var products = _projectService.GetAllProjects();
            Assert.Null(products);
            SetUpProjects();
        }

        ///<summary>  
        /// Service should return product if correct id is supplied  
        ///</summary>  
        [Test]
        public void GetProjectByRightIdTest()
        {
            var project = _projectService.GetProjectById(2);
            //var project = _unitOfWork.ProjectRepository.GetByID(2);

            if (project != null)
            {
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<ProjectEntity, Project>();
                });
                IMapper mapper = config.CreateMapper();
                var projectModel = mapper.Map<ProjectEntity, Project>(project);

                AssertObjects.PropertyValuesAreEquals(projectModel,
                    _projects.Find(a => a.Project1.Contains("updated")));
            }
        }
        ///<summary>  
        /// Service should return null  
        ///</summary>  
        [Test]
        public void GetProjectByWrongIdTest()
        {
            var project = _projectService.GetProjectById(0);
            Assert.Null(project);
        }

        ///<summary>  
        /// Add new product test  
        ///</summary>  
        [Test]
        public void AddNewProjectTest()
        {
            var newProduct = new ProjectEntity()
            {
                Project1 = "AFMS1",
                Priority = "2",
                Start_Date = Convert.ToDateTime("2018-01-17"),
                End_Date = Convert.ToDateTime("2018-01-22")
            };

            var maxProductIDBeforeAdd = _projects.Max(a => a.Project_ID);
            newProduct.Project_ID = maxProductIDBeforeAdd + 1;

            _projectService.CreateProject(newProduct);
            var addedproduct = new Project()
            {
                Project1 = newProduct.Project1, Project_ID = newProduct.Project_ID
            };
            AssertObjects.PropertyValuesAreEquals(addedproduct, _projects.Last());
            Assert.That(maxProductIDBeforeAdd + 1, Is.EqualTo(newProduct.Project_ID));
        }

        ///<summary>  
        /// Update project test  
        ///</summary>  
        [Test]
        public void UpdateProjectTest()
        {
            var firstProject = _projects.First();
            firstProject.Project1 = "Project updated";
            var updatedProduct = new ProjectEntity()
            {
                Project_ID = firstProject.Project_ID,
                Project1 = firstProject.Project1,
                Priority = firstProject.Priority,
                Start_Date = firstProject.Start_Date,
                End_Date = firstProject.End_Date
            };
            _projectService.UpdateProject(firstProject.Project_ID, updatedProduct);
            Assert.That(firstProject.Project_ID, Is.EqualTo(2)); // hasn't changed  
            Assert.That(firstProject.Project1, Is.EqualTo("Project updated")); // Project name changed  

        }

        ///<summary>  
        /// Delete project test  
        ///</summary>  
        [Test]
        public void DeleteProjectTest()
        {
            int maxID = _projects.Max(a => a.Project_ID); // Before removal  
            var lastProduct = _projects.Last();

            // Remove last Product  
            _projectService.DeleteProject(lastProduct.Project_ID);
            var project = _projectService.GetProjectById(maxID-1);
            if(project != null)
                Assert.That(maxID, Is.GreaterThan(project.Project_ID)); // Max id reduced by 1  
        }
        #endregion

    }
}
