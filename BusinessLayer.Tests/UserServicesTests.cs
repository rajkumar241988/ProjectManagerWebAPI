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
    public class UserServicesTests
    {
        #region Variables  
        private IUserServices _userService;
        private IUnitOfWork _unitOfWork;
        private List<User> _user;
        private GenericRepository<User> _userRepository;
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
            _userRepository = SetUpUserRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.UserRepository).Returns(_userRepository);
            _unitOfWork = unitOfWork.Object;
            _userService = new UserServices();
        }

        #endregion

        private GenericRepository<User> SetUpUserRepository()
        {

            // Initialise repository  
            var mockRepo = new Mock<GenericRepository<User>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior  
            mockRepo.Setup(p => p.GetAll()).Returns(_user);

            mockRepo.Setup(p => p.GetByID(It.IsAny<int>()))
                .Returns(new Func<int, User>(
                    id => _user.Find(p => p.User_ID.Equals(id))));


            mockRepo.Setup(p => p.Insert((It.IsAny<User>())))
                .Callback(new Action<User>(newUser =>
                {
                    _user.Add(newUser);
                }));


            mockRepo.Setup(p => p.Update(It.IsAny<User>()))
                .Callback(new Action<User>(usr =>
                {
                    var oldUser = _user.Find(a => a.User_ID == usr.User_ID);
                    oldUser = usr;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<User>()))
                .Callback(new Action<User>(usr =>
                {
                    var userToRemove =
                        _user.Find(a => a.User_ID == usr.User_ID);

                    if (userToRemove != null)
                        _user.Remove(userToRemove);
                }));

            // Return mock implementation object  
            return mockRepo.Object;
        }
        [TearDown]
        public void DisposeTest()
        {
            _userService = null;
            _unitOfWork = null;
            _userRepository = null;
            if (_dbEntities != null)
                _dbEntities.Dispose();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _user = SetUpUsers();
        }

        private static List<User> SetUpUsers()
        {
            var users = DataInitializer.GetAllUsers();
            return users;
        }
        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _user = null;
        }

        #region User Test Methods

        [Test]
        public void GetAllUserTest()
        {
            var users = _userService.GetAllUsers();
            var userList =
                users.Select(
                    userEntity =>
                    new User
                    {
                        User_ID = userEntity.User_ID,
                        First_Name = userEntity.First_Name,
                        Last_Name = userEntity.Last_Name,
                        Employee_ID = userEntity.Employee_ID,
                        Project_ID = userEntity.Project_ID,
                        Task_ID = userEntity.Task_ID
                    }).ToList();
            var comparer = new UserComparer();
            CollectionAssert.AreEqual(
                userList.OrderBy(user => user, comparer),
                _user.OrderBy(user => user, comparer), comparer);
        }

        ///<summary>  
        /// Service should return null  
        ///</summary>  
        [Test]
        public void GetAllUsersTestForNull()
        {
            _user.Clear();
            var users = _userService.GetAllUsers();
            Assert.IsEmpty(users);
            SetUpUsers();
        }

        ///<summary>  
        /// Service should return user if correct id is supplied  
        ///</summary>  
        [Test]
        public void GetUserByRightIdTest()
        {
            var user = _userService.GetUserById(1);

            if (user != null)
            {
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<UserEntity, User>();
                });
                IMapper mapper = config.CreateMapper();
                var userModel = mapper.Map<UserEntity, User>(user);

                AssertObjects.PropertyValuesAreEquals(userModel,
                    _user.Find(a => a.First_Name.Contains("Rajj")));
            }
        }
        ///<summary>  
        /// Service should return null  
        ///</summary>  
        [Test]
        public void GetUserByWrongIdTest()
        {
            var user = _userService.GetUserById(0);
            Assert.Null(user);
        }

        ///<summary>  
        /// Add new user test  
        ///</summary>  
        [Test]
        public void AddNewUserTest()
        {
            var newUser = new UserEntity()
            {
                User_ID=9,
                First_Name = "Sunil",
                Last_Name = "G",
                Employee_ID = 42343,
                Project_ID = null,
                Task_ID = null
            };

            var maxUserIDBeforeAdd = _user.Max(a => a.User_ID);
            newUser.User_ID = maxUserIDBeforeAdd + 1;

            _userService.CreateUsers(newUser);
            var addedUser = new User()
            {
                User_ID= newUser.User_ID,
                First_Name = newUser.First_Name,
                Last_Name = newUser.Last_Name,
                Employee_ID = newUser.Employee_ID,
                Project_ID = newUser.Project_ID,
                Task_ID = newUser.Task_ID
            };
            AssertObjects.PropertyValuesAreEquals(addedUser, _user.Last());
            Assert.That(maxUserIDBeforeAdd + 1, Is.EqualTo(newUser.User_ID));
        }

        ///<summary>  
        /// Update user test  
        ///</summary>  
        [Test]
        public void UpdateUserTest()
        {
            var firstUser = _user.First();
            firstUser.First_Name = "Raj kumar";
            var updatedUser = new UserEntity()
            {
                User_ID= firstUser.User_ID,
                First_Name = firstUser.First_Name,
                Last_Name = firstUser.Last_Name,
                Employee_ID = firstUser.Employee_ID,
                Project_ID = firstUser.Project_ID,
                Task_ID = firstUser.Task_ID
            };
            _userService.UpdateUser(firstUser.User_ID, updatedUser);
            Assert.That(firstUser.User_ID, Is.EqualTo(1)); // hasn't changed  
            Assert.That(firstUser.First_Name, Is.EqualTo("Raj kumar")); // First name changed  

        }

        ///<summary>  
        /// Delete user test  
        ///</summary>  
        [Test]
        public void DeleteUserTest()
        {
            int maxID = _user.Max(a => a.User_ID); // Before removal  
            var lastUser = _user.Last();

            // Remove last User  
            _userService.DeleteUser(lastUser.User_ID);
            var user = _userService.GetUserById(maxID - 1);
            Assert.That(maxID, Is.GreaterThan(user.User_ID)); // Max id reduced by 1  
        }
        #endregion
    }
}
