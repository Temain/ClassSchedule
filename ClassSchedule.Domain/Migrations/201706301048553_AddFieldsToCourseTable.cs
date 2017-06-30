namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsToCourseTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Course", "CourseNamePrefix", c => c.String());
            AddColumn("dbo.Course", "CourseNameSuffix", c => c.String());
            AddColumn("dbo.Course", "IsIntensive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Course", "IsIntensive");
            DropColumn("dbo.Course", "CourseNameSuffix");
            DropColumn("dbo.Course", "CourseNamePrefix");
        }
    }
}
