namespace ClassSchedule.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dict.AcademicDegreeLevel",
                c => new
                    {
                        AcademicDegreeLevelId = c.Int(nullable: false, identity: true),
                        AcademicDegreeLevelGuid = c.Guid(),
                        AcademicDegreeLevelName = c.String(nullable: false, maxLength: 40),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.AcademicDegreeLevelId);
            
            CreateTable(
                "dict.AcademicDegree",
                c => new
                    {
                        AcademicDegreeId = c.Int(nullable: false, identity: true),
                        AcademicDegreeGuid = c.Guid(nullable: false),
                        AcademicDegreeName = c.String(nullable: false, maxLength: 50),
                        AcademicDegreeShortName = c.String(maxLength: 20),
                        AcademicDegreeLevelId = c.Int(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.AcademicDegreeId)
                .ForeignKey("dict.AcademicDegreeLevel", t => t.AcademicDegreeLevelId)
                .Index(t => t.AcademicDegreeLevelId);
            
            CreateTable(
                "dbo.Person",
                c => new
                    {
                        PersonId = c.Int(nullable: false, identity: true),
                        PersonGuid = c.Guid(),
                        PersonCode = c.String(maxLength: 20),
                        LastName = c.String(nullable: false, maxLength: 200),
                        FirstName = c.String(nullable: false, maxLength: 200),
                        MiddleName = c.String(maxLength: 200),
                        Birthday = c.DateTime(),
                        AcademicDegreeId = c.Int(),
                        AcademicStatusId = c.Int(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.PersonId)
                .ForeignKey("dict.AcademicDegree", t => t.AcademicDegreeId)
                .ForeignKey("dict.AcademicStatus", t => t.AcademicStatusId)
                .Index(t => t.AcademicDegreeId)
                .Index(t => t.AcademicStatusId);
            
            CreateTable(
                "dict.AcademicStatus",
                c => new
                    {
                        AcademicStatusId = c.Int(nullable: false, identity: true),
                        AcademicStatusGuid = c.Guid(nullable: false),
                        AcademicStatusName = c.String(nullable: false, maxLength: 50),
                        AcademicStatusShortName = c.String(maxLength: 50),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.AcademicStatusId);
            
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
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmployeeId)
                .ForeignKey("dbo.Person", t => t.PersonId)
                .Index(t => t.PersonId);
            
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
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
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
                "dbo.Chair",
                c => new
                    {
                        ChairId = c.Int(nullable: false, identity: true),
                        ChairGuid = c.Guid(),
                        FacultyId = c.Int(nullable: false),
                        DivisionCode = c.String(maxLength: 20),
                        DivisionName = c.String(nullable: false, maxLength: 200),
                        ParentId = c.Guid(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ChairId)
                .ForeignKey("dbo.Faculty", t => t.FacultyId)
                .Index(t => t.FacultyId);
            
            CreateTable(
                "dbo.Auditorium",
                c => new
                    {
                        AuditoriumId = c.Int(nullable: false, identity: true),
                        AuditoriumGuid = c.Guid(nullable: false),
                        AuditoriumNumber = c.String(),
                        HousingId = c.Int(nullable: false),
                        AuditoriumTypeId = c.Int(nullable: false),
                        Places = c.Int(),
                        ChairId = c.Int(),
                        Comment = c.String(maxLength: 100),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.AuditoriumId)
                .ForeignKey("dict.AuditoriumType", t => t.AuditoriumTypeId)
                .ForeignKey("dbo.Chair", t => t.ChairId)
                .ForeignKey("dict.Housing", t => t.HousingId)
                .Index(t => t.HousingId)
                .Index(t => t.AuditoriumTypeId)
                .Index(t => t.ChairId);
            
            CreateTable(
                "dict.AuditoriumType",
                c => new
                    {
                        AuditoriumTypeId = c.Int(nullable: false, identity: true),
                        AuditoriumTypeName = c.String(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.AuditoriumTypeId);
            
            CreateTable(
                "dict.Housing",
                c => new
                    {
                        HousingId = c.Int(nullable: false, identity: true),
                        HousingName = c.String(maxLength: 500),
                        Abbreviation = c.String(maxLength: 20),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.HousingId);
            
            CreateTable(
                "dbo.Lesson",
                c => new
                    {
                        LessonId = c.Int(nullable: false, identity: true),
                        LessonGuid = c.Guid(),
                        ScheduleId = c.Int(nullable: false),
                        LessonTypeId = c.Int(),
                        DisciplineId = c.Int(nullable: false),
                        AuditoriumId = c.Int(nullable: false),
                        PlannedChairJobId = c.Int(),
                        Order = c.Int(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.LessonId)
                .ForeignKey("dbo.Auditorium", t => t.AuditoriumId)
                .ForeignKey("dbo.Schedule", t => t.ScheduleId)
                .ForeignKey("dbo.Discipline", t => t.DisciplineId)
                .ForeignKey("dict.LessonType", t => t.LessonTypeId)
                .ForeignKey("dbo.PlannedChairJob", t => t.PlannedChairJobId)
                .Index(t => t.ScheduleId)
                .Index(t => t.LessonTypeId)
                .Index(t => t.DisciplineId)
                .Index(t => t.AuditoriumId)
                .Index(t => t.PlannedChairJobId);
            
            CreateTable(
                "dbo.Discipline",
                c => new
                    {
                        DisciplineId = c.Int(nullable: false, identity: true),
                        DisciplineGuid = c.Guid(),
                        DisciplineNameId = c.Int(nullable: false),
                        BaseProgramOfEducationId = c.Int(),
                        EducationSemesterId = c.Int(nullable: false),
                        CombinedWithDisciplineId = c.Int(),
                        FormControlExam = c.Boolean(),
                        FormControlCredit = c.Boolean(),
                        FormControlCreditWithGrade = c.Boolean(),
                        FormControlCourseProject = c.Boolean(),
                        FormControlCourseWork = c.Boolean(),
                        FormControlControlWork = c.Boolean(),
                        FormControlEssay = c.Boolean(),
                        ChairId = c.Int(nullable: false),
                        IsExludedFromCalculation = c.Boolean(nullable: false),
                        IsFakeDiscipline = c.Boolean(nullable: false),
                        IsHeadOfSectionDiscipline = c.Boolean(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.DisciplineId)
                .ForeignKey("dbo.BaseProgramOfEducation", t => t.BaseProgramOfEducationId)
                .ForeignKey("dbo.Chair", t => t.ChairId)
                .ForeignKey("dbo.Discipline", t => t.CombinedWithDisciplineId)
                .ForeignKey("dict.DisciplineName", t => t.DisciplineNameId)
                .ForeignKey("dict.EducationSemester", t => t.EducationSemesterId)
                .Index(t => t.DisciplineNameId)
                .Index(t => t.BaseProgramOfEducationId)
                .Index(t => t.EducationSemesterId)
                .Index(t => t.CombinedWithDisciplineId)
                .Index(t => t.ChairId);
            
            CreateTable(
                "dbo.BaseProgramOfEducation",
                c => new
                    {
                        BaseProgramOfEducationId = c.Int(nullable: false, identity: true),
                        BaseProgramOfEducationGuid = c.Guid(),
                        FederalEducationalStandardId = c.Int(nullable: false),
                        Name = c.String(maxLength: 1024),
                        NumberOfMonths = c.Int(),
                        EducationDirectionId = c.Int(nullable: false),
                        EducationProfileId = c.Int(nullable: false),
                        EducationFormId = c.Int(nullable: false),
                        QualificationId = c.Int(nullable: false),
                        EducationLevelId = c.Int(nullable: false),
                        EducationYearStartId = c.Int(nullable: false),
                        BaseOfAccelerationId = c.Int(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.BaseProgramOfEducationId)
                .ForeignKey("dict.BaseOfAcceleration", t => t.BaseOfAccelerationId)
                .ForeignKey("dict.EducationDirection", t => t.EducationDirectionId)
                .ForeignKey("dict.EducationForm", t => t.EducationFormId)
                .ForeignKey("dict.EducationLevel", t => t.EducationLevelId)
                .ForeignKey("dict.EducationProfile", t => t.EducationProfileId)
                .ForeignKey("dict.EducationYear", t => t.EducationYearStartId)
                .ForeignKey("dict.FederalEducationalStandard", t => t.FederalEducationalStandardId)
                .ForeignKey("dict.Qualification", t => t.QualificationId)
                .Index(t => t.FederalEducationalStandardId)
                .Index(t => t.EducationDirectionId)
                .Index(t => t.EducationProfileId)
                .Index(t => t.EducationFormId)
                .Index(t => t.QualificationId)
                .Index(t => t.EducationLevelId)
                .Index(t => t.EducationYearStartId)
                .Index(t => t.BaseOfAccelerationId);
            
            CreateTable(
                "dict.BaseOfAcceleration",
                c => new
                    {
                        BaseOfAccelerationId = c.Int(nullable: false, identity: true),
                        BaseOfAccelerationGuid = c.Guid(),
                        BaseOfAccelerationName = c.String(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.BaseOfAccelerationId);
            
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
                        ResearchWorkWeeks = c.Int(nullable: false),
                        Schedule = c.String(),
                        BaseProgramOfEducationId = c.Int(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.CourseScheduleId)
                .ForeignKey("dbo.BaseProgramOfEducation", t => t.BaseProgramOfEducationId)
                .Index(t => t.BaseProgramOfEducationId);
            
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
                        ResearchWorkWeeks = c.Int(nullable: false),
                        Schedule = c.String(),
                        CourseScheduleId = c.Int(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.SemesterScheduleId)
                .ForeignKey("plan.CourseSchedule", t => t.CourseScheduleId)
                .Index(t => t.CourseScheduleId);
            
            CreateTable(
                "dict.EducationDirection",
                c => new
                    {
                        EducationDirectionId = c.Int(nullable: false, identity: true),
                        EducationDirectionGuid = c.Guid(),
                        EducationDirectionCode = c.String(nullable: false, maxLength: 20),
                        EducationDirectionName = c.String(nullable: false, maxLength: 200),
                        QualificationId = c.Int(nullable: false),
                        EducationDirectionFGOSVO = c.String(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationDirectionId)
                .ForeignKey("dict.Qualification", t => t.QualificationId)
                .Index(t => t.QualificationId);
            
            CreateTable(
                "dict.EducationProfile",
                c => new
                    {
                        EducationProfileId = c.Int(nullable: false, identity: true),
                        EducationProfileGuid = c.Guid(),
                        EducationDirectionId = c.Int(nullable: false),
                        EducationProfileName = c.String(nullable: false, maxLength: 200),
                        IsCorrectProfile = c.Boolean(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationProfileId)
                .ForeignKey("dict.EducationDirection", t => t.EducationDirectionId)
                .Index(t => t.EducationDirectionId);
            
            CreateTable(
                "dict.Qualification",
                c => new
                    {
                        QualificationId = c.Int(nullable: false, identity: true),
                        QualificationGuid = c.Guid(),
                        QualificationCode = c.String(nullable: false, maxLength: 3),
                        QualificationName = c.String(nullable: false, maxLength: 100),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.QualificationId);
            
            CreateTable(
                "dict.EducationForm",
                c => new
                    {
                        EducationFormId = c.Int(nullable: false, identity: true),
                        EducationFormGuid = c.Guid(),
                        EducationFormName = c.String(nullable: false, maxLength: 50),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationFormId);
            
            CreateTable(
                "dict.EducationLevel",
                c => new
                    {
                        EducationLevelId = c.Int(nullable: false, identity: true),
                        EducationLevelGuid = c.Guid(),
                        EducationLevelName = c.String(nullable: false, maxLength: 50),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationLevelId);
            
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
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationYearId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        WeekNumber = c.Int(),
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
                .ForeignKey("dict.EducationYear", t => t.EducationYearId)
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
                "dbo.Faculty",
                c => new
                    {
                        FacultyId = c.Int(nullable: false, identity: true),
                        FacultyGuid = c.Guid(),
                        DivisionCode = c.String(maxLength: 20),
                        DivisionName = c.String(maxLength: 200),
                        ParentId = c.Guid(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.FacultyId);
            
            CreateTable(
                "dbo.Course",
                c => new
                    {
                        CourseId = c.Int(nullable: false, identity: true),
                        CourseGuid = c.Guid(),
                        FacultyId = c.Int(nullable: false),
                        CourseName = c.String(),
                        CourseNumber = c.Int(),
                        YearStart = c.Int(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.CourseId)
                .ForeignKey("dbo.Faculty", t => t.FacultyId)
                .Index(t => t.FacultyId);
            
            CreateTable(
                "dbo.Group",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        GroupGuid = c.Guid(),
                        CourseId = c.Int(nullable: false),
                        GroupCode = c.String(maxLength: 20),
                        GroupName = c.String(nullable: false, maxLength: 200),
                        ParentGuid = c.Guid(),
                        NumberOfStudents = c.Int(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                        BaseProgramOfEducationId = c.Int(),
                    })
                .PrimaryKey(t => t.GroupId)
                .ForeignKey("dbo.BaseProgramOfEducation", t => t.BaseProgramOfEducationId)
                .ForeignKey("dbo.Course", t => t.CourseId)
                .Index(t => t.CourseId)
                .Index(t => t.BaseProgramOfEducationId);
            
            CreateTable(
                "dbo.GroupFlowBinding",
                c => new
                    {
                        GroupFlowBindingId = c.Int(nullable: false, identity: true),
                        GroupFlowBindingGuid = c.Guid(nullable: false),
                        DisciplineId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                        FlowNumber = c.Int(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.GroupFlowBindingId)
                .ForeignKey("dbo.Discipline", t => t.DisciplineId)
                .ForeignKey("dbo.Group", t => t.GroupId)
                .Index(t => t.DisciplineId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.GroupSetGroup",
                c => new
                    {
                        GroupSetId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.GroupSetId, t.GroupId })
                .ForeignKey("dbo.Group", t => t.GroupId)
                .ForeignKey("dbo.GroupSet", t => t.GroupSetId)
                .Index(t => t.GroupSetId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.GroupSet",
                c => new
                    {
                        GroupSetId = c.Int(nullable: false, identity: true),
                        GroupSetName = c.String(),
                        ApplicationUserId = c.String(maxLength: 128),
                        IsSelected = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.GroupSetId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.GroupSubgroups",
                c => new
                    {
                        GroupSubgroupsId = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        DisciplineId = c.Int(nullable: false),
                        NumberOfSubgroups = c.Int(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.GroupSubgroupsId)
                .ForeignKey("dbo.Discipline", t => t.DisciplineId)
                .ForeignKey("dbo.Group", t => t.GroupId)
                .Index(t => t.GroupId)
                .Index(t => t.DisciplineId);
            
            CreateTable(
                "dbo.Schedule",
                c => new
                    {
                        ScheduleId = c.Int(nullable: false, identity: true),
                        ScheduleGuid = c.Guid(),
                        WeekNumber = c.Int(nullable: false),
                        DayNumber = c.Int(nullable: false),
                        ClassNumber = c.Int(nullable: false),
                        ClassDate = c.DateTime(nullable: false),
                        GroupId = c.Int(nullable: false),
                        EducationYearId = c.Int(),
                        IsNotActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ScheduleId)
                .ForeignKey("dict.ClassTime", t => new { t.DayNumber, t.ClassNumber })
                .ForeignKey("dict.EducationYear", t => t.EducationYearId)
                .ForeignKey("dbo.Group", t => t.GroupId)
                .Index(t => new { t.DayNumber, t.ClassNumber })
                .Index(t => t.GroupId)
                .Index(t => t.EducationYearId);
            
            CreateTable(
                "dict.ClassTime",
                c => new
                    {
                        DayNumber = c.Int(nullable: false),
                        ClassNumber = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.DayNumber, t.ClassNumber });
            
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
                "dict.EducationSemester",
                c => new
                    {
                        EducationSemesterId = c.Int(nullable: false, identity: true),
                        EducationSemesterGuid = c.Guid(),
                        EducationSemesterName = c.String(maxLength: 256),
                        EducationSemesterNumber = c.Int(nullable: false),
                        EducationYearId = c.Int(nullable: false),
                        EducationSemesterStart = c.DateTime(nullable: false),
                        EducationSemesterEnd = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EducationSemesterId)
                .ForeignKey("dict.EducationYear", t => t.EducationYearId)
                .Index(t => t.EducationYearId);
            
            CreateTable(
                "dict.FederalEducationalStandard",
                c => new
                    {
                        FederalEducationalStandardId = c.Int(nullable: false, identity: true),
                        FederalEducationalStandardGuid = c.Guid(),
                        Name = c.String(maxLength: 512),
                        Version = c.String(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.FederalEducationalStandardId);
            
            CreateTable(
                "dict.DisciplineName",
                c => new
                    {
                        DisciplineNameId = c.Int(nullable: false, identity: true),
                        DisciplineNameGuid = c.Guid(),
                        Name = c.String(nullable: false, maxLength: 1024),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.DisciplineNameId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dict.LessonType",
                c => new
                    {
                        LessonTypeId = c.Int(nullable: false, identity: true),
                        LessonTypeGuid = c.Guid(nullable: false),
                        LessonTypeName = c.String(),
                        Order = c.Int(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.LessonTypeId);
            
            CreateTable(
                "dbo.PlannedChairJob",
                c => new
                    {
                        PlannedChairJobId = c.Int(nullable: false, identity: true),
                        PlannedChairJobComment = c.String(),
                        EducationYearId = c.Int(nullable: false),
                        ChairId = c.Int(nullable: false),
                        PositionRealId = c.Int(nullable: false),
                        EmploymentTypeId = c.Int(nullable: false),
                        AcademicDegreeId = c.Int(),
                        JobId = c.Int(),
                        IsHeadOfChair = c.Boolean(nullable: false),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.PlannedChairJobId)
                .ForeignKey("dict.AcademicDegree", t => t.AcademicDegreeId)
                .ForeignKey("dbo.Chair", t => t.ChairId)
                .ForeignKey("dict.EducationYear", t => t.EducationYearId)
                .ForeignKey("dict.EmploymentType", t => t.EmploymentTypeId)
                .ForeignKey("dbo.Job", t => t.JobId)
                .ForeignKey("dict.PositionReal", t => t.PositionRealId)
                .Index(t => t.EducationYearId)
                .Index(t => t.ChairId)
                .Index(t => t.PositionRealId)
                .Index(t => t.EmploymentTypeId)
                .Index(t => t.AcademicDegreeId)
                .Index(t => t.JobId);
            
            CreateTable(
                "dict.EmploymentType",
                c => new
                    {
                        EmploymentTypeId = c.Int(nullable: false, identity: true),
                        EmploymentTypeGuid = c.Guid(),
                        EmploymentTypeName = c.String(nullable: false, maxLength: 50),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmploymentTypeId);
            
            CreateTable(
                "dict.PositionReal",
                c => new
                    {
                        PositionRealId = c.Int(nullable: false, identity: true),
                        PositionRealGuid = c.Guid(),
                        PositionName = c.String(nullable: false, maxLength: 100),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.PositionRealId);
            
            CreateTable(
                "dict.Position",
                c => new
                    {
                        PositionId = c.Int(nullable: false, identity: true),
                        PositionGuid = c.Guid(),
                        PositionCode = c.String(maxLength: 20),
                        PositionName = c.String(nullable: false, maxLength: 100),
                        PositionRealId = c.Int(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.PositionId)
                .ForeignKey("dict.PositionReal", t => t.PositionRealId)
                .Index(t => t.PositionRealId);
            
            CreateTable(
                "serv.LogEntry",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
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
                "dbo.AspNetUserFaculties",
                c => new
                    {
                        FacultyId = c.Int(nullable: false),
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.FacultyId, t.Id })
                .ForeignKey("dbo.Faculty", t => t.FacultyId)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.FacultyId)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Employee", "PersonId", "dbo.Person");
            DropForeignKey("dbo.Job", "PositionId", "dict.Position");
            DropForeignKey("dbo.Job", "EmploymentTypeId", "dict.EmploymentType");
            DropForeignKey("dbo.Job", "EmployeeId", "dbo.Employee");
            DropForeignKey("dbo.Job", "ChairId", "dbo.Chair");
            DropForeignKey("dbo.Chair", "FacultyId", "dbo.Faculty");
            DropForeignKey("dict.Position", "PositionRealId", "dict.PositionReal");
            DropForeignKey("dbo.PlannedChairJob", "PositionRealId", "dict.PositionReal");
            DropForeignKey("dbo.Lesson", "PlannedChairJobId", "dbo.PlannedChairJob");
            DropForeignKey("dbo.PlannedChairJob", "JobId", "dbo.Job");
            DropForeignKey("dbo.PlannedChairJob", "EmploymentTypeId", "dict.EmploymentType");
            DropForeignKey("dbo.PlannedChairJob", "EducationYearId", "dict.EducationYear");
            DropForeignKey("dbo.PlannedChairJob", "ChairId", "dbo.Chair");
            DropForeignKey("dbo.PlannedChairJob", "AcademicDegreeId", "dict.AcademicDegree");
            DropForeignKey("dbo.Lesson", "LessonTypeId", "dict.LessonType");
            DropForeignKey("dbo.Lesson", "DisciplineId", "dbo.Discipline");
            DropForeignKey("dbo.Discipline", "EducationSemesterId", "dict.EducationSemester");
            DropForeignKey("dbo.Discipline", "DisciplineNameId", "dict.DisciplineName");
            DropForeignKey("dbo.Discipline", "CombinedWithDisciplineId", "dbo.Discipline");
            DropForeignKey("dbo.Discipline", "ChairId", "dbo.Chair");
            DropForeignKey("dbo.Discipline", "BaseProgramOfEducationId", "dbo.BaseProgramOfEducation");
            DropForeignKey("dbo.BaseProgramOfEducation", "QualificationId", "dict.Qualification");
            DropForeignKey("dbo.BaseProgramOfEducation", "FederalEducationalStandardId", "dict.FederalEducationalStandard");
            DropForeignKey("dict.EducationSemester", "EducationYearId", "dict.EducationYear");
            DropForeignKey("dbo.BaseProgramOfEducation", "EducationYearStartId", "dict.EducationYear");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Lesson", "ScheduleId", "dbo.Schedule");
            DropForeignKey("dbo.Schedule", "GroupId", "dbo.Group");
            DropForeignKey("dbo.Schedule", "EducationYearId", "dict.EducationYear");
            DropForeignKey("dbo.Schedule", new[] { "DayNumber", "ClassNumber" }, "dict.ClassTime");
            DropForeignKey("dbo.GroupSubgroups", "GroupId", "dbo.Group");
            DropForeignKey("dbo.GroupSubgroups", "DisciplineId", "dbo.Discipline");
            DropForeignKey("dbo.GroupSetGroup", "GroupSetId", "dbo.GroupSet");
            DropForeignKey("dbo.GroupSet", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GroupSetGroup", "GroupId", "dbo.Group");
            DropForeignKey("dbo.GroupFlowBinding", "GroupId", "dbo.Group");
            DropForeignKey("dbo.GroupFlowBinding", "DisciplineId", "dbo.Discipline");
            DropForeignKey("dbo.Group", "CourseId", "dbo.Course");
            DropForeignKey("dbo.Group", "BaseProgramOfEducationId", "dbo.BaseProgramOfEducation");
            DropForeignKey("dbo.Course", "FacultyId", "dbo.Faculty");
            DropForeignKey("dbo.AspNetUserFaculties", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserFaculties", "FacultyId", "dbo.Faculty");
            DropForeignKey("dbo.AspNetUsers", "EducationYearId", "dict.EducationYear");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BaseProgramOfEducation", "EducationProfileId", "dict.EducationProfile");
            DropForeignKey("dbo.BaseProgramOfEducation", "EducationLevelId", "dict.EducationLevel");
            DropForeignKey("dbo.BaseProgramOfEducation", "EducationFormId", "dict.EducationForm");
            DropForeignKey("dbo.BaseProgramOfEducation", "EducationDirectionId", "dict.EducationDirection");
            DropForeignKey("dict.EducationDirection", "QualificationId", "dict.Qualification");
            DropForeignKey("dict.EducationProfile", "EducationDirectionId", "dict.EducationDirection");
            DropForeignKey("plan.SemesterSchedule", "CourseScheduleId", "plan.CourseSchedule");
            DropForeignKey("plan.CourseSchedule", "BaseProgramOfEducationId", "dbo.BaseProgramOfEducation");
            DropForeignKey("dbo.BaseProgramOfEducation", "BaseOfAccelerationId", "dict.BaseOfAcceleration");
            DropForeignKey("dbo.Lesson", "AuditoriumId", "dbo.Auditorium");
            DropForeignKey("dbo.Auditorium", "HousingId", "dict.Housing");
            DropForeignKey("dbo.Auditorium", "ChairId", "dbo.Chair");
            DropForeignKey("dbo.Auditorium", "AuditoriumTypeId", "dict.AuditoriumType");
            DropForeignKey("dbo.Person", "AcademicStatusId", "dict.AcademicStatus");
            DropForeignKey("dbo.Person", "AcademicDegreeId", "dict.AcademicDegree");
            DropForeignKey("dict.AcademicDegree", "AcademicDegreeLevelId", "dict.AcademicDegreeLevel");
            DropIndex("dbo.AspNetUserFaculties", new[] { "Id" });
            DropIndex("dbo.AspNetUserFaculties", new[] { "FacultyId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dict.Position", new[] { "PositionRealId" });
            DropIndex("dbo.PlannedChairJob", new[] { "JobId" });
            DropIndex("dbo.PlannedChairJob", new[] { "AcademicDegreeId" });
            DropIndex("dbo.PlannedChairJob", new[] { "EmploymentTypeId" });
            DropIndex("dbo.PlannedChairJob", new[] { "PositionRealId" });
            DropIndex("dbo.PlannedChairJob", new[] { "ChairId" });
            DropIndex("dbo.PlannedChairJob", new[] { "EducationYearId" });
            DropIndex("dict.DisciplineName", new[] { "Name" });
            DropIndex("dict.EducationSemester", new[] { "EducationYearId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Schedule", new[] { "EducationYearId" });
            DropIndex("dbo.Schedule", new[] { "GroupId" });
            DropIndex("dbo.Schedule", new[] { "DayNumber", "ClassNumber" });
            DropIndex("dbo.GroupSubgroups", new[] { "DisciplineId" });
            DropIndex("dbo.GroupSubgroups", new[] { "GroupId" });
            DropIndex("dbo.GroupSet", new[] { "ApplicationUserId" });
            DropIndex("dbo.GroupSetGroup", new[] { "GroupId" });
            DropIndex("dbo.GroupSetGroup", new[] { "GroupSetId" });
            DropIndex("dbo.GroupFlowBinding", new[] { "GroupId" });
            DropIndex("dbo.GroupFlowBinding", new[] { "DisciplineId" });
            DropIndex("dbo.Group", new[] { "BaseProgramOfEducationId" });
            DropIndex("dbo.Group", new[] { "CourseId" });
            DropIndex("dbo.Course", new[] { "FacultyId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "EducationYearId" });
            DropIndex("dict.EducationProfile", new[] { "EducationDirectionId" });
            DropIndex("dict.EducationDirection", new[] { "QualificationId" });
            DropIndex("plan.SemesterSchedule", new[] { "CourseScheduleId" });
            DropIndex("plan.CourseSchedule", new[] { "BaseProgramOfEducationId" });
            DropIndex("dbo.BaseProgramOfEducation", new[] { "BaseOfAccelerationId" });
            DropIndex("dbo.BaseProgramOfEducation", new[] { "EducationYearStartId" });
            DropIndex("dbo.BaseProgramOfEducation", new[] { "EducationLevelId" });
            DropIndex("dbo.BaseProgramOfEducation", new[] { "QualificationId" });
            DropIndex("dbo.BaseProgramOfEducation", new[] { "EducationFormId" });
            DropIndex("dbo.BaseProgramOfEducation", new[] { "EducationProfileId" });
            DropIndex("dbo.BaseProgramOfEducation", new[] { "EducationDirectionId" });
            DropIndex("dbo.BaseProgramOfEducation", new[] { "FederalEducationalStandardId" });
            DropIndex("dbo.Discipline", new[] { "ChairId" });
            DropIndex("dbo.Discipline", new[] { "CombinedWithDisciplineId" });
            DropIndex("dbo.Discipline", new[] { "EducationSemesterId" });
            DropIndex("dbo.Discipline", new[] { "BaseProgramOfEducationId" });
            DropIndex("dbo.Discipline", new[] { "DisciplineNameId" });
            DropIndex("dbo.Lesson", new[] { "PlannedChairJobId" });
            DropIndex("dbo.Lesson", new[] { "AuditoriumId" });
            DropIndex("dbo.Lesson", new[] { "DisciplineId" });
            DropIndex("dbo.Lesson", new[] { "LessonTypeId" });
            DropIndex("dbo.Lesson", new[] { "ScheduleId" });
            DropIndex("dbo.Auditorium", new[] { "ChairId" });
            DropIndex("dbo.Auditorium", new[] { "AuditoriumTypeId" });
            DropIndex("dbo.Auditorium", new[] { "HousingId" });
            DropIndex("dbo.Chair", new[] { "FacultyId" });
            DropIndex("dbo.Job", new[] { "EmploymentTypeId" });
            DropIndex("dbo.Job", new[] { "EmployeeId" });
            DropIndex("dbo.Job", new[] { "PositionId" });
            DropIndex("dbo.Job", new[] { "ChairId" });
            DropIndex("dbo.Employee", new[] { "PersonId" });
            DropIndex("dbo.Person", new[] { "AcademicStatusId" });
            DropIndex("dbo.Person", new[] { "AcademicDegreeId" });
            DropIndex("dict.AcademicDegree", new[] { "AcademicDegreeLevelId" });
            DropTable("dbo.AspNetUserFaculties");
            DropTable("dbo.AspNetRoles");
            DropTable("serv.LogEntry");
            DropTable("dict.Position");
            DropTable("dict.PositionReal");
            DropTable("dict.EmploymentType");
            DropTable("dbo.PlannedChairJob");
            DropTable("dict.LessonType");
            DropTable("dict.DisciplineName");
            DropTable("dict.FederalEducationalStandard");
            DropTable("dict.EducationSemester");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dict.ClassTime");
            DropTable("dbo.Schedule");
            DropTable("dbo.GroupSubgroups");
            DropTable("dbo.GroupSet");
            DropTable("dbo.GroupSetGroup");
            DropTable("dbo.GroupFlowBinding");
            DropTable("dbo.Group");
            DropTable("dbo.Course");
            DropTable("dbo.Faculty");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dict.EducationYear");
            DropTable("dict.EducationLevel");
            DropTable("dict.EducationForm");
            DropTable("dict.Qualification");
            DropTable("dict.EducationProfile");
            DropTable("dict.EducationDirection");
            DropTable("plan.SemesterSchedule");
            DropTable("plan.CourseSchedule");
            DropTable("dict.BaseOfAcceleration");
            DropTable("dbo.BaseProgramOfEducation");
            DropTable("dbo.Discipline");
            DropTable("dbo.Lesson");
            DropTable("dict.Housing");
            DropTable("dict.AuditoriumType");
            DropTable("dbo.Auditorium");
            DropTable("dbo.Chair");
            DropTable("dbo.Job");
            DropTable("dbo.Employee");
            DropTable("dict.AcademicStatus");
            DropTable("dbo.Person");
            DropTable("dict.AcademicDegree");
            DropTable("dict.AcademicDegreeLevel");
        }
    }
}
