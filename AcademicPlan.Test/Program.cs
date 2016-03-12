using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlan.Parser.Models;
using AcademicPlan.Parser.Service;

namespace AcademicPlan.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "05.03.06_Экология_и_природопользование_2012-2015_Академ_бак.plm (180897 v1).XML";
            string xml = File.ReadAllText(fileName);
            var plan = xml.ParseXml<Document>();
        }
    }
}
