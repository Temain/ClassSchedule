namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeAuditoriumIdInLessonTableRequired : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Lesson", new[] { "AuditoriumId" });
            AlterColumn("dbo.Lesson", "AuditoriumId", c => c.Int(nullable: false));
            CreateIndex("dbo.Lesson", "AuditoriumId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Lesson", new[] { "AuditoriumId" });
            AlterColumn("dbo.Lesson", "AuditoriumId", c => c.Int());
            CreateIndex("dbo.Lesson", "AuditoriumId");
        }
    }
}
