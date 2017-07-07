namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStudyLoadCalculationStringid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Discipline", "StudyLoadCalculationStringId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Discipline", "StudyLoadCalculationStringId");
        }
    }
}
