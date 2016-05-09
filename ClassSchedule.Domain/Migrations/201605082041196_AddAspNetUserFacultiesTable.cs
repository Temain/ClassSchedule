namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAspNetUserFacultiesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetUserFaculties",
                c => new
                    {
                        FacultyId = c.Int(nullable: false),
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.FacultyId, t.Id })
                .ForeignKey("dict.Faculty", t => t.FacultyId)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.FacultyId)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserFaculties", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserFaculties", "FacultyId", "dict.Faculty");
            DropIndex("dbo.AspNetUserFaculties", new[] { "Id" });
            DropIndex("dbo.AspNetUserFaculties", new[] { "FacultyId" });
            DropTable("dbo.AspNetUserFaculties");
        }
    }
}
