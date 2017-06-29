namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeWeekNumberRequiredInUserProfile : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "WeekNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "WeekNumber", c => c.Int());
        }
    }
}
