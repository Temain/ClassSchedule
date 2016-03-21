namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEducationYearTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dict.EducationYear",
                c => new
                    {
                        EducationYearId = c.Int(nullable: false, identity: true),
                        EducationYearGuid = c.Guid(),
                        EducationYearName = c.String(nullable: false, maxLength: 20),
                        YearStart = c.Int(nullable: false),
                        YearEnd = c.Int(nullable: false),
                        DateStart = c.DateTime(nullable: false),
                        DateEnd = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationYearId);
            
            AddColumn("dbo.AspNetUsers", "EducationYearId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "EducationYearId");
            AddForeignKey("dbo.AspNetUsers", "EducationYearId", "dict.EducationYear", "EducationYearId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "EducationYearId", "dict.EducationYear");
            DropIndex("dbo.AspNetUsers", new[] { "EducationYearId" });
            DropColumn("dbo.AspNetUsers", "EducationYearId");
            DropTable("dict.EducationYear");
        }
    }
}
