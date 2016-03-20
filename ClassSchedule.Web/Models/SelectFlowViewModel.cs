using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models
{
    public class SelectFlowViewModel
    {
        [Required]
        [Display(Name = "Факультет")]
        public int FacultyId { get; set; }

        [Display(Name = "Курс")]
        public int? CourseId { get; set; }

        [Display(Name = "Группа")]
        public int? GroupId { get; set; }

        [Display(Name = "Поток")]
        public int? FlowId { get; set; }
    }
}