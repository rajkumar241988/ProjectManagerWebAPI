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
    public class ParentTaskController : ApiController
    {
        private readonly IParentTaskServices _taskServices;
        private readonly ILogger _loggerServices;

        public ParentTaskController()
        {
            _taskServices = new ParentTaskServices();
            _loggerServices = new LoggerException();
        }
        // GET: api/ParentTask
        public HttpResponseMessage Get()
        {
            try
            {
                _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : TaskController | Method Name : GetAllTasks | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);

                var tasks = _taskServices.GetAllParentTasks();
                if (tasks != null)
                {
                    var taskEntities = tasks as List<ParentTaskEntity> ?? tasks.ToList();
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

        // GET: api/ParentTask/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ParentTask
        public int Post([FromBody]ParentTaskEntity taskEntity)
        {
            try
            {
                _loggerServices.LogInfo("InfoCode: API Info | Message :" + "File Name : ParentTaskController | Method Name : CreateTask | Description : Method Begin", LoggerConstants.Informations.WebAPIInfo);

                return _taskServices.CreateParentTask(taskEntity);
            }
            catch (Exception exception)
            {
                _loggerServices.LogException(exception, LoggerConstants.Informations.WebAPIInfo);
            }
            return 0;
        }

        // PUT: api/ParentTask/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ParentTask/5
        public void Delete(int id)
        {
        }
    }
}
