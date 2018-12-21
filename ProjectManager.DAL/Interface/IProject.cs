using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.DAL.Interface
{
    public interface IProject
    {
        int GetTotalProjectss();
        List<Project> GetProjects();
        User GetProjectByID();
        List<Project> AddProject(Project objProject);
        List<Project> UpdateProject(Project objProject);
        List<Project> DeleteProject(Project objProject);
    }
}
