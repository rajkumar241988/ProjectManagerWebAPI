using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Logger
{
    public interface ILogger
    {
        void LogInfo(string infoMessage, LoggerConstants.Informations infoCode);
        void LogDebug(string infoMessage, LoggerConstants.Informations infoCode);
        void LogException(Exception exception, LoggerConstants.Informations infoCode);
    }
}
