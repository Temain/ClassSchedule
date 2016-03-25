namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAbbreviationFieldToHousingTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dict.Housing", "Abbreviation", c => c.String(maxLength: 20));
            AlterColumn("dict.Housing", "HousingName", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dict.Housing", "HousingName", c => c.String());
            DropColumn("dict.Housing", "Abbreviation");
        }
    }
}
