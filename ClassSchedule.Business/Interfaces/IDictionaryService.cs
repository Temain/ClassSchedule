using System.Collections.Generic;
using ClassSchedule.Business.Models;

namespace ClassSchedule.Business.Interfaces
{
    public interface IDictionaryService
    {
        List<CourseViewModel> GetCourses(int? facultyId);
        List<int> GetCourseNumbers(int facultyId, int? educationFormId, int? educationLevelId);
        List<GroupViewModel> GetGroups(int facultyId, int? courseId, int? educationFormId
            , int? educationLevelId, int? courseNumber, int? yearStart = null);
        List<EducationFormViewModel> GetEducationForms();
        List<EducationLevelViewModel> GetEducationLevels();
        List<EducationDirectionViewModel> GetEducationDirections();
        List<EducationProfileViewModel> GetEducationProfiles(int educationFormId, int educationDirectionId, int yearStart);
        List<DisciplineViewModel> GetDisciplines(int groupId, int? chairId = null, int? educationYearId = null);
        List<TeacherViewModel> GetTeachers(int educationYearId, int? chairId, string query = null, int? take = null);
        List<TeacherViewModel> GetTeacherWithEmployment(int educationYearId, int weekNumber, int dayNumber, int classNumber, int groupId, int? disciplineId = null, int? chairId = null);
        List<HousingViewModel> GetHousings();
        List<HousingViewModel> GetHousingEqualLength();
        List<AuditoriumViewModel> GetAuditoriums(int? chairId, int? housingId, string query, bool shortResult = false, int? take = null);
        List<AuditoriumViewModel> GetAuditoriumWithEmployment(int housingId, int weekNumber, int dayNumber, int classNumber, int groupId, int? chairId = null);
        List<LessonTypeViewModel> GetLessonTypes();
        List<EducationYearViewModel> GetEducationYears();
        List<FacultyViewModel> GetFaculties(string userId = null);
        List<DayViewModel> GetDays();
    }
}
