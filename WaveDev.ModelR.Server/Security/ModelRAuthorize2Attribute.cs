using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WaveDev.ModelR.Server.Security
{
    public class ModelRAuthorize2Attribute : AuthorizeAttribute  
    {
        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            return false;
        }
    }
}
