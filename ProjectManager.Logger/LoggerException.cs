using System;

namespace ProjectManager.Logger
{
    public class LoggerException : ILogger
    {
        #region VARIABLES/PROPERTY DECLARATION
        log4net.ILog logger = log4net.LogManager.GetLogger("FileAppender");
        private string infoMessage;
        #endregion        

        #region EXPOSED PUBLIC METHODS       
        
        public void LogInfo(string infoMessage, LoggerConstants.Informations infoCode)
        {
            this.infoMessage = infoMessage;
            logger.Info(" InfoCode:" + infoCode + " Message :" + infoMessage);
        }
        public void LogDebug(string infoMessage, LoggerConstants.Informations infoCode)
        {
            this.infoMessage = infoMessage;
            logger.Debug(" InfoCode:" + infoCode + " Message :" + infoMessage);
        }
        public void LogException(Exception exception, LoggerConstants.Informations infoCode)
        {
            this.infoMessage = exception.ToString();
            logger.Error(" InfoCode:" + infoCode + " Message :" + infoMessage);
        }
        
        #endregion

    }
}
