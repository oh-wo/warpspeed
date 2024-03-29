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
    
    public partial class ProjectNotification
    {
        public ProjectNotification()
        {
            this.NotificationSeens = new HashSet<NotificationSeen>();
        }
    
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ProjectID { get; set; }
        public Nullable<System.Guid> WUserID { get; set; }
        public Nullable<int> TypeID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
    
        public virtual ICollection<NotificationSeen> NotificationSeens { get; set; }
        public virtual NotificationType NotificationType { get; set; }
        public virtual Project Project { get; set; }
        public virtual WUser WUser { get; set; }
    }
}
