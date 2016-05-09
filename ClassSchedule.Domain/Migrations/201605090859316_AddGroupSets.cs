namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupSets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupSet",
                c => new
                    {
                        GroupSetId = c.Int(nullable: false, identity: true),
                        GroupSetName = c.String(),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.GroupSetId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.GroupSetGroup",
                c => new
                    {
                        GroupSetId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupSetId, t.GroupId })
                .ForeignKey("dbo.GroupSet", t => t.GroupSetId)
                .ForeignKey("dict.Group", t => t.GroupId)
                .Index(t => t.GroupSetId)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupSetGroup", "GroupId", "dict.Group");
            DropForeignKey("dbo.GroupSetGroup", "GroupSetId", "dbo.GroupSet");
            DropForeignKey("dbo.GroupSet", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.GroupSetGroup", new[] { "GroupId" });
            DropIndex("dbo.GroupSetGroup", new[] { "GroupSetId" });
            DropIndex("dbo.GroupSet", new[] { "ApplicationUserId" });
            DropTable("dbo.GroupSetGroup");
            DropTable("dbo.GroupSet");
        }
    }
}
