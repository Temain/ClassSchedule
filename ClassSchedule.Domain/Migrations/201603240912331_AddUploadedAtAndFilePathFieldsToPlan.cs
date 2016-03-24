namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUploadedAtAndFilePathFieldsToPlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("plan.AcademicPlan", "UploadedAt", c => c.DateTime(nullable: false));
            AddColumn("plan.AcademicPlan", "FilePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("plan.AcademicPlan", "FilePath");
            DropColumn("plan.AcademicPlan", "UploadedAt");
        }
    }
}
