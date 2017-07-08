﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ClassSchedule.Business.Interfaces;
using ClassSchedule.Business.Models;
using ClassSchedule.Domain.Context;

namespace ClassSchedule.Business.Services
{
    public class DictionaryService : IDictionaryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJobService _jobService;
        private readonly IHousingService _housingService;
        private readonly IAuditoriumService _auditoriumService;

        public DictionaryService(ApplicationDbContext context, IHousingService housingService
            , IAuditoriumService auditoriumService, IJobService jobService)
        {
            _context = context;
            _housingService = housingService;
            _auditoriumService = auditoriumService;
            _jobService = jobService;
        }

        public List<CourseViewModel> GetCourses(int? facultyId)
        {
            var courses = _context.Courses
                .Where(x => x.DeletedAt == null && x.FacultyId == facultyId && x.YearStart != null
                    /*&& x.YearStart + x.CourseNumber == UserProfile.EducationYear.YearEnd*/)
                .OrderBy(n => n.CourseName)
                .Select(x => new CourseViewModel { CourseId = x.CourseId, CourseName = x.CourseName })
                .ToList();

            return courses;
        }

        public List<int> GetCourseNumbers(int facultyId, int? educationFormId, int? educationLevelId)
        {
            var courses = _context.Courses
                .Include(x => x.Groups.Select(g => g.BaseProgramOfEducation))
                .Where(x => x.DeletedAt == null && x.FacultyId == facultyId && x.YearStart != null
                    /*&& x.YearStart + x.CourseNumber == UserProfile.EducationYear.YearEnd*/);

            if (educationFormId != null)
            {
                courses = courses.Where(x => x.Groups.Any(g => g.BaseProgramOfEducation.EducationFormId == educationFormId && g.DeletedAt == null));
            }

            if (educationLevelId != null)
            {
                courses = courses.Where(x => x.Groups.Any(g => g.BaseProgramOfEducation.EducationLevelId == educationLevelId && g.DeletedAt == null));
            }

            var courseNumbers = courses
                .Where(x => x.CourseNumber != null)
                .OrderBy(n => n.CourseNumber)
                .Select(x => x.CourseNumber ?? 0)
                .Distinct()
                .ToList();

            return courseNumbers;
        }

        public List<GroupViewModel> GetGroups(int facultyId, int? courseId, int? educationFormId
            , int? educationLevelId, int? courseNumber, int? yearStart = null)
        {
            var groups = _context.Groups
                   .Where(x => x.DeletedAt == null && x.Course.FacultyId == facultyId);

            if (courseId != null)
            {
                groups = groups.Where(x => x.CourseId == courseId);
            }

            if (educationFormId != null)
            {
                groups = groups.Where(g => g.BaseProgramOfEducation.EducationFormId == educationFormId);
            }

            if (educationLevelId != null)
            {
                groups = groups.Where(g => g.BaseProgramOfEducation.EducationLevelId == educationLevelId);
            }

            if (courseNumber != null && yearStart != null)
            {
                groups = groups.Where(g => yearStart - g.Course.YearStart + 1 == courseNumber);
            }

            var result = groups
                .OrderBy(n => n.GroupName)
                .Select(x => new GroupViewModel { GroupId = x.GroupId, GroupName = x.GroupName })
                .ToList();

            return result;
        }

        public List<EducationFormViewModel> GetEducationForms()
        {
            var forms = _context.EducationForms
                .Where(x => x.DeletedAt == null && !x.IsNotUsed)
                .OrderBy(n => n.EducationFormName)
                .Select(x => new EducationFormViewModel { EducationFormId = x.EducationFormId, EducationFormName = x.EducationFormName })
                .ToList();

            return forms;
        }

        public List<EducationLevelViewModel> GetEducationLevels()
        {
            var levels = _context.EducationLevels
                .Where(x => x.DeletedAt == null && !x.IsNotUsed)
                .OrderBy(n => n.EducationLevelId)
                .Select(x => new EducationLevelViewModel { EducationLevelId = x.EducationLevelId, EducationLevelName = x.EducationLevelName })
                .ToList();

            return levels;
        }

