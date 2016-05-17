namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClassTimeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dict.ClassTime",
                c => new
                    {
                        DayNumber = c.Int(nullable: false),
                        ClassNumber = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.DayNumber, t.ClassNumber });
            
        }
        
        public override void Down()
        {
            DropTable("dict.ClassTime");
        }
    }
}
