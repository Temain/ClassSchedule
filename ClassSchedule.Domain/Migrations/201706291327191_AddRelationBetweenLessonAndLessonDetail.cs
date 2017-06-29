namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRelationBetweenLessonAndLessonDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LessonDetail", "LessonId", c => c.Int(nullable: false));
            CreateIndex("dbo.LessonDetail", "LessonId");
            AddForeignKey("dbo.LessonDetail", "LessonId", "dbo.Lesson", "LessonId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LessonDetail", "LessonId", "dbo.Lesson");
            DropIndex("dbo.LessonDetail", new[] { "LessonId" });
            DropColumn("dbo.LessonDetail", "LessonId");
        }
    }
}
