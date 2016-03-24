namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHoursPerWeekFieldsToDisciplineSemesterPlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("plan.DisciplineSemesterPlan", "LecturesPerWeek", c => c.Int(nullable: false));
            AddColumn("plan.DisciplineSemesterPlan", "PracticePerWeek", c => c.Int(nullable: false));
            AddColumn("plan.DisciplineSemesterPlan", "LaboratoryPerWeek", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("plan.DisciplineSemesterPlan", "LaboratoryPerWeek");
            DropColumn("plan.DisciplineSemesterPlan", "PracticePerWeek");
            DropColumn("plan.DisciplineSemesterPlan", "LecturesPerWeek");
        }
    }
}
