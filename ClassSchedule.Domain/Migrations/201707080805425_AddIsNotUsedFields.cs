namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsNotUsedFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dict.EducationForm", "IsNotUsed", c => c.Boolean(nullable: false));
            AddColumn("dict.EducationLevel", "IsNotUsed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dict.EducationLevel", "IsNotUsed");
            DropColumn("dict.EducationForm", "IsNotUsed");
        }
    }
}
