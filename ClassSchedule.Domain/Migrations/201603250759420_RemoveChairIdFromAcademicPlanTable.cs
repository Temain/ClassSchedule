namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveChairIdFromAcademicPlanTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("plan.AcademicPlan", "ChairId", "dbo.Chair");
            DropIndex("plan.AcademicPlan", new[] { "ChairId" });
            DropColumn("plan.AcademicPlan", "ChairId");
        }
        
        public override void Down()
        {
            AddColumn("plan.AcademicPlan", "ChairId", c => c.Int());
            CreateIndex("plan.AcademicPlan", "ChairId");
            AddForeignKey("plan.AcademicPlan", "ChairId", "dbo.Chair", "ChairId");
        }
    }
}
