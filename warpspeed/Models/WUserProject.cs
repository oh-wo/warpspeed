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
    
    public partial class WUserProject
    {
        public System.Guid WUserID { get; set; }
        public System.Guid ProjectID { get; set; }
        public Nullable<System.Guid> PrivilegeID { get; set; }
    
        public virtual Privilege Privilege { get; set; }
        public virtual Project Project { get; set; }
        public virtual WUser WUser { get; set; }
    }
}
