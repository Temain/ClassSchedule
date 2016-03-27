using System.Xml.Serialization;

namespace AcademicPlan.Parser.Models.Plan.Title
{
    /// <summary>
    /// Разработчик плана
    /// </summary>
    public class Developer
    {
        [XmlAttribute("Ном")]
        public string Order { get; set; }

        [XmlAttribute("ФИО")]
        public string FullName { get; set; }

        [XmlAttribute("Должность")]
        public string Position { get; set; }

        [XmlAttribute("Активный")]
        public int IsActive { get; set; }
    }
}
