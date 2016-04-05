using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models.QueryResults
{
    public class HousingQueryResult
    {
        public int HousingId { get; set; }
        public string HousingName { get; set; }
        public string Abbreviation { get; set; }
    }
}
