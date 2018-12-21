using ProjectManager.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.DAL.Component
{
    public class Project : IProject
    {
        public List<DAL.Project> AddProject(DAL.Project objProject)
        {
            throw new NotImplementedException();
        }

        public List<DAL.Project> DeleteProject(DAL.Project objProject)
        {
            throw new NotImplementedException();
        }

        public DAL.User GetProjectByID()
        {
            throw new NotImplementedException();
        }

        public List<DAL.Project> GetProjects()
        {
            throw new NotImplementedException();
        }

        public int GetTotalProjectss()
        {
            throw new NotImplementedException();
        }

        public List<DAL.Project> UpdateProject(DAL.Project objProject)
        {
            throw new NotImplementedException();
        }
    }
}
