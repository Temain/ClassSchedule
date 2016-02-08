using System.Xml.Serialization;

namespace AcademicPlan.Parser.Models.Plan.Title
{
    public class Approval
    {
        [XmlAttribute("НомПротокола")]
        public string ProtocolNumber { get; set; }
    }
}
