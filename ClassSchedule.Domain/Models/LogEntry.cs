using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassSchedule.Domain.Models
{
    [Table("LogEntry", Schema = "serv")]
    public class LogEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string ClassMethod { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public string RequestUri { get; set; }
        public string RemoteAddress { get; set; }
        public string UserAgent { get; set; }
        public string Exception { get; set; }               
    }
}
