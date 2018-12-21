using ProjectManager.BL;
using ProjectManager.BusinessEntities;
using ProjectManager.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace ProjectManager.WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]

    public class UserController : ApiController
    {
        private readonly IUserServices _userServices;
        private readonly ILogger _loggerServices;

        #region Public Constructor  

        /// <summary>  
        /// Public constructor to initialize user service instance  
        /// </summary>  
        public UserController()
        {
            _userServices = new UserServices();
            _loggerServices = new LoggerException();
        }

        #endregion

        // GET: api/User
        public HttpResponseMessage Get()
        {
            try
            {
                _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : UserController | Method Name : Get | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);
                var users = _userServices.GetAllUsers();
                if (users != null)
                {
                    var userEntities = users as List<UserEntity> ?? users.ToList();
                    if (userEntities.Any())
                        return Request.CreateResponse(HttpStatusCode.OK, userEntities);
                }
            }
            catch (Exception exception)
            {
                _loggerServices.LogException(exception, LoggerConstants.Informations.WebAPIInfo);
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Users not found");
        }

        // GET: api/User/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : UserController | Method Name : GetUserById | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);
                var user = _userServices.GetUserById(id);
                if (user != null)
                    return Request.CreateResponse(HttpStatusCode.OK, user);

                throw new ApiDataException(1001, "No user found for this id.", HttpStatusCode.NotFound);

            }
            catch (Exception exception)
            {
                _loggerServices.LogException(exception, LoggerConstants.Informations.WebAPIInfo);
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No user found for this id");
        }

        // POST: api/User
        [ResponseType(typeof(UserEntity))]
        public IHttpActionResult PostUser(UserEntity userEntity)
        {
            try
            {
                _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : UserController | Method Name : CreateUsers | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);
                _userServices.CreateUsers(userEntity);
            }
            catch (Exception exception)
            {
                _loggerServices.LogException(exception, LoggerConstants.Informations.WebAPIInfo);
            }
            return CreatedAtRoute("DefaultApi", new { id = userEntity.User_ID }, userEntity);
        }

        // PUT: api/User/5
        public bool Put(int id, UserEntity userEntity)
        {
            try
            {
                if (id > 0)
                {
                    _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : UserController | Method Name : UpdateUser | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);
                    return _userServices.UpdateUser(id, userEntity);
                }
            }
            catch (Exception exception)
            {
                _loggerServices.LogException(exception, LoggerConstants.Informations.WebAPIInfo);
            }
            return false;
        }

        // DELETE: api/User/5
        public bool Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : UserController | Method Name : DeleteUser | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);
                    return _userServices.DeleteUser(id);
                }
            }
            catch (Exception exception)
            {
                _loggerServices.LogException(exception, LoggerConstants.Informations.WebAPIInfo);
            }
            return false;
        }
    }
}
