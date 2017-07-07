namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeDisciplineNameNotRequired : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Discipline", new[] { "DisciplineNameId" });
            AlterColumn("dbo.Discipline", "DisciplineNameId", c => c.Int());
            CreateIndex("dbo.Discipline", "DisciplineNameId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Discipline", new[] { "DisciplineNameId" });
            AlterColumn("dbo.Discipline", "DisciplineNameId", c => c.Int(nullable: false));
            CreateIndex("dbo.Discipline", "DisciplineNameId");
        }
    }
}
