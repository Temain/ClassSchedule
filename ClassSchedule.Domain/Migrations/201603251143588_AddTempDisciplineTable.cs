namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTempDisciplineTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "tmp.TempDiscipline",
                c => new
                    {
                        TempDisciplineId = c.Int(nullable: false, identity: true),
                        DisciplineName = c.String(),
                        ChairCode = c.String(),
                    })
                .PrimaryKey(t => t.TempDisciplineId);
            
        }
        
        public override void Down()
        {
            DropTable("tmp.TempDiscipline");
        }
    }
}
