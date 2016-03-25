namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakePlacesFieldNotRequiredInAuditoriumTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Auditorium", "Places", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Auditorium", "Places", c => c.Int(nullable: false));
        }
    }
}
