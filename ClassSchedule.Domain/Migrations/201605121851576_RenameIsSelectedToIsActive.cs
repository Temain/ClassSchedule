namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIsSelectedToIsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupSet", "IsSelected", c => c.Boolean(nullable: false));
            DropColumn("dbo.GroupSet", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GroupSet", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.GroupSet", "IsSelected");
        }
    }
}
