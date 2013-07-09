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
    
    public partial class Project
    {
        public Project()
        {
            this.ProjectNotifications = new HashSet<ProjectNotification>();
            this.SubProjects = new HashSet<SubProject>();
            this.WUserProjects = new HashSet<WUserProject>();
        }
    
        public System.Guid ID { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> PercentComplete { get; set; }
        public string Title { get; set; }
        public Nullable<System.Guid> CreatedByID { get; set; }
        public Nullable<bool> Deleted { get; set; }
    
        public virtual WUser WUser { get; set; }
        public virtual ICollection<ProjectNotification> ProjectNotifications { get; set; }
        public virtual ICollection<SubProject> SubProjects { get; set; }
        public virtual ICollection<WUserProject> WUserProjects { get; set; }
    }
}
