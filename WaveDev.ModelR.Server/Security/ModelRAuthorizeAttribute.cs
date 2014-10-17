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
            var authToken = (from token in hubIncomingInvokerContext.Hub.Context.Headers
                where token.Key == "ModelRAuthToken"
                select token).FirstOrDefault();

            return authToken.Value.Contains("Robin") && authToken.Value.Contains("Sedlaczek");
        }
    }
}
