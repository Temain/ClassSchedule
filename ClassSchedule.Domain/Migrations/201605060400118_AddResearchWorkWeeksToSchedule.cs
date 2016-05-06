namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResearchWorkWeeksToSchedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("plan.CourseSchedule", "ResearchWorkWeeks", c => c.Int(nullable: false));
            AddColumn("plan.SemesterSchedule", "ResearchWorkWeeks", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("plan.SemesterSchedule", "ResearchWorkWeeks");
            DropColumn("plan.CourseSchedule", "ResearchWorkWeeks");
        }
    }
}
