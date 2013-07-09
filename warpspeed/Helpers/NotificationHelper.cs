using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using warpspeed.Models;

namespace warpspeed.Helpers
{
    public class NotificationHelper
    {

        public bool CreateNotification(Guid UserID, Guid ElementID,string ElementType,int TypeID)
        {
            using (warpspeedEntities db = new warpspeedEntities())
            {
                switch (ElementType)
                {
                    case "project":
                        ProjectNotification notification = new ProjectNotification()
                        {
                            ID = Guid.NewGuid(),
                            WUserID = UserID,
                            ProjectID = ElementID,
                            TypeID = TypeID,
                            Created = DateTime.UtcNow,
                        };
                        db.ProjectNotifications.Add(notification);
                        break;
                    case "subproject":
                        SubProjectNotifcation subNot = new SubProjectNotifcation()
                        {
                            ID = Guid.NewGuid(),
                            WUserID = UserID,
                            SubProjectID = ElementID,
                            TypeID = TypeID,
                            Created = DateTime.UtcNow,
                        };
                        db.SubProjectNotifcations.Add(subNot);
                        break;
                    case "task":
                        TaskNotifcation taskNot = new TaskNotifcation()
                        {
                            ID = Guid.NewGuid(),
                            WUserID = UserID,
                            TaskID = ElementID,
                            TypeID = TypeID,
                            Created = DateTime.UtcNow,
                        };
                        db.TaskNotifcations.Add(taskNot);
                        break;
                }
                db.SaveChanges();
            
            }
            return true;
        }

        public bool RemoveReadyToDeleteNotification(Guid notificationID, string ElementType)
        {
            bool Success = false;
            using (warpspeedEntities db = new warpspeedEntities())
            {
                try
                {
                    switch (ElementType)
                    {
                        case "project":
                            if (db.ProjectNotifications.Any(pn => pn.ID == notificationID))
                            {
                                db.ProjectNotifications.Remove(db.ProjectNotifications.First(pn => pn.ID == notificationID));
                            }
                            break;
                        case "subproject":
                            if (db.SubProjectNotifcations.Any(pn => pn.ID == notificationID))
                            {
                                db.SubProjectNotifcations.Remove(db.SubProjectNotifcations.First(pn => pn.ID == notificationID));
                            }
                            break;
                        case "task":
                            if (db.TaskNotifcations.Any(pn => pn.ID == notificationID))
                            {
                                //Get any notification seens and delete them
                                foreach (NotificationSeen notSeen in db.NotificationSeens.Where(ns => ns.TaskNotificationID == notificationID).ToList())
                                {
                                    db.NotificationSeens.Remove(notSeen);
                                }
                                db.TaskNotifcations.Remove(db.TaskNotifcations.First(pn => pn.ID == notificationID));
                            }

                            break;
                    }
                    //db.SaveChanges();
                    Success = true;
                }
                catch (Exception ex)
                {

                }
            }
            return Success;
        }
        public bool NotificationsSeen(Guid ElementID, string ElementType)
        {
            Guid UserID = new Guid(HttpContext.Current.User.Identity.Name);
            bool Success = false;
            using (warpspeedEntities db = new warpspeedEntities())
            {
                try
                {
                    switch (ElementType)
                    {
                        case "project":
                            //This won't work yet
                            if (db.ProjectNotifications.Any(pn => pn.ProjectID == ElementID && !pn.NotificationSeens.Any(ns => ns.WUserID == UserID)))
                            {
                                IQueryable projectNotications = db.ProjectNotifications.Where(pn => pn.ProjectID == ElementID && !pn.NotificationSeens.Any(ns => ns.WUserID == UserID));
                                foreach (ProjectNotification projNot in projectNotications)
                                {
                                    NotificationSeen notificationSeen = new Models.NotificationSeen()
                                    {
                                        ID = Guid.NewGuid(),
                                        WUserID = UserID,
                                        ProjectNotificationID = projNot.ID,
                                    };
                                    db.NotificationSeens.Add(notificationSeen);
                                }
                            }
                            break;
                        case "subproject":
                            if (db.SubProjectNotifcations.Any(pn => pn.SubProject.ProjectID == ElementID && !pn.NotificationSeens.Any(ns => ns.WUserID == UserID)))
                            {
                                IQueryable subProjectNotications = db.SubProjectNotifcations.Where(pn => pn.SubProject.ProjectID == ElementID && !pn.NotificationSeens.Any(ns => ns.WUserID == UserID));
                                foreach (SubProjectNotifcation projNot in subProjectNotications)
                                {
                                    NotificationSeen notificationSeen = new Models.NotificationSeen()
                                    {
                                        ID = Guid.NewGuid(),
                                        WUserID = UserID,
                                        SubProjectNotificationID = projNot.ID,
                                    };
                                    db.NotificationSeens.Add(notificationSeen);
                                }
                            }
                            break;
                        case "task":
                            if (db.TaskNotifcations.Any(pn => pn.Task.SubProjectID == ElementID && !pn.NotificationSeens.Any(ns => ns.WUserID == UserID)))
                            {
                                IQueryable taskNotications = db.TaskNotifcations.Where(pn => pn.Task.SubProjectID == ElementID && !pn.NotificationSeens.Any(ns => ns.WUserID == UserID));
                                foreach (TaskNotifcation taskNot in taskNotications)
                                {
                                    NotificationSeen notificationSeen = new Models.NotificationSeen()
                                    {
                                        ID = Guid.NewGuid(),
                                        WUserID = UserID,
                                        TaskNotificationID = taskNot.ID,
                                        
                                    };
                                    db.NotificationSeens.Add(notificationSeen);
                                }
                            }
                            break;
                    }
                    db.SaveChanges();
                    Success = true;
                }
                catch (Exception ex)
                {

                }
            }
            return Success;
        }
    }
}