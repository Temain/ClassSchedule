namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveChairIdFromLessonTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Lesson", "ChairId", "dbo.Chair");
            DropIndex("dbo.Lesson", new[] { "ChairId" });
            DropColumn("dbo.Lesson", "ChairId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lesson", "ChairId", c => c.Int(nullable: false));
            CreateIndex("dbo.Lesson", "ChairId");
            AddForeignKey("dbo.Lesson", "ChairId", "dbo.Chair", "ChairId");
        }
    }
}
