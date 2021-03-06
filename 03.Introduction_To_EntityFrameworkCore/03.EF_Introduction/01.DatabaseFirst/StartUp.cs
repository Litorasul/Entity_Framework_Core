﻿using System;
using System.Globalization;
using System.Text;
using SoftUni.Data;
using System.Linq;
using SoftUni.Models;


namespace SoftUni
{
    public class StartUp
    {
        public static void Main()
        {
            SoftUniContext context = new SoftUniContext();

            string result = GetLatestProjects(context);

            Console.WriteLine(result);
        }

        //Problem 15
        public static string RemoveTown(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Address.Town.Name == "Seattle")
                .ToList();

            foreach (var employee in employees)
            {
                employee.AddressId = null;
            }

            var addresses = context.Addresses
                .Where(t => t.Town.Name == "Seattle");
            int count = addresses.Count();

            foreach (var address in addresses)
            {
                context.Addresses.Remove(address);
            }

            var towns = context.Towns
                .Where(t => t.Name == "Seattle");
            foreach (var town in towns)
            {
                context.Towns.Remove(town);
            }

            context.SaveChanges();
            return $"{count} addresses in Seattle were deleted";
        }

        //Problem 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectToRemove = context.Projects.Find(2);
            var targetEmployeeProject = context.EmployeesProjects.Where(ep => ep.ProjectId == 2);

            context.EmployeesProjects.RemoveRange(targetEmployeeProject);
            context.Projects.Remove(projectToRemove);
            context.SaveChanges();

            var projects = context
                .Projects
                .Select(p => new
                {
                    p.Name
                })
                .Take(10);
                

            StringBuilder sb = new StringBuilder();

            foreach (var project in projects)
            {
                sb.AppendLine(project.Name);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 13 
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary

                })
                .Where(e => e.FirstName.StartsWith("Sa"))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName);
                

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 12 
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design" ||
                            e.Department.Name == "Marketing" || e.Department.Name == "Information Services");
           

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12m;

            }
            context.SaveChanges();

            var employeesToPrint = employees
                .Select(e => new
                {
                    FullName = e.FirstName + " " + e.LastName,
                    Salary = e.Salary
                })
                .OrderBy(e => e.FullName);

            var sb = new StringBuilder();
            foreach (var employee in employeesToPrint)
            {
                sb.AppendLine($"{employee.FullName} (${employee.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();


            var projects = context
                .Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    startDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                })
                .OrderBy(p => p.Name);

            foreach (var p in projects)
            {
                sb.AppendLine(p.Name)
                    .AppendLine(p.Description)
                    .AppendLine(p.startDate);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context
                .Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    departmentName = d.Name,
                    managerFirstName = d.Manager.FirstName,
                    managerLastName = d.Manager.LastName.Length,
                    employees = d.Employees
                        .Select(e => new
                        {
                            e.FirstName,
                            e.LastName,
                            e.JobTitle
                        })
                        .OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                });

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.departmentName} - {d.managerFirstName}  {d.managerLastName}");
                foreach (var e in d.employees)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 09
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee147 = context
                .Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    projects = e.EmployeesProjects
                        .Select(p => new
                        {
                            projectName = p.Project.Name
                        })
                        .OrderBy(p => p.projectName)
                })
                .First();

            sb.AppendLine($"{employee147.FirstName} {employee147.LastName} - {employee147.JobTitle}");

            foreach (var e in employee147.projects)
            {
                sb.AppendLine($"{e.projectName}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 08
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var addresses = context
                .Addresses
                .Select(a => new
                {
                    a.AddressText,
                    townName = a.Town.Name,
                    employeesCount = a.Employees.Count
                })
                .OrderByDescending(a => a.employeesCount)
                .ThenBy(a => a.townName)
                .ThenBy(a => a.AddressText)
                .Take(10);

            foreach (var a in addresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.townName} - {a.employeesCount} employees");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 07**
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.EmployeesProjects.Any(p => p.Project.StartDate.Year >= 2001))
                .Where(e => e.EmployeesProjects.Any(p => p.Project.EndDate.Value.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    managerFirstName = e.Manager.FirstName,
                    managerLastName = e.Manager.LastName,
                    proj = e.EmployeesProjects.Select(p => new
                    {
                        projectName = p.Project.Name,
                        startDate = p.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                        endDate = p.Project.EndDate
                    })
                });

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} – Manager: {e.managerFirstName} {e.managerLastName} ");
                foreach (var p in e.proj)
                {

                    var endDate = p.endDate == null
                        ? "not finished"
                        : p.endDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    sb.AppendLine($"--{p.projectName} - {p.startDate} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 06
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            Employee nakov = context
                .Employees
                .First(e => e.LastName == "Nakov");

            nakov.Address = address;

            context.SaveChanges();

            var employees = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => new
                {
                    e.Address.AddressText
                });

            foreach (var e in employees)
            {
                sb
                    .AppendLine($"{e.AddressText}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 05
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .OrderBy(e => e.Salary).ThenByDescending(e => e.FirstName)
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary,
                    DepatrmentName = e.Department.Name

                });

            foreach (var e in employees)
            {
                sb
                    .AppendLine($"{e.FirstName} {e.LastName} from {e.DepatrmentName} - ${e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 04
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

        //Problem 03
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
