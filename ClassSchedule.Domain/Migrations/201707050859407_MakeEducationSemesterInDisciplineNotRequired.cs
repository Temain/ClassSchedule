namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeEducationSemesterInDisciplineNotRequired : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Discipline", new[] { "EducationSemesterId" });
            AlterColumn("dbo.Discipline", "EducationSemesterId", c => c.Int());
            CreateIndex("dbo.Discipline", "EducationSemesterId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Discipline", new[] { "EducationSemesterId" });
            AlterColumn("dbo.Discipline", "EducationSemesterId", c => c.Int(nullable: false));
            CreateIndex("dbo.Discipline", "EducationSemesterId");
        }
    }
}
