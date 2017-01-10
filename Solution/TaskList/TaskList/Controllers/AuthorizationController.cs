using System.Web.Mvc;
using System.Web.Security;
using TaskList.Global;
using TaskList.Models;

namespace TaskList.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly Data data = new Data();

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User obj)
        {          
            if (ModelState.IsValid)
            {
                FormsAuthentication.RedirectFromLoginPage(obj.UserLogin, false); 
                data.Authorization(obj.UserLogin);
                return RedirectToAction("AllTasks", "Task");
            }
            return PartialView("Login", obj);         
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return View("Login");
        }
    }
}
