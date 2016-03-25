namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChairIdFieldToDspTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dict.Discipline", "ChairId", c => c.Int(nullable: false));
            CreateIndex("dict.Discipline", "ChairId");
            AddForeignKey("dict.Discipline", "ChairId", "dbo.Chair", "ChairId");
        }
        
        public override void Down()
        {
            DropForeignKey("dict.Discipline", "ChairId", "dbo.Chair");
            DropIndex("dict.Discipline", new[] { "ChairId" });
            DropColumn("dict.Discipline", "ChairId");
        }
    }
}
