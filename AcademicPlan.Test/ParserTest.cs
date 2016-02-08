using System;
using System.IO;
using AcademicPlan.Parser.Models;
using AcademicPlan.Parser.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcademicPlan.Test
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void ShouldParseXml()
        {
            string fileName = "05.03.06_Экология_и_природопользование_2012-2015_Академ_бак (180897 v1).plm.XML";
            string xml = File.ReadAllText(fileName);
            var plan = xml.ParseXml<Document>();
        }
    }
}
