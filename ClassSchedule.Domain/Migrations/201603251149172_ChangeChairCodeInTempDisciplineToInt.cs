namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeChairCodeInTempDisciplineToInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("tmp.TempDiscipline", "ChairCode", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("tmp.TempDiscipline", "ChairCode", c => c.String());
        }
    }
}
