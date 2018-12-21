using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using ProjectManager.BL;
using ProjectManager.BusinessEntities;
using ProjectManager.DAL;
using ProjectManager.DAL.GenericRepository;
using ProjectManager.DAL.UnitOfWork;
using ProjectManager.WebAPI;
using ProjectManager.WebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Hosting;
using TestHelper;

namespace ProjectManager.WebAPITests
{
    public class ProjectControllerTest
    {
        #region Variables  
        private IProjectServices _projectService;
        private IUnitOfWork _unitOfWork;
        private List<Project> _projects;
        private GenericRepository<Project> _projectRepository;
        private ProjectManagerEntities _dbEntities;
        private HttpClient _client;
        private HttpResponseMessage _response;
        private const string ServiceBaseURL = "http://172.18.4.244/api/project/";

        #endregion

        #region Setup  
        ///<summary>  
        /// Re-initializes test.  
        ///</summary>  
        [SetUp]
        public void ReInitializeTest()
        {
            _client = new HttpClient { BaseAddress = new Uri(ServiceBaseURL) };

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
            if (_response != null) _response.Dispose();
            if (_client != null) _client.Dispose();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _projects = SetUpProjects();
            _dbEntities = new Mock<ProjectManagerEntities>().Object;
            _projectRepository = SetUpProjectRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.ProjectRepository).Returns(_projectRepository);
            _unitOfWork = unitOfWork.Object;
            _projectService = new ProjectServices();
            _client = new HttpClient
            {
                BaseAddress = new Uri(ServiceBaseURL)
            };
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

        [Test]
        public void GetAllProjectsTest()
        {
            var projectController = new ProjectController()
            {               
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL)
                }
            };
            projectController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            _response = projectController.Get();
            var responseResultSearch = JsonConvert.DeserializeObject<List<vw_ProjectSearch>>(_response.Content.ReadAsStringAsync().Result);
            var projectList =
                responseResultSearch.Select(
                    projectEntity =>
                    new Project
                    {
                        Project_ID = projectEntity.Project_ID,
                        Project1 = projectEntity.ProjectName,
                        Priority = projectEntity.Project_Priority,
                        Start_Date = projectEntity.Start_Date,
                        End_Date = projectEntity.End_Date
                    }).ToList();
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(projectList.Any(), true);
            var comparer = new ProjectComparer();
            CollectionAssert.AreEqual(projectList.OrderBy(project => project, comparer), _projects.OrderBy(project => project, comparer), comparer);
        }

        [Test]
        public void GetProjectByIdTest()
        {
            var projectController = new ProjectController()
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "get/2")
                }
            };
            projectController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            _response = projectController.Get(2);
            var responseResult = JsonConvert.DeserializeObject<Project>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            AssertObjects.PropertyValuesAreEquals(responseResult, _projects.Find(a => a.Project1.Contains("Project updated")));
        }

       

        [Test]
        public void CreateProjectTest()
        {
            var projectController = new ProjectController()
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(ServiceBaseURL + "Create")
                }
            };
            projectController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var newProject = new ProjectEntity()
            {
                Project_ID = 0,
                Project1 = "Project Created New ",
                Priority = "3",
                Start_Date = Convert.ToDateTime("2018-01-17"),
                End_Date = Convert.ToDateTime("2018-01-22")
            };
            projectController.Post(newProject);

            _response = projectController.Get();
            var responseResultSearch = JsonConvert.DeserializeObject<List<vw_ProjectSearch>>(_response.Content.ReadAsStringAsync().Result);
            var projectList =
                responseResultSearch.Select(
                    projectEntity =>
                    new Project
                    {
                        Project_ID = projectEntity.Project_ID,
                        Project1 = projectEntity.ProjectName,
                        Priority = projectEntity.Project_Priority,
                        Start_Date = projectEntity.Start_Date,
                        End_Date = projectEntity.End_Date
                    }).ToList();
            var addedproject = new Project()
            {
                Project1 = newProject.Project1,
                Project_ID = newProject.Project_ID
            };
            AssertObjects.PropertyValuesAreEquals(addedproject, projectList.Last());
        }
        [Test]
        public void UpdateProjectTest()
        {
            var projectController = new ProjectController()
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseURL + "put/2")
                }
            };
            projectController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var firstProject = _projects.First();
            firstProject.Project1 = "updated";
            var updatedProject = new ProjectEntity()
            {
                Project_ID = firstProject.Project_ID,
                Project1 = firstProject.Project1,
                Priority = firstProject.Priority,
                Start_Date = firstProject.Start_Date,
                End_Date = firstProject.End_Date
            };
            projectController.Put(firstProject.Project_ID, updatedProject);
            Assert.That(firstProject.Project_ID, Is.EqualTo(2)); // hasn't changed
        }

        [Test]
        public void DeleteProjectTest()
        {
            var projectController = new ProjectController()
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseURL + "delete")
                }
            };
            projectController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var lastProject = _projects.Last();
            int maxID = _projects.Max(a => a.Project_ID); // Before removal

            // Remove last Product
            projectController.Delete(lastProject.Project_ID);

            _response = projectController.Get();
            var responseResultSearch = JsonConvert.DeserializeObject<List<vw_ProjectSearch>>(_response.Content.ReadAsStringAsync().Result);
            var projectList =
                responseResultSearch.Select(
                    projectEntity =>
                    new Project
                    {
                        Project_ID = projectEntity.Project_ID,
                        Project1 = projectEntity.ProjectName,
                        Priority = projectEntity.Project_Priority,
                        Start_Date = projectEntity.Start_Date,
                        End_Date = projectEntity.End_Date
                    }).ToList();

            if(projectList != null)
                Assert.That(maxID, Is.GreaterThan(projectList.Max(a => a.Project_ID))); // Max id reduced by 1
        }


        #region Integration Test

        /// <summary>
        /// Get all products test
        /// </summary>
        [Test]
        public void GetAllProjectsIntegrationTest()
        {
            #region To be written inside Setup method specifically for integration tests
            var client = new HttpClient { BaseAddress = new Uri(ServiceBaseURL) };
            MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            #endregion
            _response = client.GetAsync(ServiceBaseURL).Result;
            var responseResult =
                JsonConvert.DeserializeObject<List<vw_ProjectSearchEntity>>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseResult.Any(), true);
        }

        #endregion

    }
}
