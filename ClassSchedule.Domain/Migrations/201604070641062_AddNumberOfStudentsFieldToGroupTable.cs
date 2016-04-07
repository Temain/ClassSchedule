namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNumberOfStudentsFieldToGroupTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dict.Group", "NumberOfStudents", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dict.Group", "NumberOfStudents");
        }
    }
}
