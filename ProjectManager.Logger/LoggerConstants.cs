using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Logger
{
    public class LoggerConstants
    {
        public enum Informations
        {
            WebAPIInfo = 100,
            BusinessLayerInfo = 200,
            DALInfo = 300
        }

        public enum InfoLevel
        {
            Critical = 1,
            Medium = 2,
            Low = 2
        }
    }
}
