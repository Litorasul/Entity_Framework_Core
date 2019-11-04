using System;
using System.Text;
using SoftUni.Data;
using System.Linq;


namespace SoftUni
{
    public class StartUp
    {
        public static void Main()
        {
            SoftUniContext context = new SoftUniContext();

            string result = GetEmployeesWithSalaryOver50000(context);

            Console.WriteLine(result);
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .OrderBy(e => e.FirstName)
                .Where(e => e.Salary > 50000)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                });

            foreach (var e in employees)
            {
                sb
                    .AppendLine($"{e.FirstName} - {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    Name = String.Join(" ", e.FirstName, e.LastName, e.MiddleName),
                    e.JobTitle,
                    e.Salary
                });

            foreach (var e in employees)
            {
                sb
                    .AppendLine($"{e.Name} {e.JobTitle} {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();

        }
    }
}
