using System.Security.Principal;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WaveDev.ModelR.Server.Security
{
    public class ModelRAuthorizeAttribute : AuthorizeAttribute  
    {
        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            var user = request.User;

            return user.Identity.IsAuthenticated && user.Identity.Name == "Robin";
        }

        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {

            return base.AuthorizeHubMethodInvocation(hubIncomingInvokerContext, appliesToMethod);
        }

        protected override bool UserAuthorized(IPrincipal user)
        {
            return base.UserAuthorized(user);
        }
    }
}
