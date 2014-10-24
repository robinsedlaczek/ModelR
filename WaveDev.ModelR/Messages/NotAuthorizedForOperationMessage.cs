namespace WaveDev.ModelR.Messages
{
    /// <summary>
    /// This message is used when the user tries to access a hub service unauthorized. The client proxy layer
    /// sends this message in order to notify the appropriate models.
    /// </summary>
    internal class NotAuthorizedForOperationMessage
    {
        /// <summary>
        /// Constructor that takes some info text to indicate some reasons and help for the user.
        /// </summary>
        /// <param name="info">The info text for the user.</param>
        public NotAuthorizedForOperationMessage(string info)
        {
            Info = info;
        }

        /// <summary>
        /// This property sets or gets some further information about the authorization problem.
        /// </summary>
        public string Info
        {
            get;
            set;
        }

    }
}