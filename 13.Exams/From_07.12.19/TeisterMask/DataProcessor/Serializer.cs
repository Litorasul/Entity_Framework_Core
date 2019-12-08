using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TeisterMask.DataProcessor.ExportDto;

namespace TeisterMask.DataProcessor
{
    using System;
    using Data;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context
                .Projects
                .Where(p => p.Tasks.Count > 0)
                .Select(p => new ExportProjectsDto
                {
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate == null ? "No" : "Yes",
                    Tasks = p.Tasks.Select(t => new ExportTasksDto
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()

                    }).OrderBy(t => t.Name).ToArray(),
                    TasksCount = p.Tasks.Count
                })
                .OrderByDescending(p => p.TasksCount)
                .ThenBy(p => p.ProjectName)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportProjectsDto[]),
                new XmlRootAttribute("Projects"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), projects, namespaces );

            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context
                .Employees
                .Where(e => e.EmployeesTasks.Count > 0
                            && e.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
                .OrderByDescending(e => e.EmployeesTasks.Count)
                .ThenBy(e => e.Username)
                .Take(10)
                .Select(e => new ExportEmployeesDto
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                        .Where(t => t.Task.OpenDate >= date)
                        .Select(t => new ExportTaskEmployeesDto
                        {
                            TaskName = t.Task.Name,
                            OpenDate = t.Task.OpenDate
                                .ToString("d", CultureInfo.InvariantCulture),
                            DueDate = t.Task.DueDate
                                .ToString("d", CultureInfo.InvariantCulture),
                            LabelType = t.Task.LabelType.ToString(),
                            ExecutionType = t.Task.ExecutionType.ToString()

                        })
                        .OrderByDescending(t => t.DueDate)
                        .ThenBy(t => t.TaskName)
                        .ToArray()
                })
                .ToArray();

            return JsonConvert.SerializeObject(employees, Formatting.Indented);

        }
    }
}