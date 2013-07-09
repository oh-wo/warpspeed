using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using warpspeed.Helpers;
using warpspeed.Models;
using System.Data;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace warpspeed.Controllers
{
    public class SubProjectController : Controller
    {
        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult Create(Guid ProjectID, string Title)
        {
            SubProject sp = new SubProject()
            {
                ID = Guid.NewGuid(),
                ProjectID = ProjectID,
                Title = Title,
                Created = DateTime.UtcNow,
                CreatedID = new Guid(User.Identity.Name),
            };


            using (warpspeedEntities db = new warpspeedEntities())
            {
                foreach (WUserProject userProj in db.WUserProjects.Where(up => up.ProjectID == ProjectID).ToList())
                {
                    WUserSubproject userSub = new WUserSubproject()
                {
                    WUserID = userProj.WUserID,
                    SubprojectID = sp.ID,
                    PrivilegeID = db.Privileges.First().ID,
                };
                    sp.WUserSubprojects.Add(userSub);
                }
                try
                {
                    db.SubProjects.Add(sp);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
            return Json(sp.ID);
        }
        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult Delete(Guid SubProjectID)
        {
            bool Success = false;
            
            using (warpspeedEntities db = new warpspeedEntities())
            {
                var ctx = ((IObjectContextAdapter)db).ObjectContext;
                try
                {
                    
                    SubProject subProj = db.SubProjects.First(sp => sp.ID == SubProjectID);
                    #region DeleteTasks
                    NotificationHelper nHelper = new NotificationHelper();
                    List<Task> tasks = db.Tasks.Where(t => t.SubProjectID == SubProjectID).ToList();
                    foreach (Task childTask in tasks.ToList())
                    {
                        //Get all notifications for that task
                        foreach (TaskNotifcation taskNot in childTask.TaskNotifcations.Where(tn => tn.TaskID == childTask.ID))
                        {
                            //Delete each notification
                            nHelper.RemoveReadyToDeleteNotification(taskNot.ID, "task");
                        }
                        //Delete the task
                        db.Tasks.Remove(childTask);
                    }

                    #endregion
                    foreach (WUserSubproject userSub in db.WUserSubprojects.Where(sp => sp.SubprojectID == SubProjectID).ToList())
                    {
                        db.WUserSubprojects.Remove(userSub);
                    }
                    db.SubProjects.Remove(subProj);
                    db.SaveChanges();
                    Success = true;
                }
                catch (DbUpdateException)
                {
                    ctx.Refresh(RefreshMode.ClientWins, db.TaskNotifcations);
                    ctx.Refresh(RefreshMode.ClientWins, db.Tasks);
                    ctx.Refresh(RefreshMode.ClientWins, db.WUserSubprojects);
                    db.SaveChanges();
                    Success = true;
                }

                catch (DBConcurrencyException)
                {
                    ctx.Refresh(RefreshMode.ClientWins, db.TaskNotifcations);
                    ctx.Refresh(RefreshMode.ClientWins, db.Tasks);
                    db.SaveChanges();
                    Success = true;
                }
                catch (OptimisticConcurrencyException)
                {
                    ctx.Refresh(RefreshMode.ClientWins, db.TaskNotifcations);
                    ctx.Refresh(RefreshMode.ClientWins, db.Tasks);
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
        public JsonResult Rename(string NewTitle, Guid SubProjectID)
        {
            bool Success = false;

            using (warpspeedEntities db = new warpspeedEntities())
            {
                SubProject subProj = db.SubProjects.First(p => p.ID == SubProjectID);
                subProj.Title = NewTitle;
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
        public JsonResult RemoveUserFromSubProject(Guid UserID, Guid SubProjectID)
        {
            bool Success = false;
            using (warpspeedEntities db = new warpspeedEntities())
            {
                try
                {
                    if (db.WUserSubprojects.Any(usp => usp.SubprojectID == SubProjectID && usp.WUserID == UserID))
                    {
                        WUserSubproject userSubProject = db.WUserSubprojects.First(usp => usp.SubprojectID == SubProjectID && usp.WUserID == UserID);
                        db.WUserSubprojects.Remove(userSubProject);
                        db.SaveChanges();
                        Success = true;
                    }
                    else
                    {
                        //No WUser subprojects that have this user
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return Json(Success);
        }
    }
}
