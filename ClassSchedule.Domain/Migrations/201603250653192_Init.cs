namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "plan.AcademicPlan",
                c => new
                    {
                        AcademicPlanId = c.Int(nullable: false, identity: true),
                        AcademicPlanName = c.String(),
                        ChairId = c.Int(),
                        ProgramOfEducationId = c.Int(nullable: false),
                        UploadedAt = c.DateTime(nullable: false),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.AcademicPlanId)
                .ForeignKey("dbo.ProgramOfEducation", t => t.ProgramOfEducationId)
                .ForeignKey("dbo.Chair", t => t.ChairId)
                .Index(t => t.ChairId)
                .Index(t => t.ProgramOfEducationId);
            
            CreateTable(
                "dbo.Chair",
                c => new
                    {
                        ChairId = c.Int(nullable: false, identity: true),
                        ChairGuid = c.Guid(),
                        FacultyId = c.Int(nullable: false),
                        DivisionCode = c.String(maxLength: 20),
                        DivisionName = c.String(nullable: false, maxLength: 200),
                        ParentId = c.Guid(),
                        IsFaculty = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ChairId)
                .ForeignKey("dict.Faculty", t => t.FacultyId)
                .Index(t => t.FacultyId);
            
            CreateTable(
                "dict.Faculty",
                c => new
                    {
                        FacultyId = c.Int(nullable: false, identity: true),
                        FacultyGuid = c.Guid(),
                        DivisionCode = c.String(maxLength: 20),
                        DivisionName = c.String(maxLength: 200),
                        ParentId = c.Guid(),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.FacultyId);
            
            CreateTable(
                "dbo.Course",
                c => new
                    {
                        CourseId = c.Int(nullable: false, identity: true),
                        CourseGuid = c.Guid(),
                        FacultyId = c.Int(nullable: false),
                        CourseName = c.String(nullable: false, maxLength: 50),
                        CourseNumber = c.Int(nullable: false),
                        YearStart = c.Int(),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.CourseId)
                .ForeignKey("dict.Faculty", t => t.FacultyId)
                .Index(t => t.FacultyId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        WeekNumber = c.Int(nullable: false),
                        CourseId = c.Int(),
                        GroupId = c.Int(),
                        EducationYearId = c.Int(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Course", t => t.CourseId)
                .ForeignKey("dict.EducationYear", t => t.EducationYearId)
                .ForeignKey("dict.Group", t => t.GroupId)
                .Index(t => t.CourseId)
                .Index(t => t.GroupId)
                .Index(t => t.EducationYearId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dict.EducationYear",
                c => new
                    {
                        EducationYearId = c.Int(nullable: false, identity: true),
                        EducationYearGuid = c.Guid(),
                        EducationYearName = c.String(nullable: false, maxLength: 20),
                        YearStart = c.Int(nullable: false),
                        YearEnd = c.Int(nullable: false),
                        DateStart = c.DateTime(nullable: false),
                        DateEnd = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationYearId);
            
            CreateTable(
                "dict.Group",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        GroupGuid = c.Guid(),
                        CourseId = c.Int(nullable: false),
                        DivisionCode = c.String(maxLength: 20),
                        DivisionName = c.String(nullable: false, maxLength: 200),
                        ParentGuid = c.Guid(),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                        ProgramOfEducationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupId)
                .ForeignKey("dbo.Course", t => t.CourseId)
                .ForeignKey("dbo.ProgramOfEducation", t => t.ProgramOfEducationId)
                .Index(t => t.CourseId)
                .Index(t => t.ProgramOfEducationId);
            
            CreateTable(
                "dbo.Lesson",
                c => new
                    {
                        LessonId = c.Int(nullable: false, identity: true),
                        LessonGuid = c.Guid(),
                        ClassNumber = c.Int(nullable: false),
                        ClassDate = c.DateTime(nullable: false),
                        WeekNumber = c.Int(nullable: false),
                        DayNumber = c.Int(nullable: false),
                        ChairId = c.Int(nullable: false),
                        JobId = c.Int(nullable: false),
                        DisciplineId = c.Int(nullable: false),
                        AuditoriumId = c.Int(),
                        GroupId = c.Int(nullable: false),
                        LessonTypeId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.LessonId)
                .ForeignKey("dbo.Auditorium", t => t.AuditoriumId)
                .ForeignKey("dbo.Chair", t => t.ChairId)
                .ForeignKey("dict.Discipline", t => t.DisciplineId)
                .ForeignKey("dict.Group", t => t.GroupId)
                .ForeignKey("dbo.Job", t => t.JobId)
                .ForeignKey("dict.LessonType", t => t.LessonTypeId)
                .Index(t => t.ChairId)
                .Index(t => t.JobId)
                .Index(t => t.DisciplineId)
                .Index(t => t.AuditoriumId)
                .Index(t => t.GroupId)
                .Index(t => t.LessonTypeId);
            
            CreateTable(
                "dbo.Auditorium",
                c => new
                    {
                        AuditoriumId = c.Int(nullable: false, identity: true),
                        AuditoriumNumber = c.String(),
                        HousingId = c.Int(nullable: false),
                        AuditoriumTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AuditoriumId)
                .ForeignKey("dict.AuditoriumType", t => t.AuditoriumTypeId)
                .ForeignKey("dict.Housing", t => t.HousingId)
                .Index(t => t.HousingId)
                .Index(t => t.AuditoriumTypeId);
            
            CreateTable(
                "dict.AuditoriumType",
                c => new
                    {
                        AuditoriumTypeId = c.Int(nullable: false, identity: true),
                        AuditoriumTypeName = c.String(),
                    })
                .PrimaryKey(t => t.AuditoriumTypeId);
            
            CreateTable(
                "dict.Housing",
                c => new
                    {
                        HousingId = c.Int(nullable: false, identity: true),
                        HousingName = c.String(),
                    })
                .PrimaryKey(t => t.HousingId);
            
            CreateTable(
                "dict.Discipline",
                c => new
                    {
                        DisciplineId = c.Int(nullable: false, identity: true),
                        DisciplineGuid = c.Guid(),
                        DisciplineName = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.DisciplineId);
            
            CreateTable(
                "dbo.Job",
                c => new
                    {
                        JobId = c.Int(nullable: false, identity: true),
                        JobGuid = c.Guid(),
                        ChairId = c.Int(nullable: false),
                        PositionId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        JobDateStart = c.DateTime(nullable: false),
                        JobDateEnd = c.DateTime(),
                        PositionQuantity = c.Single(nullable: false),
                        EmploymentTypeId = c.Int(nullable: false),
                        IsTeacher = c.Boolean(),
                        IsDean = c.Boolean(),
                        IsHeadOfChair = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.JobId)
                .ForeignKey("dbo.Chair", t => t.ChairId)
                .ForeignKey("dbo.Employee", t => t.EmployeeId)
                .ForeignKey("dict.EmploymentType", t => t.EmploymentTypeId)
                .ForeignKey("dict.Position", t => t.PositionId)
                .Index(t => t.ChairId)
                .Index(t => t.PositionId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmploymentTypeId);
            
            CreateTable(
                "plan.DisciplineSemesterPlan",
                c => new
                    {
                        DisciplineSemesterPlanId = c.Int(nullable: false, identity: true),
                        DisciplineId = c.Int(nullable: false),
                        HoursOfLectures = c.Int(),
                        HoursOfLaboratory = c.Int(),
                        HoursOfPractice = c.Int(),
                        LecturesPerWeek = c.Int(nullable: false),
                        PracticePerWeek = c.Int(nullable: false),
                        LaboratoryPerWeek = c.Int(nullable: false),
                        SemesterScheduleId = c.Int(nullable: false),
                        SessionControlTypeId = c.Int(),
                    })
                .PrimaryKey(t => t.DisciplineSemesterPlanId)
                .ForeignKey("dict.Discipline", t => t.DisciplineId)
                .ForeignKey("plan.SemesterSchedule", t => t.SemesterScheduleId)
                .ForeignKey("dict.SessionControlType", t => t.SessionControlTypeId)
                .Index(t => t.DisciplineId)
                .Index(t => t.SemesterScheduleId)
                .Index(t => t.SessionControlTypeId);
            
            CreateTable(
                "plan.DisciplineWeekPlan",
                c => new
                    {
                        DisciplineWeekPlanId = c.Int(nullable: false, identity: true),
                        HoursOfLectures = c.Int(),
                        HoursOfLaboratory = c.Int(),
                        HoursOfPractice = c.Int(),
                        DisciplineSemesterPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DisciplineWeekPlanId)
                .ForeignKey("plan.DisciplineSemesterPlan", t => t.DisciplineSemesterPlanId)
                .Index(t => t.DisciplineSemesterPlanId);
            
            CreateTable(
                "plan.SemesterSchedule",
                c => new
                    {
                        SemesterScheduleId = c.Int(nullable: false, identity: true),
                        SemesterNumber = c.Int(nullable: false),
                        NumberOfFirstWeek = c.Int(nullable: false),
                        TheoreticalTrainingWeeks = c.Int(nullable: false),
                        ExamSessionWeeks = c.Int(nullable: false),
                        StudyTrainingWeeks = c.Int(nullable: false),
                        PracticalTrainingWeeks = c.Int(nullable: false),
                        FinalQualifyingWorkWeeks = c.Int(nullable: false),
                        StateExamsWeeks = c.Int(nullable: false),
                        WeeksOfHolidays = c.Int(nullable: false),
                        CourseScheduleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SemesterScheduleId)
                .ForeignKey("plan.CourseSchedule", t => t.CourseScheduleId)
                .Index(t => t.CourseScheduleId);
            
            CreateTable(
                "plan.CourseSchedule",
                c => new
                    {
                        CourseScheduleId = c.Int(nullable: false, identity: true),
                        CourseNumber = c.Int(nullable: false),
                        FirstMaxLoad = c.Single(nullable: false),
                        SecondMaxLoad = c.Single(nullable: false),
                        TheoreticalTrainingWeeks = c.Int(nullable: false),
                        ExamSessionWeeks = c.Int(nullable: false),
                        StudyTrainingWeeks = c.Int(nullable: false),
                        PracticalTrainingWeeks = c.Int(nullable: false),
                        FinalQualifyingWorkWeeks = c.Int(nullable: false),
                        StateExamsWeeks = c.Int(nullable: false),
                        WeeksOfHolidays = c.Int(nullable: false),
                        AcademicPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourseScheduleId)
                .ForeignKey("plan.AcademicPlan", t => t.AcademicPlanId)
                .Index(t => t.AcademicPlanId);
            
            CreateTable(
                "dict.SessionControlType",
                c => new
                    {
                        SessionControlTypeId = c.Int(nullable: false, identity: true),
                        SessionControlTypeName = c.String(),
                    })
                .PrimaryKey(t => t.SessionControlTypeId);
            
            CreateTable(
                "dbo.Employee",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        EmployeeGuid = c.Guid(),
                        PersonId = c.Int(nullable: false),
                        EmployeeCode = c.String(nullable: false, maxLength: 20),
                        EmployeeDateStart = c.DateTime(nullable: false),
                        EmployeeDateEnd = c.DateTime(),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmployeeId)
                .ForeignKey("dbo.Person", t => t.PersonId)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.Person",
                c => new
                    {
                        PersonId = c.Int(nullable: false, identity: true),
                        PersonGuid = c.Guid(),
                        PersonMasterGuid = c.Guid(),
                        PersonCode = c.String(maxLength: 20),
                        LastName = c.String(nullable: false, maxLength: 200),
                        FirstName = c.String(nullable: false, maxLength: 200),
                        MiddleName = c.String(maxLength: 200),
                        Birthday = c.DateTime(),
                        Sex = c.Byte(),
                        PassportNumber = c.String(),
                        PassportSeries = c.String(),
                        IsMarkedAsDuplicated = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.PersonId);
            
            CreateTable(
                "dict.EmploymentType",
                c => new
                    {
                        EmploymentTypeId = c.Int(nullable: false, identity: true),
                        EmploymentTypeGuid = c.Guid(),
                        EmploymentTypeName = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmploymentTypeId);
            
            CreateTable(
                "dbo.PersonVacation",
                c => new
                    {
                        PersonVacationId = c.Int(nullable: false, identity: true),
                        PersonVacationGuid = c.Guid(),
                        PersonId = c.Int(nullable: false),
                        VacationBeginningDate = c.DateTime(),
                        VacationTerminationDate = c.DateTime(),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                        JobId = c.Int(),
                    })
                .PrimaryKey(t => t.PersonVacationId)
                .ForeignKey("dbo.Job", t => t.JobId)
                .ForeignKey("dbo.Person", t => t.PersonId)
                .Index(t => t.PersonId)
                .Index(t => t.JobId);
            
            CreateTable(
                "dict.Position",
                c => new
                    {
                        PositionId = c.Int(nullable: false, identity: true),
                        PositionGuid = c.Guid(),
                        PositionCode = c.String(maxLength: 20),
                        PositionName = c.String(nullable: false, maxLength: 100),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.PositionId);
            
            CreateTable(
                "dict.LessonType",
                c => new
                    {
                        LessonTypeId = c.Int(nullable: false, identity: true),
                        LessonTypeName = c.String(),
                    })
                .PrimaryKey(t => t.LessonTypeId);
            
            CreateTable(
                "dbo.ProgramOfEducation",
                c => new
                    {
                        ProgramOfEducationId = c.Int(nullable: false, identity: true),
                        ProgramOfEducationGuid = c.Guid(),
                        EducationProfileId = c.Int(nullable: false),
                        EducationFormId = c.Int(nullable: false),
                        EducationLevelId = c.Int(nullable: false),
                        YearStart = c.Short(nullable: false),
                        YearEnd = c.Short(),
                        ProgramOfEducationCode = c.String(maxLength: 5),
                        ProgramOfEducationName = c.String(maxLength: 200),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProgramOfEducationId)
                .ForeignKey("dict.EducationForm", t => t.EducationFormId)
                .ForeignKey("dict.EducationLevel", t => t.EducationLevelId)
                .ForeignKey("dict.EducationProfile", t => t.EducationProfileId)
                .Index(t => t.EducationProfileId)
                .Index(t => t.EducationFormId)
                .Index(t => t.EducationLevelId);
            
            CreateTable(
                "dict.EducationForm",
                c => new
                    {
                        EducationFormId = c.Int(nullable: false, identity: true),
                        EducationFormGuid = c.Guid(),
                        EducationFormName = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationFormId);
            
            CreateTable(
                "dict.EducationLevel",
                c => new
                    {
                        EducationLevelId = c.Int(nullable: false, identity: true),
                        EducationLevelGuid = c.Guid(),
                        EducationLevelName = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationLevelId);
            
            CreateTable(
                "dict.EducationProfile",
                c => new
                    {
                        EducationProfileId = c.Int(nullable: false, identity: true),
                        EducationProfileGuid = c.Guid(),
                        EducationDirectionId = c.Int(nullable: false),
                        EducationProfileName = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationProfileId)
                .ForeignKey("dict.EducationDirection", t => t.EducationDirectionId)
                .Index(t => t.EducationDirectionId);
            
            CreateTable(
                "dict.EducationDirection",
                c => new
                    {
                        EducationDirectionId = c.Int(nullable: false, identity: true),
                        EducationDirectionGuid = c.Guid(),
                        EducationDirectionCode = c.String(nullable: false, maxLength: 20),
                        EducationDirectionName = c.String(nullable: false, maxLength: 200),
                        EducationDirectionFGOSVO = c.String(),
                        IsDeleted = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationDirectionId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "serv.LogEntry",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.String(),
                        Level = c.String(),
                        Logger = c.String(),
                        ClassMethod = c.String(),
                        Message = c.String(),
                        Username = c.String(),
                        RequestUri = c.String(),
                        RemoteAddress = c.String(),
                        UserAgent = c.String(),
                        Exception = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        FullName = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.DisciplineSemesterPlanJob",
                c => new
                    {
                        DisciplineSemesterPlanId = c.Int(nullable: false),
                        JobId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DisciplineSemesterPlanId, t.JobId })
                .ForeignKey("plan.DisciplineSemesterPlan", t => t.DisciplineSemesterPlanId)
                .ForeignKey("dbo.Job", t => t.JobId)
                .Index(t => t.DisciplineSemesterPlanId)
                .Index(t => t.JobId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("plan.AcademicPlan", "ChairId", "dbo.Chair");
            DropForeignKey("dbo.Chair", "FacultyId", "dict.Faculty");
            DropForeignKey("dbo.Course", "FacultyId", "dict.Faculty");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dict.Group", "ProgramOfEducationId", "dbo.ProgramOfEducation");
            DropForeignKey("dbo.ProgramOfEducation", "EducationProfileId", "dict.EducationProfile");
            DropForeignKey("dict.EducationProfile", "EducationDirectionId", "dict.EducationDirection");
            DropForeignKey("dbo.ProgramOfEducation", "EducationLevelId", "dict.EducationLevel");
            DropForeignKey("dbo.ProgramOfEducation", "EducationFormId", "dict.EducationForm");
            DropForeignKey("plan.AcademicPlan", "ProgramOfEducationId", "dbo.ProgramOfEducation");
            DropForeignKey("dbo.Lesson", "LessonTypeId", "dict.LessonType");
            DropForeignKey("dbo.Job", "PositionId", "dict.Position");
            DropForeignKey("dbo.PersonVacation", "PersonId", "dbo.Person");
            DropForeignKey("dbo.PersonVacation", "JobId", "dbo.Job");
            DropForeignKey("dbo.Lesson", "JobId", "dbo.Job");
            DropForeignKey("dbo.Job", "EmploymentTypeId", "dict.EmploymentType");
            DropForeignKey("dbo.Job", "EmployeeId", "dbo.Employee");
            DropForeignKey("dbo.Employee", "PersonId", "dbo.Person");
            DropForeignKey("plan.DisciplineSemesterPlan", "SessionControlTypeId", "dict.SessionControlType");
            DropForeignKey("plan.DisciplineSemesterPlan", "SemesterScheduleId", "plan.SemesterSchedule");
            DropForeignKey("plan.SemesterSchedule", "CourseScheduleId", "plan.CourseSchedule");
            DropForeignKey("plan.CourseSchedule", "AcademicPlanId", "plan.AcademicPlan");
            DropForeignKey("dbo.DisciplineSemesterPlanJob", "JobId", "dbo.Job");
            DropForeignKey("dbo.DisciplineSemesterPlanJob", "DisciplineSemesterPlanId", "plan.DisciplineSemesterPlan");
            DropForeignKey("plan.DisciplineWeekPlan", "DisciplineSemesterPlanId", "plan.DisciplineSemesterPlan");
            DropForeignKey("plan.DisciplineSemesterPlan", "DisciplineId", "dict.Discipline");
            DropForeignKey("dbo.Job", "ChairId", "dbo.Chair");
            DropForeignKey("dbo.Lesson", "GroupId", "dict.Group");
            DropForeignKey("dbo.Lesson", "DisciplineId", "dict.Discipline");
            DropForeignKey("dbo.Lesson", "ChairId", "dbo.Chair");
            DropForeignKey("dbo.Lesson", "AuditoriumId", "dbo.Auditorium");
            DropForeignKey("dbo.Auditorium", "HousingId", "dict.Housing");
            DropForeignKey("dbo.Auditorium", "AuditoriumTypeId", "dict.AuditoriumType");
            DropForeignKey("dict.Group", "CourseId", "dbo.Course");
            DropForeignKey("dbo.AspNetUsers", "GroupId", "dict.Group");
            DropForeignKey("dbo.AspNetUsers", "EducationYearId", "dict.EducationYear");
            DropForeignKey("dbo.AspNetUsers", "CourseId", "dbo.Course");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.DisciplineSemesterPlanJob", new[] { "JobId" });
            DropIndex("dbo.DisciplineSemesterPlanJob", new[] { "DisciplineSemesterPlanId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dict.EducationProfile", new[] { "EducationDirectionId" });
            DropIndex("dbo.ProgramOfEducation", new[] { "EducationLevelId" });
            DropIndex("dbo.ProgramOfEducation", new[] { "EducationFormId" });
            DropIndex("dbo.ProgramOfEducation", new[] { "EducationProfileId" });
            DropIndex("dbo.PersonVacation", new[] { "JobId" });
            DropIndex("dbo.PersonVacation", new[] { "PersonId" });
            DropIndex("dbo.Employee", new[] { "PersonId" });
            DropIndex("plan.CourseSchedule", new[] { "AcademicPlanId" });
            DropIndex("plan.SemesterSchedule", new[] { "CourseScheduleId" });
            DropIndex("plan.DisciplineWeekPlan", new[] { "DisciplineSemesterPlanId" });
            DropIndex("plan.DisciplineSemesterPlan", new[] { "SessionControlTypeId" });
            DropIndex("plan.DisciplineSemesterPlan", new[] { "SemesterScheduleId" });
            DropIndex("plan.DisciplineSemesterPlan", new[] { "DisciplineId" });
            DropIndex("dbo.Job", new[] { "EmploymentTypeId" });
            DropIndex("dbo.Job", new[] { "EmployeeId" });
            DropIndex("dbo.Job", new[] { "PositionId" });
            DropIndex("dbo.Job", new[] { "ChairId" });
            DropIndex("dbo.Auditorium", new[] { "AuditoriumTypeId" });
            DropIndex("dbo.Auditorium", new[] { "HousingId" });
            DropIndex("dbo.Lesson", new[] { "LessonTypeId" });
            DropIndex("dbo.Lesson", new[] { "GroupId" });
            DropIndex("dbo.Lesson", new[] { "AuditoriumId" });
            DropIndex("dbo.Lesson", new[] { "DisciplineId" });
            DropIndex("dbo.Lesson", new[] { "JobId" });
            DropIndex("dbo.Lesson", new[] { "ChairId" });
            DropIndex("dict.Group", new[] { "ProgramOfEducationId" });
            DropIndex("dict.Group", new[] { "CourseId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "EducationYearId" });
            DropIndex("dbo.AspNetUsers", new[] { "GroupId" });
            DropIndex("dbo.AspNetUsers", new[] { "CourseId" });
            DropIndex("dbo.Course", new[] { "FacultyId" });
            DropIndex("dbo.Chair", new[] { "FacultyId" });
            DropIndex("plan.AcademicPlan", new[] { "ProgramOfEducationId" });
            DropIndex("plan.AcademicPlan", new[] { "ChairId" });
            DropTable("dbo.DisciplineSemesterPlanJob");
            DropTable("dbo.AspNetRoles");
            DropTable("serv.LogEntry");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dict.EducationDirection");
            DropTable("dict.EducationProfile");
            DropTable("dict.EducationLevel");
            DropTable("dict.EducationForm");
            DropTable("dbo.ProgramOfEducation");
            DropTable("dict.LessonType");
            DropTable("dict.Position");
            DropTable("dbo.PersonVacation");
            DropTable("dict.EmploymentType");
            DropTable("dbo.Person");
            DropTable("dbo.Employee");
            DropTable("dict.SessionControlType");
            DropTable("plan.CourseSchedule");
            DropTable("plan.SemesterSchedule");
            DropTable("plan.DisciplineWeekPlan");
            DropTable("plan.DisciplineSemesterPlan");
            DropTable("dbo.Job");
            DropTable("dict.Discipline");
            DropTable("dict.Housing");
            DropTable("dict.AuditoriumType");
            DropTable("dbo.Auditorium");
            DropTable("dbo.Lesson");
            DropTable("dict.Group");
            DropTable("dict.EducationYear");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Course");
            DropTable("dict.Faculty");
            DropTable("dbo.Chair");
            DropTable("plan.AcademicPlan");
        }
    }
}
