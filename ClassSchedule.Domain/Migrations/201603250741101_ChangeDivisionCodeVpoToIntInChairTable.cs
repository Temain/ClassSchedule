namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDivisionCodeVpoToIntInChairTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Chair", "DivisionCodeVpo", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Chair", "DivisionCodeVpo", c => c.String(maxLength: 20));
        }
    }
}
