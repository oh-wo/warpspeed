using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using warpspeed.Models;
using warpspeed.Helpers;
using System.Data;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace warpspeed.Controllers
{
    public class TaskController : Controller
    {
        public class CreateTaskClass
        {
            public string Title {get;set;}
            public Guid SubProjectID { get; set; }
            public Guid ParentTaskID { get; set; }
            public bool HasParent { get; set; }
        }
        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult Create(CreateTaskClass clientTask)
        {
            bool Success = false;
            Task task = new Task();
            using (warpspeedEntities db = new warpspeedEntities())
            {
                WUser user = db.WUsers.First(u => u.ID == new Guid(User.Identity.Name));

                task.ID = Guid.NewGuid();
                task.Title = clientTask.Title;
                task.CreatedID = user.ID;
                task.WUser = user;
                task.SubProjectID = clientTask.SubProjectID;
                task.Created = DateTime.UtcNow;
                task.Status = 0;
                if(clientTask.HasParent){
                    task.ParentTaskID = clientTask.ParentTaskID;
                }
                //Task
                db.Tasks.Add(task);
                //Notification
                NotificationHelper nHelper = new NotificationHelper();  
                try
                {
                    db.SaveChanges();
                    nHelper.CreateNotification(user.ID, task.ID, "task", 1);
                    Success = true;
                }
                catch (Exception ex)
                {

                }
            }
            return Json(Success?task.ID.ToString():String.Format("null"));
        }

        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult Delete(Guid TaskID, Guid SubProjectID)
        {
            bool Success = false;
            NotificationHelper nHelper = new NotificationHelper();
            using (warpspeedEntities db = new warpspeedEntities())
            {
                var ctx = ((IObjectContextAdapter)db).ObjectContext;
                try
                {
                    List<Task> tasks = db.Tasks.Where(t => t.SubProjectID == SubProjectID).ToList();
                    Task originalTask = tasks.First(t => t.ID == TaskID);
                    List<Task> childTasks = TaskHelper.ListAllChildTasks(tasks.First(t => t.ID == TaskID), tasks.Where(t => t.ID != TaskID).ToList());
                    foreach (Task childTask in childTasks.ToList())
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

                    //Delete the notifications for the original task

                    foreach (TaskNotifcation taskNot in db.TaskNotifcations.Where(tn => tn.TaskID == TaskID))
                    {
                        //Delete each notification
                        nHelper.RemoveReadyToDeleteNotification(taskNot.ID, "task");
                    }
                    //Percent complete
                    ProgressHelper progHelper = new ProgressHelper();
                    originalTask.SubProject.PercentComplete = progHelper.PrepareToUpdateProgressOfSubProject(originalTask.SubProject);
                    originalTask.SubProject.Project.PercentComplete = progHelper.PrepareToUpdateProgressOfProject(originalTask.SubProject.Project);
                    //Delete the original task
                    db.Tasks.Remove(originalTask);
                    
                    db.SaveChanges();
                    Success = true;
                }/*
                catch (DbUpdateConcurrencyException)
                {
                    ctx.Refresh(RefreshMode.ClientWins, db.TaskNotifcations);
                    ctx.Refresh(RefreshMode.ClientWins, db.Tasks);
                    db.SaveChanges();
                    Success = true;
                }*/
                catch (DbUpdateException)
                {
                    ctx.Refresh(RefreshMode.ClientWins, db.TaskNotifcations);
                    ctx.Refresh(RefreshMode.ClientWins, db.Tasks);
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
            }
            return Json(Success);
        }
            
        

        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult Edit(Guid TaskID,string Title)
        {
            bool Success = false;
            using (warpspeedEntities db = new warpspeedEntities())
            {
                try
                {
                    Task task = db.Tasks.First(t => t.ID == TaskID);
                    task.Title = Title;
                    db.SaveChanges();
                    NotificationHelper nHelper = new NotificationHelper();
                    nHelper.CreateNotification(new Guid(User.Identity.Name), task.ID, "task", 4);
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
        public JsonResult ToggleComplete(Guid TaskID, int Status)
        {
            bool Success = false;
            using (warpspeedEntities db = new warpspeedEntities())
            {
                try
                {
                    Task task = db.Tasks.First(t => t.ID == TaskID);
                    task.Status = Status;
                    //Percent complete
                    ProgressHelper progHelper = new ProgressHelper();
                    task.SubProject.PercentComplete = progHelper.PrepareToUpdateProgressOfSubProject(task.SubProject);
                    task.SubProject.Project.PercentComplete = progHelper.PrepareToUpdateProgressOfProject(task.SubProject.Project);
                    db.SaveChanges();
                    Success = true;
                }
                catch (Exception ex)
                {

                }
            }
            return Json(Success);
        }
    }
}
