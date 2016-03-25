namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChairIdFieldToAuditoriumTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auditorium", "ChairId", c => c.Int());
            AddColumn("dbo.Auditorium", "ChairCode", c => c.String());
            CreateIndex("dbo.Auditorium", "ChairId");
            AddForeignKey("dbo.Auditorium", "ChairId", "dbo.Chair", "ChairId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Auditorium", "ChairId", "dbo.Chair");
            DropIndex("dbo.Auditorium", new[] { "ChairId" });
            DropColumn("dbo.Auditorium", "ChairCode");
            DropColumn("dbo.Auditorium", "ChairId");
        }
    }
}
