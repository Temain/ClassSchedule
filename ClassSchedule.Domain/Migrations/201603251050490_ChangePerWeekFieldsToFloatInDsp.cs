namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePerWeekFieldsToFloatInDsp : DbMigration
    {
        public override void Up()
        {
            AlterColumn("plan.DisciplineSemesterPlan", "LecturesPerWeek", c => c.Single());
            AlterColumn("plan.DisciplineSemesterPlan", "PracticePerWeek", c => c.Single());
            AlterColumn("plan.DisciplineSemesterPlan", "LaboratoryPerWeek", c => c.Single());
        }
        
        public override void Down()
        {
            AlterColumn("plan.DisciplineSemesterPlan", "LaboratoryPerWeek", c => c.Int(nullable: false));
            AlterColumn("plan.DisciplineSemesterPlan", "PracticePerWeek", c => c.Int(nullable: false));
            AlterColumn("plan.DisciplineSemesterPlan", "LecturesPerWeek", c => c.Int(nullable: false));
        }
    }
}
