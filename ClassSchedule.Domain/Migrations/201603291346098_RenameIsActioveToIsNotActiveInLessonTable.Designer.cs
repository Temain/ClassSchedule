// <auto-generated />
namespace ClassSchedule.Domain.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.0-30225")]
    public sealed partial class RenameIsActioveToIsNotActiveInLessonTable : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(RenameIsActioveToIsNotActiveInLessonTable));
        
        string IMigrationMetadata.Id
        {
            get { return "201603291346098_RenameIsActioveToIsNotActiveInLessonTable"; }
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
