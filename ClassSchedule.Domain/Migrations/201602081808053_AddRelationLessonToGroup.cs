namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRelationLessonToGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lesson", "DayNumber", c => c.Int(nullable: false));
            AddColumn("dbo.Lesson", "GroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Lesson", "GroupId");
            AddForeignKey("dbo.Lesson", "GroupId", "dict.Group", "GroupId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lesson", "GroupId", "dict.Group");
            DropIndex("dbo.Lesson", new[] { "GroupId" });
            DropColumn("dbo.Lesson", "GroupId");
            DropColumn("dbo.Lesson", "DayNumber");
        }
    }
}
