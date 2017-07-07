namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemakeRelationDisciplinePlannedChairJobs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DisciplinePlannedChairJobs", "PlannedChairJobId", "dbo.Discipline");
            DropForeignKey("dbo.DisciplinePlannedChairJobs", "DisciplineId", "dbo.PlannedChairJob");
            DropIndex("dbo.DisciplinePlannedChairJobs", new[] { "PlannedChairJobId" });
            DropIndex("dbo.DisciplinePlannedChairJobs", new[] { "DisciplineId" });
            CreateTable(
                "dbo.DisciplinePlannedChairJob",
                c => new
                    {
                        DisciplinePlannedChairJobId = c.Int(nullable: false, identity: true),
                        DisciplineId = c.Int(nullable: false),
                        PlannedChairJobId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DisciplinePlannedChairJobId)
                .ForeignKey("dbo.Discipline", t => t.DisciplineId)
                .ForeignKey("dbo.PlannedChairJob", t => t.PlannedChairJobId)
                .Index(t => t.DisciplineId)
                .Index(t => t.PlannedChairJobId);
            
            DropTable("dbo.DisciplinePlannedChairJobs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DisciplinePlannedChairJobs",
                c => new
                    {
                        PlannedChairJobId = c.Int(nullable: false),
                        DisciplineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PlannedChairJobId, t.DisciplineId });
            
            DropForeignKey("dbo.DisciplinePlannedChairJob", "PlannedChairJobId", "dbo.PlannedChairJob");
            DropForeignKey("dbo.DisciplinePlannedChairJob", "DisciplineId", "dbo.Discipline");
            DropIndex("dbo.DisciplinePlannedChairJob", new[] { "PlannedChairJobId" });
            DropIndex("dbo.DisciplinePlannedChairJob", new[] { "DisciplineId" });
            DropTable("dbo.DisciplinePlannedChairJob");
            CreateIndex("dbo.DisciplinePlannedChairJobs", "DisciplineId");
            CreateIndex("dbo.DisciplinePlannedChairJobs", "PlannedChairJobId");
            AddForeignKey("dbo.DisciplinePlannedChairJobs", "DisciplineId", "dbo.PlannedChairJob", "PlannedChairJobId");
            AddForeignKey("dbo.DisciplinePlannedChairJobs", "PlannedChairJobId", "dbo.Discipline", "DisciplineId");
        }
    }
}
