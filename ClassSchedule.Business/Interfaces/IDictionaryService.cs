using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        List<DisciplineViewModel> GetDisciplines(string query, int? chairId);
        List<TeacherViewModel> GetTeachers(int? chairId, string query);
        List<TeacherViewModel> GetTeacherWithEmployment(int chairId, int weekNumber, int dayNumber, int classNumber, int groupId);
        List<HousingViewModel> GetHousings();
        List<HousingViewModel> GetHousingEqualLength();
        List<AuditoriumViewModel> GetAuditoriums(int? chairId, int? housingId, string query, bool shortResult = false);
        List<AuditoriumViewModel> GetAuditoriumWithEmployment(int chairId, int housingId, int weekNumber, int dayNumber, int classNumber, int groupId);
        List<LessonTypeViewModel> GetLessonTypes();
    }
}
