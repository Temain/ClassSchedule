namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStudyLoadCalculationStringName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Discipline", "StudyLoadCalculationStringName", c => c.String());
            AddColumn("dbo.Discipline", "StudyLoadCalculationStringGuid", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Discipline", "StudyLoadCalculationStringGuid");
            DropColumn("dbo.Discipline", "StudyLoadCalculationStringName");
        }
    }
}
