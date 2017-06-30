using System.Collections.Generic;
using System.Linq;
using ClassSchedule.Business.Interfaces;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Business.Services
{
    public class HousingService : IHousingService
    {
        private readonly ApplicationDbContext _context;

        public HousingService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Наименования корпусов одинаковой длинны
        /// Используется для выпадающих списков, чтобы выстроить всё ровно в несколько колонок
        /// </summary>
        public List<HousingQueryResult> HousingEqualLength()
        {
            var query = @"
                DECLARE @MaxLength INT;

                SELECT @MaxLength = (SELECT MAX(LEN(h1.HousingName))
                    FROM dict.Housing h1);

                SELECT h2.HousingId, h2.Abbreviation,
                   LEFT(h2.HousingName + space(@maxLength), @MaxLength + 10) AS HousingName
                FROM dict.Housing h2;";
            var housings = _context.Database.SqlQuery<HousingQueryResult>(query).ToList();

            return housings;
        }
    }
}
