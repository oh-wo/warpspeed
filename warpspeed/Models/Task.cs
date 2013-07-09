//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace warpspeed.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Task
    {
        public Task()
        {
            this.TaskNotifcations = new HashSet<TaskNotifcation>();
            this.Task1 = new HashSet<Task>();
        }
    
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> SubProjectID { get; set; }
        public string Title { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.Guid> CreatedID { get; set; }
        public Nullable<System.Guid> TargetUserID { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.Guid> ParentTaskID { get; set; }
    
        public virtual SubProject SubProject { get; set; }
        public virtual WUser WUser { get; set; }
        public virtual ICollection<TaskNotifcation> TaskNotifcations { get; set; }
        public virtual ICollection<Task> Task1 { get; set; }
        public virtual Task Task2 { get; set; }
    }
}
