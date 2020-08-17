using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AppRaisal.Data.Enitities;
using Constants = AppRaisal.Data;
using System.Reflection.Metadata;
using AppRaisal.Models;

namespace AppRaisal.Data
{
    public class ApplicationContext : DbContext
    {
        User[] users = new User[]
        {
            new User { Username=new AppraisalsModel().GetCurrentUserName(), FullName="Current user" }
        };

        AppraisalPeriod[] periods = new AppraisalPeriod[]
        {
            new AppraisalPeriod { Id=1, Name="01/01/2020 - 01/06/2020", Status="Active" },
            new AppraisalPeriod { Id=4, Name="01/01/2020 - 01/06/2020", Status="Active" },
            new AppraisalPeriod { Id=5, Name="01/01/2020 - 01/06/2020", Status="Active" },
            new AppraisalPeriod { Id=6, Name="01/01/2020 - 01/06/2020", Status="Active" },
            new AppraisalPeriod { Id=7, Name="01/01/2020 - 01/06/2020", Status="Active" },
            new AppraisalPeriod { Id=8, Name="01/01/2020 - 01/06/2020", Status="Finished" },
            new AppraisalPeriod { Id=2, Name="01/06/2019 - 01/01/2020", Status="Finished" },
            new AppraisalPeriod { Id=3, Name="Test", Status="Finished" },
        };

        AppraisalItem[] items = new AppraisalItem[]
        {
            new AppraisalItem { Id=1, Content="Item 1", Status=ItemStatus.InProgress.ToString(), Type=ItemType.Achieved.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=2, Content="Item 2", Status=ItemStatus.Active.ToString(), Type=ItemType.Planned.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=3, Content="Item 3", Status=ItemStatus.Active.ToString(), Type=ItemType.Planned.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=4, Content="Item 4", Status=ItemStatus.Active.ToString(), Type=ItemType.Achieved.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=5, Content="Item 1", Status=ItemStatus.Active.ToString(), Type=ItemType.Achieved.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=6, Content="Item 2", Status=ItemStatus.Active.ToString(), Type=ItemType.Achieved.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=7, Content="Item 3", Status=ItemStatus.Active.ToString(), Type=ItemType.Achieved.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=8, Content="Item 4", Status=ItemStatus.Active.ToString(), Type=ItemType.Planned.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=9, Content="Item 1", Status=ItemStatus.Active.ToString(), Type=ItemType.Planned.ToString(), AppraisalPeriodId=4, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=10, Content="Item 1", Status=ItemStatus.Active.ToString(), Type=ItemType.Planned.ToString(), AppraisalPeriodId=5, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=11, Content="Item 1", Status=ItemStatus.Active.ToString(), Type=ItemType.Planned.ToString(), AppraisalPeriodId=6, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=12, Content="Item 1", Status=ItemStatus.Active.ToString(), Type=ItemType.Planned.ToString(), AppraisalPeriodId=7, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=13, Content="Item 1", Status=ItemStatus.Active.ToString(), Type=ItemType.Planned.ToString(), AppraisalPeriodId=8, UserName=new AppraisalsModel().GetCurrentUserName() },
            // Trainings
            new AppraisalItem { Id=14, Content="XML", Status=ItemStatus.Active.ToString(), Type=ItemType.Training.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=15, Content="Customs", Status=ItemStatus.Active.ToString(), Type=ItemType.Training.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=16, Content="Go Fuck yourself", Status=ItemStatus.Active.ToString(), Type=ItemType.Training_Suggested.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=17, Content="Asshole", Status=ItemStatus.Active.ToString(), Type=ItemType.Training_Suggested.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            // SWOT
            new AppraisalItem { Id=18, Content="Stronk", Status=ItemStatus.Active.ToString(), Type=ItemType.SWOT_Strength.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=19, Content="Stress bullshit", Status=ItemStatus.Active.ToString(), Type=ItemType.SWOT_Strength.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=20, Content="Asshole", Status=ItemStatus.Active.ToString(), Type=ItemType.SWOT_Weakness.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=21, Content="Opportunity test", Status=ItemStatus.Active.ToString(), Type=ItemType.SWOT_Opportunity.ToString(), AppraisalPeriodId=1, UserName=new AppraisalsModel().GetCurrentUserName() },
            new AppraisalItem { Id=22, Content="Planned from last appraisal", Status=ItemStatus.InProgress.ToString(), Type=ItemType.Planned.ToString(), AppraisalPeriodId=2, UserName=new AppraisalsModel().GetCurrentUserName() },
            // Test period
            new AppraisalItem { Id=23, Content="Item 1", Status=ItemStatus.Active.ToString(), Type=ItemType.Planned.ToString(), AppraisalPeriodId=3, UserName=new AppraisalsModel().GetCurrentUserName() },
        };

        public DbSet<User> Users { get; set; }
        public DbSet<AppraisalPeriod> AppraisalPeriods { get; set; }
        public DbSet<AppraisalItem> AppraisalItems { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void  OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            var connectionString = $"Filename={ConfigurationManager.AppSettings.Get("Database")}";
            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppraisalItem>()
                .HasOne(i => i.User)
                .WithMany(u => u.Items)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppraisalItem>()
                .HasOne(i => i.AppraisalPeriod)
                .WithMany(p => p.Items)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppraisalItem>()
                .HasIndex(i => i.RelatedId);

            modelBuilder.Entity<User>()
                .HasData(users);
            modelBuilder.Entity<AppraisalPeriod>()
                .HasData(periods);
            modelBuilder.Entity<AppraisalItem>()
                .HasData(items);

        }

    }
}
