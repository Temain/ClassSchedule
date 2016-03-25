namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveChairCodeFieldInAuditoriumTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Auditorium", "ChairCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Auditorium", "ChairCode", c => c.String());
        }
    }
}
