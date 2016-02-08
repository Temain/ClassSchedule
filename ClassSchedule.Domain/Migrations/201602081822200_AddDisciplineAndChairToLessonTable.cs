namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDisciplineAndChairToLessonTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lesson", "ChairId", c => c.Int(nullable: false));
            AddColumn("dbo.Lesson", "DisciplineId", c => c.Int(nullable: false));
            CreateIndex("dbo.Lesson", "ChairId");
            CreateIndex("dbo.Lesson", "DisciplineId");
            AddForeignKey("dbo.Lesson", "ChairId", "dbo.Chair", "ChairId");
            AddForeignKey("dbo.Lesson", "DisciplineId", "dict.Discipline", "DisciplineId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lesson", "DisciplineId", "dict.Discipline");
            DropForeignKey("dbo.Lesson", "ChairId", "dbo.Chair");
            DropIndex("dbo.Lesson", new[] { "DisciplineId" });
            DropIndex("dbo.Lesson", new[] { "ChairId" });
            DropColumn("dbo.Lesson", "DisciplineId");
            DropColumn("dbo.Lesson", "ChairId");
        }
    }
}
