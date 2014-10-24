using System;

namespace WaveDev.ModelR.Communication
{
    /// <summary>
    /// Used to indicate that the current user is not authorized at the server. The client code
    /// should immediately shutdown the application.
    /// </summary>
    public class UserNotAuthorizedException : Exception
    {
        /// <summary>
        /// This constructor takes a message and the name of the not authorized user.
        /// </summary>
        /// <param name="message">Detailed problem information.</param>
        /// <param name="userName">The name of the unauthorized user.</param>
        public UserNotAuthorizedException(string message, string userName)
            : base(message)
        {
            UserName = userName;
        }

        /// <summary>
        /// This property gets the name of the unauthorized user.
        /// </summary>
        /// <returns>Returns the name of the unauthorized user.</returns>
        public string UserName
        {
            get;
            private set;
        }
    }
}
