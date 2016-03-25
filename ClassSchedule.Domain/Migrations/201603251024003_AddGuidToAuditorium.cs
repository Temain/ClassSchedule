namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGuidToAuditorium : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auditorium", "AuditoriumGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.Auditorium", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auditorium", "IsDeleted");
            DropColumn("dbo.Auditorium", "AuditoriumGuid");
        }
    }
}
