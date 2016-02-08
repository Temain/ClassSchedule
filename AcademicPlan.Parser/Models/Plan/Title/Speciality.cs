using System.Xml.Serialization;

namespace AcademicPlan.Parser.Models.Plan.Title
{
    public class Speciality
    {
        [XmlAttribute("Ном")]
        public string Order { get; set; }

        [XmlAttribute("Название")]
        public string SpecialityName { get; set; }
    }
}
