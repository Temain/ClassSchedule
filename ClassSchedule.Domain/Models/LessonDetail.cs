using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Информация о занятии, в каких аудитории(ях) проходит, какой(ие) преподаватели ведут
    /// </summary>
    [Table("LessonDetail", Schema = "dbo")]
    public class LessonDetail
    {
        public LessonDetail()
        {
            CreatedAt = DateTime.Now;
            LessonDetailGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public int LessonDetailId { get; set; }

        public Guid? LessonDetailGuid { get; set; }

        /// <summary>
        /// Занятие
        /// </summary>
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        /// <summary>
        /// Аудитория
        /// </summary>
        public int AuditoriumId { get; set; }
        public Auditorium Auditorium { get; set; }

        /// <summary>
        /// Преподаватель
        /// </summary>
        public int? PlannedChairJobId { get; set; }
        public PlannedChairJob PlannedChairJob { get; set; }

        /// <summary>
        /// Порядок
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата последнего обновления записи
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Дата удаления записи
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}
