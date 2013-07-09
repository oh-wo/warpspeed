using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;
using warpspeed.Helpers;
using warpspeed.Models;

namespace warpspeed.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("WarpDriveEngaged");
            };
            return View();
        }


        [AuthenticationHelper.IsUser]
        public ActionResult VersionOne()
        {
            List<Project> projects = new List<Project>();
            using (warpspeedEntities db = new warpspeedEntities())
            {
                projects = db.Projects.Where(p => p.WUser.ID == new Guid(User.Identity.Name)).Include("SubProjects").Include("SubProjects.Tasks").ToList();
            }
            return View(projects);
        }

        [AuthenticationHelper.IsUser]
        public ActionResult WarpDriveEngaged()
        {
            List<Project> projects = new List<Project>();
            using (warpspeedEntities db = new warpspeedEntities())
            {
                if (db.Projects.Any(p => p.WUserProjects.Any(up => (up.WUser.ID == new Guid(User.Identity.Name)) && !(p.Deleted ?? false))))
                {
                    projects = db.Projects.Where(p => p.WUserProjects.Any(up => (up.WUser.ID == new Guid(User.Identity.Name)) && !(p.Deleted ?? false))).Include("ProjectNotifications").Include("WUserProjects.WUser").Include("SubProjects").Include("SubProjects.SubProjectNotifcations").Include("SubProjects.WUserSubprojects").Include("SubProjects.Tasks").Include("SubProjects.Tasks.TaskNotifcations").Include("SubProjects.Tasks.TaskNotifcations.NotificationSeens").ToList();
                }
                else
                {
                    //No projects for this..
                }
            }
            return View(projects);
        }

        [AuthenticationHelper.IsUser]
        public PartialViewResult Workspace(Project proj)
        {
            return PartialView(proj);
        }

        [AuthenticationHelper.IsUser]
        public PartialViewResult WorkspaceID(Guid pid)
        {
            Project proj = new Project();
            using (warpspeedEntities db = new warpspeedEntities())
            {
                if (db.Projects.Any(p => p.ID == pid && !(p.Deleted ?? false)))
                {
                    proj = db.Projects.Where(p => p.ID == pid).Include("ProjectNotifications").Include("WUserProjects.WUser").Include("SubProjects").Include("SubProjects.SubProjectNotifcations").Include("SubProjects.WUserSubprojects").Include("SubProjects.WUserSubprojects.WUser").Include("SubProjects.Tasks").Include("SubProjects.Tasks.TaskNotifcations").Include("SubProjects.Tasks.TaskNotifcations.NotificationSeens").First();
                }
            }
            return PartialView("Workspace", proj);
        }

        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult SearchNames(string searchString, Guid ID, bool SubProject, bool Project)
        {
            List<NameId> people = new List<NameId>();
            using (warpspeedEntities db = new warpspeedEntities())
            {
                List<WUser> users = new List<WUser>();
                if (SubProject)
                {
                    users = db.WUsers.Where(u => (u.FirstName.Contains(searchString) || u.Email.Contains(searchString) || u.LastName.Contains(searchString)) && !u.WUserSubprojects.Any(sp => sp.SubprojectID != ID)).ToList();
                }
                else
                {
                    if (Project)
                    {
                        users = db.WUsers.Where(u => (u.FirstName.Contains(searchString) || u.Email.Contains(searchString) || u.LastName.Contains(searchString)) && !u.Projects.Any(p=>p.ID==ID)).ToList();
                    }
                }
                foreach (WUser person in users)
                {
                    NameId nameId = new NameId()
                    {
                        Id = person.ID,
                        Name = String.Format("{0} {1}", person.FirstName, person.LastName),
                    };
                    people.Add(nameId);
                };
            }
            return Json(people);
        }
        public class NameId
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
        }

    }
}
