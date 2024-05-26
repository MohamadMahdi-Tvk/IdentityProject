using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.UI.Controllers
{
    [Authorize(Roles = "Admin,Operator")]
    //[Authorize(Roles = "")]
    public class AuthorizeTestController : Controller
    {
        //[AllowAnonymous]
        public string Index()
        {
            return "Index";
        }

        [Authorize(Roles = "Operator")]
        public string Edit()
        {
            return "Index";
        }
    }
}
