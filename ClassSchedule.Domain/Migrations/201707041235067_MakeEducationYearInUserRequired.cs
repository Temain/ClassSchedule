namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeEducationYearInUserRequired : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUsers", new[] { "EducationYearId" });
            AlterColumn("dbo.AspNetUsers", "EducationYearId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "EducationYearId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", new[] { "EducationYearId" });
            AlterColumn("dbo.AspNetUsers", "EducationYearId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "EducationYearId");
        }
    }
}
