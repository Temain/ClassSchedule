namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScheduleFieldToSemesterAndCoursePlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("plan.CourseSchedule", "Schedule", c => c.String());
            AddColumn("plan.SemesterSchedule", "Schedule", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("plan.SemesterSchedule", "Schedule");
            DropColumn("plan.CourseSchedule", "Schedule");
        }
    }
}
