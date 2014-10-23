using System;

namespace WaveDev.ModelR.Communication
{
    /// <summary>
    /// Used to indicate that the current user is not authorized at the server. The client code
    /// should immediately shutdown the application.
    /// </summary>
    public class UserNotAuthorizedException : Exception
    {
        public UserNotAuthorizedException(string message)
            : base(message)
        {

        }

    }
}
