namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdditionalFieldToGroupSetGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupSetGroup",
                c => new
                    {
                        GroupSetId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupSetId, t.GroupId })
                .ForeignKey("dict.Group", t => t.GroupId)
                .ForeignKey("dbo.GroupSet", t => t.GroupSetId)
                .Index(t => t.GroupSetId)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupSetGroup", "GroupSetId", "dbo.GroupSet");
            DropForeignKey("dbo.GroupSetGroup", "GroupId", "dict.Group");
            DropIndex("dbo.GroupSetGroup", new[] { "GroupId" });
            DropIndex("dbo.GroupSetGroup", new[] { "GroupSetId" });
            DropTable("dbo.GroupSetGroup");
        }
    }
}
