using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using warpspeed.Models;
using warpspeed.Helpers;

namespace warpspeed.Controllers
{
    public class ProjectController : Controller
    {
        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult Create(string Title)
        {
            bool Success = false;
            Project proj = new Project();
            using (warpspeedEntities db = new warpspeedEntities())
            {
                WUser user = db.WUsers.First(u => u.ID == new Guid(User.Identity.Name));

                proj.ID = Guid.NewGuid();
                proj.Title = Title;
                proj.CreatedByID = user.ID;
                proj.PercentComplete = 0;
                WUserProject userProject = new WUserProject(){
                    WUser = user,
                    Privilege = db.Privileges.First(),
                    ProjectID = proj.ID,
                };
                proj.WUserProjects.Add(userProject);

                proj.Deleted = false;

                db.Projects.Add(proj);
                try
                {
                    db.SaveChanges();
                    Success = true;
                }
                catch (Exception ex)
                {

                }
            }
            return Json(proj.ID);
        }


        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult Rename(string NewTitle, Guid ProjectID)
        {
            bool Success = false;
            
            using (warpspeedEntities db = new warpspeedEntities())
            {
                Project proj = db.Projects.First(p => p.ID == ProjectID);
                proj.Title = NewTitle;
                try
                {
                    db.SaveChanges();
                    Success = true;
                }
                catch (Exception ex)
                {

                }
            }
            return Json(Success);
        }

        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult Delete(Guid ProjectID)
        {
            bool Success = false;
            using (warpspeedEntities db = new warpspeedEntities())
            {
                try
                {
                    db.Projects.First(p => p.ID == ProjectID).Deleted = true;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
            return Json(Success);
        }

        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult AddUserToProject(Guid userID, Guid projectID)
        {
            bool Success = false;
            using (warpspeedEntities db = new warpspeedEntities())
            {
                try
                {
                    Guid fullPrivileges = db.Privileges.First().ID;

                    if (!db.WUserProjects.Any(up => up.ProjectID == projectID && up.WUserID == userID))
                    {
                        
                        //Add user to project
                        WUserProject userProject = new WUserProject()
                        {
                            WUserID = userID,
                            ProjectID = projectID,
                            PrivilegeID = fullPrivileges,
                        };
                        db.WUserProjects.Add(userProject);
                        db.SaveChanges();

                        //Add user to all subprojects
                        foreach(SubProject subProj in db.SubProjects.Where(sp=>sp.ProjectID==projectID).ToList()){
                            //check to see if this user has already been shared with
                            if(!db.WUserSubprojects.Any(usp=>usp.SubprojectID==subProj.ID&&usp.WUserID==userID)){
                                WUserSubproject userSubProj = new WUserSubproject(){
                                    WUserID = userID,
                                    SubprojectID = subProj.ID,
                                    PrivilegeID = fullPrivileges,
                                };
                                db.WUserSubprojects.Add(userSubProj);
                                db.SaveChanges();
                            }else{
                                //user already added
                            }
                        
                        }
                        //Make notification that user has been added to the project
                        ProjectNotification projNotification = new ProjectNotification(){
                            ID=Guid.NewGuid(),
                            WUserID=new Guid(User.Identity.Name),
                            ProjectID = projectID,
                            TypeID = 3,
                        };
                        db.ProjectNotifications.Add(projNotification);
                        db.SaveChanges();
                        
                        Success = true;
                    }
                    else
                    {
                        //User is already part of this project; can't re-add
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return Json(Success);
        }

        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult RemoveUserFromProject(Guid userID, Guid projectID)
        {
            bool Success = false;
            using (warpspeedEntities db = new warpspeedEntities())
            {
                try
                {
                    if (db.WUserProjects.Any(up => up.ProjectID == projectID && up.WUserID == userID))
                    {
                        if (db.WUserSubprojects.Any(usp => usp.SubProject.ProjectID == projectID && usp.WUserID == userID))
                        {
                            List<WUserSubproject> userSubProjects = db.WUserSubprojects.Where(usp => usp.SubProject.ProjectID == projectID && usp.WUserID == userID).ToList();
                            foreach (WUserSubproject userSubProject in userSubProjects.ToList())
                            {
                                db.WUserSubprojects.Remove(userSubProject);
                            }
                        }
                        else
                        {
                            //project has no subprojects
                        }
                        WUserProject userProject = db.WUserProjects.First(up => up.ProjectID == projectID && up.WUserID == userID);
                        db.WUserProjects.Remove(userProject);
                        db.SaveChanges();
                        Success = true;
                    }
                    else
                    {
                        //User isn't part of the project - can't remve if they're not there!
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return Json(Success);
        }

        [AuthenticationHelper.IsUser]
        public PartialViewResult ProjectNotifications(Guid ProjectID)
        {
            List<string> notifications = new List<string>();
            Guid UserID = new Guid(User.Identity.Name);
            using (warpspeedEntities db = new warpspeedEntities())
            {
                foreach(TaskNotifcation tn in db.TaskNotifcations.Where(tn => tn.Task.SubProject.ProjectID == ProjectID && tn.WUserID!=UserID).OrderByDescending(tn=>tn.Created).ToList()){
                    notifications.Add(String.Format("{0} {1} a task \"{2}\"",tn.WUser.FirstName,tn.NotificationType.Title.ToLower(),tn.Task.Title));
                }
            }

            return PartialView(notifications);
        }
    }
}
