namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIsActioveToIsNotActiveInLessonTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lesson", "IsNotActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.Lesson", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lesson", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.Lesson", "IsNotActive");
        }
    }
}
