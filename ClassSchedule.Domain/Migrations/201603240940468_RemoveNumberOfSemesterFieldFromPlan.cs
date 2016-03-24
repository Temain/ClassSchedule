namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveNumberOfSemesterFieldFromPlan : DbMigration
    {
        public override void Up()
        {
            DropColumn("plan.AcademicPlan", "NumberOfSemesters");
        }
        
        public override void Down()
        {
            AddColumn("plan.AcademicPlan", "NumberOfSemesters", c => c.Int(nullable: false));
        }
    }
}
