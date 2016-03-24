namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeChairIdNotRequiredInPlan : DbMigration
    {
        public override void Up()
        {
            DropIndex("plan.AcademicPlan", new[] { "ChairId" });
            AlterColumn("plan.AcademicPlan", "ChairId", c => c.Int());
            AlterColumn("plan.AcademicPlan", "NumberOfSemesters", c => c.Int(nullable: false));
            CreateIndex("plan.AcademicPlan", "ChairId");
        }
        
        public override void Down()
        {
            DropIndex("plan.AcademicPlan", new[] { "ChairId" });
            AlterColumn("plan.AcademicPlan", "NumberOfSemesters", c => c.String());
            AlterColumn("plan.AcademicPlan", "ChairId", c => c.Int(nullable: false));
            CreateIndex("plan.AcademicPlan", "ChairId");
        }
    }
}
