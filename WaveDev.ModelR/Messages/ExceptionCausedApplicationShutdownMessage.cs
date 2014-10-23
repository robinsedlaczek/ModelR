using System;

namespace WaveDev.ModelR.Messages
{
    internal class ExceptionCausedApplicationShutdownMessage
    {
        public Exception Exception
        {
            get;
            set;
        }

        public bool ShowMessageToUser
        {
            get;
            set;
        }
    }
}