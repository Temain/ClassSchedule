namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCourseIdAndGroupIdFromUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "CourseId", "dbo.Course");
            DropForeignKey("dbo.AspNetUsers", "GroupId", "dict.Group");
            DropIndex("dbo.AspNetUsers", new[] { "CourseId" });
            DropIndex("dbo.AspNetUsers", new[] { "GroupId" });
            DropColumn("dbo.AspNetUsers", "CourseId");
            DropColumn("dbo.AspNetUsers", "GroupId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "GroupId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "CourseId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "GroupId");
            CreateIndex("dbo.AspNetUsers", "CourseId");
            AddForeignKey("dbo.AspNetUsers", "GroupId", "dict.Group", "GroupId");
            AddForeignKey("dbo.AspNetUsers", "CourseId", "dbo.Course", "CourseId");
        }
    }
}
