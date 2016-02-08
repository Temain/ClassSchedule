namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateRelations : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "dbo.SemesterSchedule", newSchema: "acpl");
            AddColumn("acpl.DisciplineSemesterPlan", "SemesterScheduleId", c => c.Int(nullable: false));
            CreateIndex("acpl.DisciplineSemesterPlan", "SemesterScheduleId");
            AddForeignKey("acpl.DisciplineSemesterPlan", "SemesterScheduleId", "acpl.SemesterSchedule", "SemesterScheduleId");
        }
        
        public override void Down()
        {
            DropForeignKey("acpl.DisciplineSemesterPlan", "SemesterScheduleId", "acpl.SemesterSchedule");
            DropIndex("acpl.DisciplineSemesterPlan", new[] { "SemesterScheduleId" });
            DropColumn("acpl.DisciplineSemesterPlan", "SemesterScheduleId");
            MoveTable(name: "acpl.SemesterSchedule", newSchema: "dbo");
        }
    }
}
