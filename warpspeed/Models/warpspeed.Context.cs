﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class warpspeedEntities : DbContext
    {
        public warpspeedEntities()
            : base("name=warpspeedEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Group> Groups { get; set; }
        public DbSet<NotificationSeen> NotificationSeens { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectNotification> ProjectNotifications { get; set; }
        public DbSet<SubProject> SubProjects { get; set; }
        public DbSet<SubProjectNotifcation> SubProjectNotifcations { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskNotifcation> TaskNotifcations { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<WUser> WUsers { get; set; }
        public DbSet<WUserProject> WUserProjects { get; set; }
        public DbSet<WUserSubproject> WUserSubprojects { get; set; }
    }
}
