using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.Helpers;
using ClassSchedule.Domain.Models;
using ClassSchedule.Domain.Models.QueryResults;

namespace ClassSchedule.Domain.DataAccess.Repositories
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public List<KeyValueDictionary> ActualTeachers(EducationYear educationYear, int? chairId)
        {
            var parameters = new object[]
            {
                new SqlParameter("@chairId", chairId),
                new SqlParameter("@startDate", educationYear.DateStart),
                new SqlParameter("@endDate", educationYear.DateEnd)
            };

            var query = @"
                SELECT * 
                FROM (
                  SELECT j.JobId AS [Key], p.LastName + COALESCE(' ' + p.FirstName, '') + COALESCE(' ' + p.MiddleName, '') AS [Value], 
                    ROW_NUMBER() OVER(PARTITION BY e.PersonId ORDER BY j.JobDateStart DESC) as Rn 
                  FROM Job j 
                  LEFT JOIN Employee e ON j.EmployeeId = e.EmployeeId
                  LEFT JOIN Person p ON e.PersonId = p.PersonId
                  WHERE j.ChairId = @chairId 
                    AND (j.IsDeleted = 0 OR j.IsDeleted IS NULL)
                    AND (e.IsDeleted = 0 OR e.IsDeleted IS NULL)
                    AND (p.IsDeleted = 0 OR p.IsDeleted IS NULL)
  
                    -- Проверка что преподаватель работал в определенном учебном году
                    AND (
                      (j.JobDateEnd IS NULL AND (j.JobDateStart < @startDate OR (j.JobDateStart >= @startDate AND j.JobDateStart <= @endDate))) 
                      OR 
                      (j.JobDateEnd IS NOT NULL AND (j.JobDateStart < @startDate OR (j.JobDateStart >= @startDate AND j.JobDateStart <= @endDate))
                        AND (j.JobDateEnd IS NOT NULL AND ((j.JobDateEnd >= @startDate AND j.JobDateEnd <= @endDate) OR j.JobDateEnd > @endDate)))
                    )
                ) AS t0
                WHERE t0.Rn = 1;";
            var teachers = _context.Database.SqlQuery<KeyValueDictionary>(query, parameters).ToList();

            return teachers;
        }
    }
}
