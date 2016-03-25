namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentFieldToAuditoriumTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auditorium", "Comment", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auditorium", "Comment");
        }
    }
}
