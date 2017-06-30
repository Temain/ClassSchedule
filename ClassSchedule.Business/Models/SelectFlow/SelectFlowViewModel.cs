using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassSchedule.Business.Models.SelectFlow
{
    public class SelectFlowViewModel
    {
        public int GroupSetId { get; set; }
        public string GroupSetName { get; set; }
        public ICollection<GroupSetViewModel> GroupSets { get; set; }
        
        [Required]        
        public int FacultyId { get; set; }
        public ICollection<FacultyViewModel> Faculties { get; set; } 


        public int EducationLevelId { get; set; }
        public ICollection<EducationLevelViewModel> EducationLevels { get; set; } 

        public int EducationFormId { get; set; }
        public ICollection<EducationFormViewModel> EducationForms { get; set; } 


        public int CourseNumber { get; set; }
        public ICollection<int> CourseNumbers { get; set; } 

        public ICollection<GroupViewModel> Groups { get; set; }
    }
}