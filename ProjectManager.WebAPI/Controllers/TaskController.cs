using ProjectManager.BL;
using ProjectManager.BusinessEntities;
using ProjectManager.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjectManager.WebAPI.Controllers
{
    public class TaskController : ApiController
    {
        private readonly ITaskServices _taskServices;
        private readonly ILogger _loggerServices;
        private readonly IUserServices _userServices;


        #region Public Constructor  

        /// <summary>  
        /// Public constructor to initialize user service instance  
        /// </summary>  
        public TaskController()
        {
            _taskServices = new TaskServices();
            _userServices = new UserServices();

            _loggerServices = new LoggerException();
        }

        #endregion
        // GET: api/Task
        public HttpResponseMessage Get()
        {
            try
            {
                _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : TaskController | Method Name : GetAllTasks | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);

                var tasks = _taskServices.GetTaskSearch();
                if (tasks != null)
                {
                    var taskEntities = tasks as List<vw_TaskSearchEntity> ?? tasks.ToList();
                    if (taskEntities.Any())
                        return Request.CreateResponse(HttpStatusCode.OK, taskEntities);
                }
            }
            catch (Exception exception)
            {
                _loggerServices.LogException(exception, LoggerConstants.Informations.WebAPIInfo);
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Tasks not found");
        }

        // GET: api/Task/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : TaskController | Method Name : GetTaskById | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);

                var task = _taskServices.GetTaskById(id);
                if (task != null)
                    return Request.CreateResponse(HttpStatusCode.OK, task);
            }
            catch (Exception exception)
            {
                _loggerServices.LogException(exception, LoggerConstants.Informations.WebAPIInfo);
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No user found for this id");
        }

        // POST: api/Task
        public int Post([FromBody]TaskEntity taskEntity)
        {
            try
            {
                _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : TaskController | Method Name : CreateTask | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);

                int iTaskID = _taskServices.CreateTask(taskEntity);
                if(taskEntity.User_ID != null)
                {
                    int iUserID = Convert.ToInt32(taskEntity.User_ID);
                    var user = _userServices.GetUserById(iUserID);
                    user.Task_ID = iTaskID;

                    if (taskEntity.User_ID != 0)
                        _userServices.UpdateUser(iUserID, user);
                }                
                return iTaskID;
            }
            catch (Exception exception)
            {
                _loggerServices.LogException(exception, LoggerConstants.Informations.WebAPIInfo);
            }
            return 0;
        }

        // PUT: api/Task/5
        public bool Put(int id, [FromBody]TaskEntity taskEntity)
        {
            try
            {
                if (id > 0)
                {
                    _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : TaskController | Method Name : UpdateTask | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);
                    bool returnStatus = _taskServices.UpdateTask(id, taskEntity);

                    if (taskEntity.User_ID != null)
                    {
                        _taskServices.UpdateExistingUsersTask(id);
                        int iUserID = Convert.ToInt32(taskEntity.User_ID);
                        var user = _userServices.GetUserById(iUserID);
                        user.Task_ID = taskEntity.Task_ID;

                        if (taskEntity.User_ID != 0)
                            _userServices.UpdateUser(iUserID, user);
                    }

                    return returnStatus;
                }
            }
            catch (Exception exception)
            {
                _loggerServices.LogException(exception, LoggerConstants.Informations.WebAPIInfo);
            }
            return false;
        }

        // DELETE: api/Task/5
        public bool Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var task = _taskServices.GetTaskById(id);
                    if (task != null)
                    {
                        task.Status = "Completed";
                    }
                    return _taskServices.UpdateTask(id, task);
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
