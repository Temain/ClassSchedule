using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ClassSchedule.Business.Exceptions
{
    public class DisciplineNotFoundException : Exception
    {
        public int? BaseProgramOfEducationId { get; set; }
        public int? EducationSemesterId { get; set; }
        public int? ChairId { get; set; }
        public int? DisciplineNameId { get; set; }
        public string StudyLoadCalculationStringName { get; set; }


        public DisciplineNotFoundException() : base() { }

        public DisciplineNotFoundException(string message) : base(message) { }

        public DisciplineNotFoundException(Exception inner) : base(string.Empty, inner) { }

        public DisciplineNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected DisciplineNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public DisciplineNotFoundException(int? baseProgramOfEducationId, int? educationSemesterId, int? chairId, int? disciplineNameId, string studyLoadCalculationStringName) : base()
        {
            this.BaseProgramOfEducationId = baseProgramOfEducationId;
            this.EducationSemesterId = educationSemesterId;
            this.ChairId = chairId;
            this.DisciplineNameId = disciplineNameId;
            this.StudyLoadCalculationStringName = studyLoadCalculationStringName;
        }

        public override string Message
        {
            get
            {
                return string.Format("Дисциплина не найдена. [BaseProgramOfEducationId: {0}, EducationSemesterId: {1}, ChairId: {2}, DisciplineNameId: {3}, StudyLoadCalculationStringName: {4}]");
            }
        }
    }
}
