using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WaveDev.ModelR.Server.Security
{
    public class ModelRAuthorizeAttribute : AuthorizeAttribute  
    {
        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            // return hubIncomingInvokerContext.Hub.Context.User.Identity.IsAuthenticated;

            return base.AuthorizeHubMethodInvocation(hubIncomingInvokerContext, appliesToMethod);
        }
    }
}
