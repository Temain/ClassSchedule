namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManyToManyJobsDisciplineSemesterPlans : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DisciplineSemesterPlanJob",
                c => new
                    {
                        DisciplineSemesterPlan_DisciplineSemesterPlanId = c.Int(nullable: false),
                        Job_JobId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DisciplineSemesterPlan_DisciplineSemesterPlanId, t.Job_JobId })
                .ForeignKey("acpl.DisciplineSemesterPlan", t => t.DisciplineSemesterPlan_DisciplineSemesterPlanId)
                .ForeignKey("dbo.Job", t => t.Job_JobId)
                .Index(t => t.DisciplineSemesterPlan_DisciplineSemesterPlanId)
                .Index(t => t.Job_JobId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DisciplineSemesterPlanJob", "Job_JobId", "dbo.Job");
            DropForeignKey("dbo.DisciplineSemesterPlanJob", "DisciplineSemesterPlan_DisciplineSemesterPlanId", "acpl.DisciplineSemesterPlan");
            DropIndex("dbo.DisciplineSemesterPlanJob", new[] { "Job_JobId" });
            DropIndex("dbo.DisciplineSemesterPlanJob", new[] { "DisciplineSemesterPlan_DisciplineSemesterPlanId" });
            DropTable("dbo.DisciplineSemesterPlanJob");
        }
    }
}
