namespace ClassSchedule.Business.Models.GroupInfo
{
    public class DisciplineSemesterPlanViewModel
    {
        public int SemesterNumber { get; set; }

        /// <summary>
        /// Часов лекций
        /// </summary>
        public int? HoursOfLectures { get; set; }

        /// <summary>
        /// Часов лабораторных
        /// </summary>
        public int? HoursOfLaboratory { get; set; }

        /// <summary>
        /// Часов практик
        /// </summary>
        public int HoursOfPracticeFilled { get; set; }

        /// <summary>
        /// Часов лекций
        /// </summary>
        public int HoursOfLecturesFilled { get; set; }

        /// <summary>
        /// Часов лабораторных
        /// </summary>
        public int HoursOfLaboratoryFilled { get; set; }

        /// <summary>
        /// Часов практик
        /// </summary>
        public int? HoursOfPractice { get; set; }

        /// <summary>
        /// Лекций в неделю
        /// </summary>
        public float? LecturesPerWeek { get; set; }

        /// <summary>
        /// Практик в неделю
        /// </summary>
        public float? PracticePerWeek { get; set; }

        /// <summary>
        /// Лабораторных в неделю
        /// </summary>
        public float? LaboratoryPerWeek { get; set; }
    }
}