using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WaveDev.ModelR.Server.Security
{
    public class ModelRAuthorizeAttribute : AuthorizeAttribute  
    {
        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            // TODO: [RS] REMOVE THIS !!! Security is completelly deactivated For development time.
            return true;

            var userName = (from token in hubIncomingInvokerContext.Hub.Context.Headers
                            where token.Key == "ModelRAuthToken_UserName"
                            select token).FirstOrDefault();

            var password = (from token in hubIncomingInvokerContext.Hub.Context.Headers
                            where token.Key == "ModelRAuthToken_UserPassword"
                            select token).FirstOrDefault();

            if (string.IsNullOrEmpty(userName.Value) || string.IsNullOrEmpty(password.Value))
                return false;

            return (userName.Value.CompareTo("Robin") == 0 && password.Value.CompareTo("Sedlaczek") == 0);
        }
    }
}
