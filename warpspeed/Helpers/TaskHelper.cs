using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using warpspeed.Models;

namespace warpspeed.Helpers
{
    public class TaskHelper
    {
        public static string GetChildren(Task task, IEnumerable<Task> tasks, IEnumerable<WUserSubproject> WUserSubprojects, int depth)
        {
            string taskStatus = "";
            string taskCompleteClass = "";
            switch (task.Status)
            {
                case null:
                    taskStatus = "sp-task-status-incomplete";
                    break;
                case 0:
                    taskStatus = "sp-task-status-incomplete";
                    break;
                case 1:
                    taskStatus = "sp-task-status-complete";
                    taskCompleteClass = "complete";
                    break;
                case 2:
                    taskStatus = "sp-task-status-active";
                    break;

            }
            string TaskString = String.Format("<div class=\"sp-task-holder\" tid=\"{0}\"><div class=\"sp-task\" depth=\"{1}\">", task.ID, depth);
            TaskString += String.Format("<div class=\"sp-task-status {0}\" title=\"status\"></div>", taskStatus);
            TaskString += "<div class=\"sp-task-createSubTask\" title=\"create sub task\">+</div>";
            TaskString += String.Format("<input type=\"text\" class=\"sp-task-value {0}\" value=\"{1}\"/>", taskCompleteClass, task.Title);
            TaskString += "<div class=\"sp-task-remove\" title=\"delete task\">x</div>";

            TaskString += "<select class=\"sp-task-target\" title=\"Who is the task for?\">";
            string selected = "";
            foreach (WUserSubproject userSubProject in WUserSubprojects)
            {

                if (task.TargetUserID == userSubProject.WUserID)
                {
                    selected = "selected='selected'";
                }
                TaskString += String.Format("<option value='{0}' {1}>{2}</option>", userSubProject.WUserID, selected, userSubProject.WUser.FirstName);
            }

            TaskString += "</select>";
            TaskString += "<div class=\"sp-task-completedBy\"><span class=\"sp-task-completedBy-value\"></span></div>";
            string startDate = ""; string endDate = "";
            if (task.StartDate != null)
            {
                startDate = String.Format("pageLoadValue=\"{0}\"", task.StartDate.Value.ToLongDateString());
            }


            if (task.EndDate != null) { endDate += String.Format("pageLoadValue=\"{0}\"", task.EndDate.Value.ToLongDateString()); }

            TaskString += String.Format("<input type=\"text\" class=\"sp-startDate sp-date\" value=\"{0}\" readonly=\"readonly\" title=\"start date\"/>", startDate);
            TaskString += String.Format("<input type=\"text\" pageLoadValue=\"\" class=\"sp-endDate sp-date\" value=\"{0}\" readonly=\"readonly\" title=\"end date\"/>", endDate);
            TaskString += String.Format("</div><div class=\"sp-task-subTasks\" parentID=\"{0}\">", task.ID);
            depth++;
            foreach (Task ta in tasks.Where(t => t.ParentTaskID == task.ID))
            {
                TaskString += String.Format("{0}", GetChildren(ta, tasks.Where(t => t.ID != task.ID), WUserSubprojects, depth));//remove the current comment from the list to speed things up
            }
            TaskString += "</div></div>";
            return TaskString;
        }

        public static List<Task> ListAllChildTasks(Task Task, List<Task> tasks)
        {
            List<Task> childTasks = new List<Task>();
            try
            {
                childTasks = tasks.Where(t => t.ParentTaskID == Task.ID).ToList();
                foreach (Task task in childTasks.ToList())
                {
                    childTasks.AddRange(ListAllChildTasks(task, tasks));
                }
            }
            catch (Exception ex)
            {

            }
            return childTasks;
        }
    }
}