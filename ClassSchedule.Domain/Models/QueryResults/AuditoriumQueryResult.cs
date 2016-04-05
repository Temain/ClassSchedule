using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models.QueryResults
{
    public class AuditoriumQueryResult
    {
        public int AuditoriumId { get; set; }
        public string AuditoriumNumber { get; set; }
        public string AuditoriumTypeName { get; set; }
        public int? ChairId { get; set; }
        public int Places { get; set; }
        public string Comment { get; set; }
        public string Employment { get; set; }

    }
}
