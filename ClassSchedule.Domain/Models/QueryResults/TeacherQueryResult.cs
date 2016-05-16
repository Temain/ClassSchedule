using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models.QueryResults
{
    public class TeacherQueryResult
    {
        public int PersonId { get; set; }
        public int JobId { get; set; }
        public string FullName { get; set; }
        public string Employment { get; set; }
    }
}
