using Microsoft.Owin;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WaveDev.ModelR.Server.Security
{
    public class ModelRAuthenticationMiddleware : OwinMiddleware
    {
        public ModelRAuthenticationMiddleware(OwinMiddleware next) : base(next)
        {

        }

        public override async Task Invoke(IOwinContext context)
        {
            var requiredUserName = context.Request.Headers["ModelRAuthToken_UserName"];
            var requiredPassword = context.Request.Headers["ModelRAuthToken_UserPassword"];

            var userStore = XDocument.Load(@"Security\Users.xml");

            var users = from userFound in userStore.Descendants("User")
                        where userFound.Attribute("Name").Value.CompareTo(requiredUserName) == 0
                        select new
                        {
                            UserName = userFound.Attribute("Name").Value,
                            Password = userFound.Attribute("Password").Value,
                            ProfileImageUri = @"Images\" + userFound.Attribute("Image").Value
                        };

            var user = users.FirstOrDefault();

            if (user != null && user.Password.CompareTo(requiredPassword) == 0)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("ProfileImageUri", user.ProfileImageUri)
                };

                var identity = new ClaimsIdentity(claims, "Password");

                context.Request.User = new ClaimsPrincipal(identity);
            }

            await Next.Invoke(context);
        }
    }
}
