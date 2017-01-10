using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TaskList.Global;
using TaskList.Models;

namespace TaskList.Controllers
{
    /// <summary>
    /// Работа с задачами
    /// </summary>
    public class TaskController : Controller
    {
        private FormsAuthenticationTicket ticket;
        private readonly Data data = new Data();

        public ActionResult AllTasks()
        {
            try
            {
                var idTicket = (FormsIdentity)User.Identity;
                ticket = idTicket.Ticket;
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Authorization");
            }
            AddViewInCookie("AllTasks", null);
            return View("Tasks", data.ShowAllTasks(ticket.Name));
        }

        public ActionResult Dates()
        {
            try
            {
                var idTicket = (FormsIdentity)User.Identity;
                ticket = idTicket.Ticket;
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Authorization");
            }
            return PartialView(data.GetAllDates(ticket.Name));
        }

        public ActionResult Marks()
        {
            try
            {
                var idTicket = (FormsIdentity)User.Identity;
                ticket = idTicket.Ticket;
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Authorization");
            }
            return PartialView(data.GetAllMarks(ticket.Name));
        }

        public ActionResult SearchTasksByDate(string parameter)
        {
            Session["View"] = "SearchTasksByDate";
            Session["Parameter"] = parameter;
            AddViewInCookie("SearchTasksByDate", parameter);
            try
            {
                var idTicket = (FormsIdentity)User.Identity;
                ticket = idTicket.Ticket;
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Authorization");
            }
            IEnumerable<Task> model = null;
            var redirect = false;
            if (parameter != null)
                model = data.SearchTasksByDate(parameter, ticket.Name);
            else
                redirect = true;
            if (model == null)
            {
                redirect = true;
            }
            if (redirect)
            {
                return RedirectToAction("AllTasks");
            }
            return View("Tasks", model);
        }

        public ActionResult SearchTasksByNullDate()
        {
            try
            {
                var idTicket = (FormsIdentity)User.Identity;
                ticket = idTicket.Ticket;
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Authorization");
            }
            AddViewInCookie("SearchTasksByNullDate", null);
            return View("Tasks", data.SearchTasksByNullDate(ticket.Name));
        }

        [ValidateInput(false)]
        public ActionResult SearchTasksByMark(string parameter)
        {
            try
            {
                var idTicket = (FormsIdentity)User.Identity;
                ticket = idTicket.Ticket;
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Authorization");
            }
            AddViewInCookie("SearchTasksByMark", parameter);
            var model = parameter != null ? data.SearchTasksByMarks(parameter, ticket.Name) : data.ShowAllTasks(ticket.Name);
            if (model == null)
            {
                return RedirectToAction("AllTasks");
            }
            return View("Tasks", model);
        }

        public ActionResult EditTask(int id)
        {
            try
            {
                var idTicket = (FormsIdentity)User.Identity;
                ticket = idTicket.Ticket;
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Authorization");
            }
            var task = data.FindTask(id);
            return task.Name == null ? (ActionResult)RedirectToAction("AllTasks") : PartialView(task);
        }

        public ActionResult DeleteTask(int id)
        {
            string view;
            string parameter;
            GetViewFromCookie(out view, out parameter);
            data.HideTask(id);
            return RedirectToAction(view, new { parameter });
        }

        public ActionResult NewTask()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddTask(Task obj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var idTicket = (FormsIdentity)User.Identity;
                    ticket = idTicket.Ticket;
                }
                catch (Exception)
                {
                    return RedirectToAction("Login", "Authorization");
                }
                data.AddTask(obj.Name, obj.Deadline, obj.Marks, ticket.Name);
                string view;
                string parameter;
                GetViewFromCookie(out view, out parameter);
                return RedirectToAction(view, new {parameter});
            }

            return PartialView("NewTask", obj);
        }

        public ActionResult ChangeTask(Task task)
        {
            if (ModelState.IsValid)
            {
                string view;
                string parameter;
                GetViewFromCookie(out view,out parameter);
                data.EditTask(task.Id, task.Name, task.Deadline, task.Marks);
                return RedirectToAction(view, new { parameter });
            }
            return PartialView("EditTask", task);
        }
        /// <summary>
        /// Добавляет в cookie данные из представления
        /// </summary>
        /// <param name="view">название представления</param>
        /// <param name="parameter">значение параметра</param>
        public void AddViewInCookie(string view, string parameter)
        {
            Request.Cookies.Clear();
            var viewCookie = new HttpCookie("View");
            viewCookie.Values["ViewName"] = view;
            if (parameter != null)
            {
               viewCookie.Values["Parameter"] = HttpUtility.HtmlEncode(parameter);
            }
            else
            {
                viewCookie.Values["Parameter"] = null;
            }
            viewCookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(viewCookie);

        }
        /// <summary>
        /// Получает из cookie данные о предыдущем представлении
        /// </summary>
        /// <param name="view">название представления</param>
        /// <param name="parameter">значение параметра</param>
        public void GetViewFromCookie(out string view, out string parameter)
        {
            parameter = null;
            if (Request.Cookies["View"] != null)
            {
                view = Request.Cookies["View"]["ViewName"];
                if (Request.Cookies["View"]["Parameter"] != null)
                {
                    parameter = HttpUtility.HtmlDecode(Request.Cookies["View"]["Parameter"]);
                }
            }
            else
            {
                view = "AllTasks";
            }
        }
    }
}
