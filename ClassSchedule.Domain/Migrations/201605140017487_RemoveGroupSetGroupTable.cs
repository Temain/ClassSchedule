namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveGroupSetGroupTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GroupSetGroup", "GroupSetId", "dbo.GroupSet");
            DropForeignKey("dbo.GroupSetGroup", "GroupId", "dict.Group");
            DropIndex("dbo.GroupSetGroup", new[] { "GroupSetId" });
            DropIndex("dbo.GroupSetGroup", new[] { "GroupId" });
            DropTable("dbo.GroupSetGroup");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GroupSetGroup",
                c => new
                    {
                        GroupSetId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupSetId, t.GroupId });
            
            CreateIndex("dbo.GroupSetGroup", "GroupId");
            CreateIndex("dbo.GroupSetGroup", "GroupSetId");
            AddForeignKey("dbo.GroupSetGroup", "GroupId", "dict.Group", "GroupId");
            AddForeignKey("dbo.GroupSetGroup", "GroupSetId", "dbo.GroupSet", "GroupSetId");
        }
    }
}
