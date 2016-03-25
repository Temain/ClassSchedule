namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlacesFieldToAuditoriumTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auditorium", "Places", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auditorium", "Places");
        }
    }
}
