namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFieldsFromDiscipline : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Discipline", "IsExludedFromCalculation");
            DropColumn("dbo.Discipline", "IsFakeDiscipline");
            DropColumn("dbo.Discipline", "IsHeadOfSectionDiscipline");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Discipline", "IsHeadOfSectionDiscipline", c => c.Boolean(nullable: false));
            AddColumn("dbo.Discipline", "IsFakeDiscipline", c => c.Boolean(nullable: false));
            AddColumn("dbo.Discipline", "IsExludedFromCalculation", c => c.Boolean(nullable: false));
        }
    }
}
