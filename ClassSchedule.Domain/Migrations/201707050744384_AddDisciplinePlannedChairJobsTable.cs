namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDisciplinePlannedChairJobsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DisciplinePlannedChairJobs",
                c => new
                    {
                        PlannedChairJobId = c.Int(nullable: false),
                        DisciplineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PlannedChairJobId, t.DisciplineId })
                .ForeignKey("dbo.Discipline", t => t.PlannedChairJobId)
                .ForeignKey("dbo.PlannedChairJob", t => t.DisciplineId)
                .Index(t => t.PlannedChairJobId)
                .Index(t => t.DisciplineId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DisciplinePlannedChairJobs", "DisciplineId", "dbo.PlannedChairJob");
            DropForeignKey("dbo.DisciplinePlannedChairJobs", "PlannedChairJobId", "dbo.Discipline");
            DropIndex("dbo.DisciplinePlannedChairJobs", new[] { "DisciplineId" });
            DropIndex("dbo.DisciplinePlannedChairJobs", new[] { "PlannedChairJobId" });
            DropTable("dbo.DisciplinePlannedChairJobs");
        }
    }
}
