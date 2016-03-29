namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLessonTypeGuidInLessonTypeTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dict.LessonType", "LessonTypeGuid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dict.LessonType", "LessonTypeGuid");
        }
    }
}