        public List<EducationDirectionViewModel> GetEducationDirections()
        {
            var directions = _context.EducationDirections
                .Where(x => x.DeletedAt == null)
                .OrderBy(n => n.EducationDirectionCode)
                .Select(x => new EducationDirectionViewModel
                    {
                        EducationDirectionId = x.EducationDirectionId,
                        EducationDirectionName = x.EducationDirectionCode + " " + x.EducationDirectionName
                    })
                .ToList();

            return directions;
        }

        public List<EducationProfileViewModel> GetEducationProfiles(int educationFormId, int educationDirectionId, int yearStart)
        {
            var profiles = _context.BaseProgramsOfEducation
                .Where(x => x.DeletedAt == null && x.EducationFormId == educationFormId
                    && x.EducationProfile.EducationDirectionId == educationDirectionId
                    && x.Groups.Any(g => g.Course.YearStart == yearStart))
                .Select(x => new EducationProfileViewModel
                {
                    // ProgramOfEducationId = x.BaseProgramOfEducationId,
                    EducationProfileId = x.EducationProfileId,
                    // EducationLevelId = x.EducationLevelId,
                    EducationProfileName =
                        x.EducationProfile.EducationDirection.EducationDirectionCode + " " +
                        x.EducationProfile.EducationProfileName + (x.EducationLevelId == 2 ? " (прикладной бакалавриат)" : "")
                })
                // .OrderBy(n => n.EducationLevelId)
                // .ThenBy(x => x.EducationProfileName)
                .ToList();

            return profiles;
        }

        public List<DisciplineViewModel> GetDisciplines(int groupId, int? chairId = null, int? educationYearId = null)
        {
            var disciplines = _context.Disciplines
                .Include(x => x.DisciplineName)
                .Include(x => x.EducationSemester)
                .Where(x => x.DeletedAt == null);

            var group = _context.Groups
                .Include(x => x.BaseProgramOfEducation)
                .SingleOrDefault(x => x.GroupId == groupId && x.DeletedAt == null);
            if (group != null)
            {
                disciplines = disciplines.Where(x => x.BaseProgramOfEducationId == group.BaseProgramOfEducationId);
            }

            if (chairId != null)
            {
                disciplines = disciplines.Where(d => d.ChairId == chairId);
            }

            if (educationYearId != null)
            {
                disciplines = disciplines.Where(x => x.EducationSemester.EducationYearId == educationYearId);
            }

            var result = disciplines
                .OrderBy(x => x.DisciplineName.Name)
                .ThenBy(x => x.StudyLoadCalculationStringName)
                .ThenBy(x => x.EducationSemesterId)
                .Select(x => new DisciplineViewModel
                {
                    DisciplineId = x.DisciplineId,
                    DisciplineName = x.DisciplineName.Name,
                    ChairId = x.ChairId,
                    ChairName = x.Chair.DivisionName,
                    EducationSemesterInd = x.EducationSemesterId,
                    EducationSemesterNumber = x.EducationSemester.EducationSemesterNumber
                })
                .ToList();

            return result;
        }

        public List<TeacherViewModel> GetTeachers(int educationYearId, int? chairId, string query = null, int? take = null)
        {
            return _jobService.ActualTeachers(educationYearId, chairId, query, take);
        }

        public List<TeacherViewModel> GetTeacherWithEmployment(int educationYearId, int weekNumber, int dayNumber, int classNumber
            , int groupId, int? disciplineId, int? chairId = null)
        {
            return _jobService.ActualTeachersWithEmployment(educationYearId, weekNumber, dayNumber, classNumber, groupId, disciplineId, chairId);
        }

