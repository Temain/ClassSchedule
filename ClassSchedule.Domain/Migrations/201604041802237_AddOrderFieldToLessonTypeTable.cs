namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderFieldToLessonTypeTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dict.LessonType", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dict.LessonType", "Order");
        }
    }
}
