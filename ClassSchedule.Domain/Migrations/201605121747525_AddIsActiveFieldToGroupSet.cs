namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsActiveFieldToGroupSet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupSet", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupSet", "IsActive");
        }
    }
}
