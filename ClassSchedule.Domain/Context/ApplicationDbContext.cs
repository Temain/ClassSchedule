using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ClassSchedule.Domain.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ClassSchedule.Domain.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AcademicPlan> AcademicPlans { get; set; }
        public DbSet<Auditorium> Auditoriums { get; set; }
        public DbSet<AuditoriumType> AuditoriumTypes { get; set; }
        public DbSet<Chair> Chairs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSchedule> CourseSchedules { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<DisciplineSemesterPlan> DisciplineSemesterPlans { get; set; }
        // public DbSet<DisciplineSemesterPlanJob> DisciplineSemesterPlanJobs { get; set; }
        public DbSet<DisciplineWeekPlan> DisciplineWeekPlans { get; set; }
        public DbSet<EducationDirection> EducationDirections { get; set; }
        public DbSet<EducationForm> EducationForms { get; set; }
        public DbSet<EducationLevel> EducationLevels { get; set; }
        public DbSet<EducationYear> EducationYears { get; set; }  
        public DbSet<EducationProfile> EducationProfiles { get; set; }
        public DbSet<Employee> Employees{ get; set; }
        public DbSet<EmploymentType> EmploymentTypes { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Housing> Housings { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonType> LessonTypes { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonVacation> PersonVacations { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<ProgramOfEducation> ProgramOfEducations { get; set; }
        public DbSet<SemesterSchedule> SemesterSchedules { get; set; }
        public DbSet<SessionControlType> SessionControlTypes { get; set; }
        public DbSet<TempDiscipline> TempDisciplines { get; set; }   


        public ApplicationDbContext()
            : base("ClassScheduleConnection", throwIfV1Schema: false)
        {
            // this.Configuration.LazyLoadingEnabled = false;      
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

            modelBuilder.Entity<DisciplineSemesterPlan>()
                .HasMany(p => p.Jobs)
                .WithMany(s => s.DisciplineSemesterPlans)
                .Map(c =>
                    {
                        c.MapLeftKey("DisciplineSemesterPlanId");
                        c.MapRightKey("JobId");
                        c.ToTable("DisciplineSemesterPlanJob");
                    });

            base.OnModelCreating(modelBuilder);
        }
    }
}
