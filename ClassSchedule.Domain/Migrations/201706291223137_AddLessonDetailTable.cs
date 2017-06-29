namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLessonDetailTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Lesson", "AuditoriumId", "dbo.Auditorium");
            DropForeignKey("dbo.Lesson", "PlannedChairJobId", "dbo.PlannedChairJob");
            DropIndex("dbo.Lesson", new[] { "AuditoriumId" });
            DropIndex("dbo.Lesson", new[] { "PlannedChairJobId" });
            CreateTable(
                "dbo.LessonDetail",
                c => new
                    {
                        LessonDetailId = c.Int(nullable: false, identity: true),
                        LessonDetailGuid = c.Guid(),
                        AuditoriumId = c.Int(nullable: false),
                        PlannedChairJobId = c.Int(),
                        Order = c.Int(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.LessonDetailId)
                .ForeignKey("dbo.Auditorium", t => t.AuditoriumId)
                .ForeignKey("dbo.PlannedChairJob", t => t.PlannedChairJobId)
                .Index(t => t.AuditoriumId)
                .Index(t => t.PlannedChairJobId);
            
            DropColumn("dbo.Lesson", "AuditoriumId");
            DropColumn("dbo.Lesson", "PlannedChairJobId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lesson", "PlannedChairJobId", c => c.Int());
            AddColumn("dbo.Lesson", "AuditoriumId", c => c.Int(nullable: false));
            DropForeignKey("dbo.LessonDetail", "PlannedChairJobId", "dbo.PlannedChairJob");
            DropForeignKey("dbo.LessonDetail", "AuditoriumId", "dbo.Auditorium");
            DropIndex("dbo.LessonDetail", new[] { "PlannedChairJobId" });
            DropIndex("dbo.LessonDetail", new[] { "AuditoriumId" });
            DropTable("dbo.LessonDetail");
            CreateIndex("dbo.Lesson", "PlannedChairJobId");
            CreateIndex("dbo.Lesson", "AuditoriumId");
            AddForeignKey("dbo.Lesson", "PlannedChairJobId", "dbo.PlannedChairJob", "PlannedChairJobId");
            AddForeignKey("dbo.Lesson", "AuditoriumId", "dbo.Auditorium", "AuditoriumId");
        }
    }
}
