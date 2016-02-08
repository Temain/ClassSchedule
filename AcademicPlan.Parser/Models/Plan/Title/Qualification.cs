using System.Xml.Serialization;

namespace AcademicPlan.Parser.Models.Plan.Title
{
    public class Qualification
    {
        [XmlAttribute("Ном")]
        public string Order { get; set; }

        [XmlAttribute("Название")]
        public string QualificationName { get; set; }

        [XmlAttribute("СрокОбучения")]
        public string TrainingPeriod { get; set; }
    }
}
