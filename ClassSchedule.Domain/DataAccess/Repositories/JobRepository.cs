using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.Helpers;
using ClassSchedule.Domain.Models;

namespace ClassSchedule.Domain.DataAccess.Repositories
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        private readonly ApplicationDbContext _context;
        public JobRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Job> ActualTeachers(EducationYear educationYear, int? chairId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var parameters = new object[]
            {
                new SqlParameter("@chairId", chairId),
                new SqlParameter("@startDate", educationYear.DateStart),
                new SqlParameter("@endDate", educationYear.DateEnd)
            };

            var query = @"
                SELECT * 
                FROM (
                  SELECT j.*, ROW_NUMBER() OVER(PARTITION BY e.PersonId ORDER BY j.JobDateStart DESC) as Rn 
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
            var teachers = _context.Jobs.SqlQuery(query, parameters).ToList();


            //var teachers = _context.Jobs
            //    .Where(x => x.ChairId == chairId && x.IsDeleted != true
            //                && x.Employee.IsDeleted != true && x.Employee.Person.IsDeleted != true)
            //    .ToList()
            //    .Where(x => DateHelpers.DatesIsActual(educationYear, x.JobDateStart, x.JobDateEnd))
            //    .GroupBy(g => g.Employee.PersonId)
            //    .Select(x => new
            //    {
            //        Job = x.OrderByDescending(n => n.JobDateStart).FirstOrDefault()
            //    })
            //    .Select(x => x.Job)
            //    .ToList();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            return teachers;
        }
    }
}
