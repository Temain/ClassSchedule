namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEducationYearToLessonTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lesson", "EducationYearId", c => c.Int());
            CreateIndex("dbo.Lesson", "EducationYearId");
            AddForeignKey("dbo.Lesson", "EducationYearId", "dict.EducationYear", "EducationYearId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lesson", "EducationYearId", "dict.EducationYear");
            DropIndex("dbo.Lesson", new[] { "EducationYearId" });
            DropColumn("dbo.Lesson", "EducationYearId");
        }
    }
}
