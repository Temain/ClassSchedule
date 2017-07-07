using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ClassSchedule.Domain.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ClassSchedule.Domain.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AcademicDegree> AcademicDegrees { get; set; }
        public DbSet<AcademicDegreeLevel> AcademicDegreeLevels { get; set; }
        public DbSet<AcademicStatus> AcademicStatuses { get; set; }
        public DbSet<Auditorium> Auditoriums { get; set; }
        public DbSet<AuditoriumType> AuditoriumTypes { get; set; }
        public DbSet<BaseOfAcceleration> BasesOfAcceleration { get; set; }
        public DbSet<BaseProgramOfEducation> BaseProgramsOfEducation { get; set; }
        public DbSet<Chair> Chairs { get; set; }
        public DbSet<ClassTime> ClassTimes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSchedule> CourseSchedules { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<DisciplinePlannedChairJob> DisciplinePlannedChairJobs { get; set; }
        public DbSet<DisciplineName> DisciplineNames { get; set; }
        public DbSet<EducationDirection> EducationDirections { get; set; }
        public DbSet<EducationForm> EducationForms { get; set; }
        public DbSet<EducationLevel> EducationLevels { get; set; }
        public DbSet<EducationProfile> EducationProfiles { get; set; }
        public DbSet<EducationSemester> EducationSemesters { get; set; }
        public DbSet<EducationYear> EducationYears { get; set; }  
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmploymentType> EmploymentTypes { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupFlowBinding> GroupFlowBindings { get; set; }
        public DbSet<GroupSet> GroupSets { get; set; }
        public DbSet<GroupSetGroup> GroupSetGroups { get; set; }
        public DbSet<GroupSubgroups> GroupSubgroups { get; set; }
        public DbSet<Housing> Housings { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonDetail> LessonDetails { get; set; }
        public DbSet<LessonType> LessonTypes { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PlannedChairJob> PlannedChairJobs { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PositionReal> PositionsReal { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<SemesterSchedule> SemesterSchedules { get; set; }

        public ApplicationDbContext()
            : base("ClassScheduleConnection", throwIfV1Schema: false)
        {
            //this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Отключаем каскадное удаление данных в связанных таблицах
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            // Запрещаем создание имен таблиц в множественном числе в т.ч. при связи многие к многим
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Faculty>()
                .HasMany(p => p.ApplicationUsers)
                .WithMany(s => s.Faculties)
                .Map(c =>
                {
                    c.MapLeftKey("FacultyId");
                    c.MapRightKey("Id");
                    c.ToTable("AspNetUserFaculties");
                });

            modelBuilder.Entity<Schedule>()
               .HasRequired(e => e.ClassTime)
               .WithMany(e => e.Schedule)
               .HasForeignKey(e => new { e.DayNumber, e.ClassNumber })
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Discipline>()
                .HasOptional(m => m.CombinedWithDiscipline)
                .WithMany(t => t.CombinedDisciplines)
                .HasForeignKey(m => m.CombinedWithDisciplineId)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Discipline>()
            //   .HasMany(s => s.PlannedChairJobs)
            //   .WithMany(c => c.Disciplines)
            //   .Map(cs =>
            //   {
            //       cs.MapLeftKey("PlannedChairJobId");
            //       cs.MapRightKey("DisciplineId");
            //       cs.ToTable("DisciplinePlannedChairJobs");
            //   });

            base.OnModelCreating(modelBuilder);
        }
    }
}
