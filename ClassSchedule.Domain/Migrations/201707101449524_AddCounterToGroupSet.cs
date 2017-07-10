namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCounterToGroupSet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupSet", "Counter", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupSet", "Counter");
        }
    }
}
