// <auto-generated />
namespace ClassSchedule.Domain.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class AddRelationBetweenLessonAndLessonDetail : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(AddRelationBetweenLessonAndLessonDetail));
        
        string IMigrationMetadata.Id
        {
            get { return "201706291327191_AddRelationBetweenLessonAndLessonDetail"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}