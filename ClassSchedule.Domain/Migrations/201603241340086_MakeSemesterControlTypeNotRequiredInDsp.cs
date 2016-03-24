namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeSemesterControlTypeNotRequiredInDsp : DbMigration
    {
        public override void Up()
        {
            DropIndex("plan.DisciplineSemesterPlan", new[] { "SessionControlTypeId" });
            AlterColumn("plan.DisciplineSemesterPlan", "SessionControlTypeId", c => c.Int());
            CreateIndex("plan.DisciplineSemesterPlan", "SessionControlTypeId");
        }
        
        public override void Down()
        {
            DropIndex("plan.DisciplineSemesterPlan", new[] { "SessionControlTypeId" });
            AlterColumn("plan.DisciplineSemesterPlan", "SessionControlTypeId", c => c.Int(nullable: false));
            CreateIndex("plan.DisciplineSemesterPlan", "SessionControlTypeId");
        }
    }
}
