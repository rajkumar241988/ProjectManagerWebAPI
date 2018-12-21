using Resolver;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.BL
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IProjectServices, ProjectServices>();
            registerComponent.RegisterType<ITaskServices, TaskServices>();
            registerComponent.RegisterType<IUserServices, UserServices>();
            registerComponent.RegisterType<IParentTaskServices, ParentTaskServices>();


        }
    }
}
