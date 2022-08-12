namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            List<Project> validProjects = new List<Project>();

            ImportProjectDTO[] projDtos = Deserialize<ImportProjectDTO[]>(xmlString, "Projects");

            foreach (var projD in projDtos)
            {
                if (!IsValid(projD))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isProjectOpenDateValid=
                                  DateTime.TryParseExact(projD.OpenDate, "dd/MM/yyyy",
                                  CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime openDate);

                if (!isProjectOpenDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? dueDate = null;

                if (!String.IsNullOrEmpty(projD.DueDate)) 
                {
                    bool isProjectDueDateValid =
                                 DateTime.TryParseExact(projD.DueDate, "dd/MM/yyyy",
                                 CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDateValue);

                    if (!isProjectDueDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    dueDate = dueDateValue;
                }


                Project project = new Project()
                {
                    Name = projD.Name,
                    OpenDate = openDate,
                    DueDate = dueDate,
                };

                foreach (var task in projD.Tasks)
                {
                    if (!IsValid(task))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isTaskOpenDateValid =
                                DateTime.TryParseExact(task.OpenDate, "dd/MM/yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime taskOpenDate);

                    bool isTaskDueDateValid =
                                DateTime.TryParseExact(task.DueDate, "dd/MM/yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime taskDueDate);

                    if (!isTaskDueDateValid | !isTaskOpenDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (taskOpenDate < openDate|| taskDueDate > dueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;

                    }

                    Task validTask = new Task()
                    {
                        Name = task.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)task.ExecutionType,
                        LabelType = (LabelType)task.LabelType
                    };

                    project.Tasks.Add(validTask);
                }

                validProjects.Add(project);
                sb.AppendLine(String.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));

            }
            context.Projects.AddRange(validProjects);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            List<Employee> validEmployees = new List<Employee>();

            ImportEmployeeDTO[] empDtos = JsonConvert.DeserializeObject<ImportEmployeeDTO[]>(jsonString);

            foreach (var emp in empDtos)
            {
                if (!IsValid(emp))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Employee validEmp = new Employee()
                {
                    Username = emp.Username,
                    Email = emp.Email,
                    Phone = emp.Phone
                };

                foreach (var task in emp.Tasks.Distinct())
                {
                    if (!IsValid(task) || !context.Tasks.Any(x => x.Id == task))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    EmployeeTask employeeTask = new EmployeeTask()
                    {
                        Employee = validEmp,
                        TaskId = task
                    };

                    validEmp.EmployeesTasks.Add(employeeTask);
                }
                validEmployees.Add(validEmp);

                sb.AppendLine(String.Format(SuccessfullyImportedEmployee,
                                                       validEmp.Username,
                                                       validEmp.EmployeesTasks.Count));
            }
            context.Employees.AddRange(validEmployees);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }

        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            T dtos = (T)xmlSerializer
                .Deserialize(reader);

            return dtos;
        }
    }
}