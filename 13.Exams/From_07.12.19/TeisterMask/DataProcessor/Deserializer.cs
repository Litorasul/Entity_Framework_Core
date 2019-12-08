using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using TeisterMask.Data.Models;
using TeisterMask.Data.Models.Enums;
using TeisterMask.DataProcessor.ImportDto;

namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportProjectsDto[]),
                new XmlRootAttribute("Projects"));

            var projects = (ImportProjectsDto[])xmlSerializer
                .Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();

            foreach (var dto in projects)
            {
                DateTime openDate;
                DateTime dueDate;

                DateTime taskOD;
                DateTime taskDD;

                bool dueDateValidation = DateTime.TryParse(dto.DueDate, out dueDate) 
                                         || dto.DueDate == null;

                if (IsValid(dto)
                    && DateTime.TryParse(dto.OpenDate, out openDate)
                    && dueDateValidation)
                {
                    var project = new Project
                    {
                        Name = dto.Name,
                        OpenDate = DateTime.ParseExact(dto.OpenDate,
                            "dd/MM/yyyy", CultureInfo.InvariantCulture),
                     
                    };

                    if (dto.DueDate != null)
                    {
                        project.DueDate = DateTime.ParseExact(dto.DueDate,
                            "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }


                    context.Projects.Add(project);

                    foreach (var taskDto in dto.Tasks)
                    {
                        if (IsValid(taskDto)
                            && DateTime.TryParse(taskDto.OpenDate, out taskOD)
                            && DateTime.TryParse(taskDto.DueDate, out taskDD)
                            && taskOD >= openDate
                            && taskDD <= dueDate)
                        {
                            var task = new Task
                            {
                                Name = taskDto.Name,
                                OpenDate = DateTime.ParseExact(taskDto.OpenDate,
                                    "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                DueDate = DateTime.ParseExact(taskDto.DueDate,
                                    "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                ExecutionType = (ExecutionType)taskDto.ExecutionType,
                                LabelType = (LabelType)taskDto.LabelType,
                                ProjectId = project.Id
                            };

                            context.Tasks.Add(task);
                        }
                        else
                        {
                            sb.AppendLine(ErrorMessage);
                        }
                    }

                    sb.AppendLine(string
                        .Format(SuccessfullyImportedProject, dto.Name, project.Tasks.Count));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var employees = JsonConvert.DeserializeObject<ImportEmployeesDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();

            foreach (var dto in employees)
            {
                if (IsValid(dto))
                {
                    var employee = new Employee
                    {
                        Username = dto.Username,
                        Email = dto.Email,
                        Phone = dto.Phone
                    };

                    context.Employees.Add(employee);

                    foreach (int taskId in dto.TasksIds)
                    {
                        if (IsValidTask(context, taskId))
                        {
                            var employeeTask = new EmployeeTask
                            {
                                EmployeeId = employee.Id,
                                TaskId = taskId
                            };

                            context.EmployeesTasks.Add(employeeTask);
                            employee.EmployeesTasks.Add(employeeTask);
                        }
                        else
                        {
                            sb.AppendLine(ErrorMessage);
                        }
                    }

                    sb.AppendLine(string.Format(SuccessfullyImportedEmployee,
                        employee.Username, employee.EmployeesTasks.Count));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }

            }

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValidTask(TeisterMaskContext context, int taskId)
        {
            return context.Tasks.Any(t => t.Id == taskId);
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}