namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddJobFieldToLessonTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lesson", "JobId", c => c.Int(nullable: false));
            CreateIndex("dbo.Lesson", "JobId");
            AddForeignKey("dbo.Lesson", "JobId", "dbo.Job", "JobId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lesson", "JobId", "dbo.Job");
            DropIndex("dbo.Lesson", new[] { "JobId" });
            DropColumn("dbo.Lesson", "JobId");
        }
    }
}
