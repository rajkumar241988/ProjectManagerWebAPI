using ProjectManager.BusinessEntities;
using ProjectManager.DAL;
using System.Collections.Generic;

namespace ProjectManager.BL
{
    /// <summary>  
    /// Project Service   
    /// </summary>  
    public interface IProjectServices
    {
        ProjectEntity GetProjectById(int projectId);
        IEnumerable<ProjectEntity> GetAllProjects();
        int CreateProject(ProjectEntity projectEntity);
        bool UpdateProject(int projectId, ProjectEntity projectEntity);
        bool DeleteProject(int projectId);
        IEnumerable<vw_ProjectSearchEntity> GetProjectsSearch();
    }
}