        public List<HousingViewModel> GetHousings()
        {
            var housings = _context.Housings
                .Select(x => new HousingViewModel
                {
                    HousingId = x.HousingId,
                    HousingName = x.HousingName,
                    Abbreviation = x.Abbreviation
                })
                .ToList();

            return housings;
        }

        public List<HousingViewModel> GetHousingEqualLength()
        {
            var housings = _housingService.HousingEqualLength();
            var result = housings
                .Select(x => new HousingViewModel
                {
                    HousingId = x.HousingId,
                    HousingName = x.HousingName,
                    Abbreviation = x.Abbreviation
                })
                .ToList();

            return result;
        }

        public List<AuditoriumViewModel> GetAuditoriums(int? chairId, int? housingId, string query, bool shortResult = false, int? take = null)
        {
            var auditoriums = _context.Auditoriums
                .Where(x => x.DeletedAt == null)
                .OrderByDescending(n => n.ChairId)
                .ThenBy(n => n.AuditoriumNumber)
                .Select(x => x);

            if (housingId != null)
            {
                auditoriums = auditoriums.Where(x => x.HousingId == housingId);
            }

            if (chairId != null)
            {
                auditoriums = auditoriums.Where(x => x.ChairId == chairId || x.ChairId == null);
            }

            if (query != null)
            {
                auditoriums = auditoriums.Where(x => x.AuditoriumNumber.StartsWith(query));
            }

            if (take != null && take != 0)
            {
                auditoriums = auditoriums.Take(take ?? 0);
            }

            List<AuditoriumViewModel> result;
            if (shortResult)
            {
                result = auditoriums
                    .Select(x => new AuditoriumViewModel
                    {
                        AuditoriumId = x.AuditoriumId,
                        AuditoriumNumber = x.AuditoriumNumber + x.Housing.Abbreviation,
                        AuditoriumTypeName = x.AuditoriumType.AuditoriumTypeName,
                        Places = x.Places ?? 0
                    })
                    .ToList();
            }
            else
            {
                result = auditoriums
                    .Select(x => new AuditoriumViewModel
                    {
                        AuditoriumId = x.AuditoriumId,
                        AuditoriumNumber = x.AuditoriumNumber,
                        AuditoriumTypeName = x.AuditoriumType.AuditoriumTypeName,
                        // HousingId = x.HousingId,
                        // HousingAbbreviation = x.Housing.Abbreviation,
                        Places = x.Places ?? 0
                    })
                    .ToList();
            }

            return result;
        }

        public List<AuditoriumViewModel> GetAuditoriumWithEmployment(int housingId, int weekNumber, int dayNumber, int classNumber, int groupId, int? chairId = null)
        {
            return _auditoriumService.AuditoriumWithEmployment(housingId, weekNumber, dayNumber, classNumber, groupId, chairId);
        }

        public List<LessonTypeViewModel> GetLessonTypes()
        {
            var lessonTypes = _context.LessonTypes
                .OrderBy(n => n.Order)
                .Select(x => new LessonTypeViewModel
                {
                    LessonTypeId = x.LessonTypeId,
                    LessonTypeName = x.LessonTypeName
                })
                .ToList();

            return lessonTypes;
        }

        public List<EducationYearViewModel> GetEducationYears()
        {
            var educationYears = _context.EducationYears
                .Where(x => x.DeletedAt == null)
                .Select(x => new EducationYearViewModel
                {
                    EducationYearId = x.EducationYearId,
                    EducationYearName = x.EducationYearName
                })
                .ToList();

            return educationYears;
        }

        public List<FacultyViewModel> GetFaculties(string userId = null)
        {
            var faculties = _context.Faculties
                .Where(f => f.DeletedAt == null);

            if (userId != null)
            {
                faculties = faculties.Where(x => x.ApplicationUsers.Any(u => u.Id == userId));
            }

            return faculties
                .OrderBy(n => n.DivisionName)
                .Select(x => new FacultyViewModel { FacultyId = x.FacultyId, FacultyName = x.DivisionName })
                .ToList();
        }
    }
}
