namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCourseAndGroupFieldToApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CourseId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "GroupId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "CourseId");
            CreateIndex("dbo.AspNetUsers", "GroupId");
            AddForeignKey("dbo.AspNetUsers", "CourseId", "dbo.Course", "CourseId");
            AddForeignKey("dbo.AspNetUsers", "GroupId", "dict.Group", "GroupId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "GroupId", "dict.Group");
            DropForeignKey("dbo.AspNetUsers", "CourseId", "dbo.Course");
            DropIndex("dbo.AspNetUsers", new[] { "GroupId" });
            DropIndex("dbo.AspNetUsers", new[] { "CourseId" });
            DropColumn("dbo.AspNetUsers", "GroupId");
            DropColumn("dbo.AspNetUsers", "CourseId");
        }
    }
}
