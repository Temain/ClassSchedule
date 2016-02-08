namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemakeManyToManyDisciplineSemesterPlanJob : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "acpl.AcademicPlan", newSchema: "plan");
            MoveTable(name: "acpl.DisciplineSemesterPlan", newSchema: "plan");
            MoveTable(name: "acpl.DisciplineWeekPlan", newSchema: "plan");
            MoveTable(name: "acpl.SemesterSchedule", newSchema: "plan");
            MoveTable(name: "acpl.CourseSchedule", newSchema: "plan");
            RenameColumn(table: "dbo.DisciplineSemesterPlanJob", name: "DisciplineSemesterPlan_DisciplineSemesterPlanId", newName: "DisciplineSemesterPlanId");
            RenameColumn(table: "dbo.DisciplineSemesterPlanJob", name: "Job_JobId", newName: "JobId");
            RenameIndex(table: "dbo.DisciplineSemesterPlanJob", name: "IX_DisciplineSemesterPlan_DisciplineSemesterPlanId", newName: "IX_DisciplineSemesterPlanId");
            RenameIndex(table: "dbo.DisciplineSemesterPlanJob", name: "IX_Job_JobId", newName: "IX_JobId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.DisciplineSemesterPlanJob", name: "IX_JobId", newName: "IX_Job_JobId");
            RenameIndex(table: "dbo.DisciplineSemesterPlanJob", name: "IX_DisciplineSemesterPlanId", newName: "IX_DisciplineSemesterPlan_DisciplineSemesterPlanId");
            RenameColumn(table: "dbo.DisciplineSemesterPlanJob", name: "JobId", newName: "Job_JobId");
            RenameColumn(table: "dbo.DisciplineSemesterPlanJob", name: "DisciplineSemesterPlanId", newName: "DisciplineSemesterPlan_DisciplineSemesterPlanId");
            MoveTable(name: "plan.CourseSchedule", newSchema: "acpl");
            MoveTable(name: "plan.SemesterSchedule", newSchema: "acpl");
            MoveTable(name: "plan.DisciplineWeekPlan", newSchema: "acpl");
            MoveTable(name: "plan.DisciplineSemesterPlan", newSchema: "acpl");
            MoveTable(name: "plan.AcademicPlan", newSchema: "acpl");
        }
    }
}
