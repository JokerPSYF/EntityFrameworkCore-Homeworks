using System;
using SoftUni.Models;
using SoftUni.Data;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            Console.WriteLine(RemoveTown(context));
        }

        /// <summary>
        /// Now we can use SoftUniContext to extract data from our database.
        /// Your first task is to extract all employees and return their first,
        /// last and middle name,
        /// their job title and salary, rounded to 2 symbols after the decimal separator,
        /// all of those separated with a space. Order them by employee id.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 3
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            Employee[] employees = context
                .Employees
                .OrderBy(e => e.EmployeeId)
                .ToArray();

            StringBuilder output = new StringBuilder();

            foreach (Employee e in employees)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Your task is to extract all employees with salary over 50000.
        /// Return their first names and salaries in format "{firstName} - {salary}".
        /// Salary must be rounded to 2 symbols, after the decimal separator.
        /// Sort them alphabetically by first name.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 4
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context
                .Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToArray();

            StringBuilder output = new StringBuilder();

            foreach (var e in employees)
            {
                output.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Extract all employees from the Research and Development department.
        /// Order them by salary (in ascending order), then by first name (in descending order).
        /// Return only their first name, last name, department name and salary rounded to 2 symbols
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 5
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employees = context
                .Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary,
                })
                .Where(e => e.DepartmentName == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();

            foreach (var e in employees)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}");
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Create a new address with the text "Vitoshka 15" and TownId 4.
        /// Set that address to the employee with last the name "Nakov".
        /// Then order by descending all the employees by their Address' Id,
        /// take 10 rows and from them, take the AddressText.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 6
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            Address newAddress = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Add(newAddress);

            Employee nakov = context
                .Employees
                .First(e => e.LastName == "Nakov");
            nakov.Address = newAddress;

            context.SaveChanges();

            var adresses = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Select(e => e.Address.AddressText)
                .Take(10)
                .ToArray();

            foreach (var a in adresses)
            {
                output.AppendLine(a.ToString());
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Find the first 10 employees who have projects started in the period 2001 - 2003 (inclusive).
        /// Print each employee's first name, last name, manager's first name and last name.
        /// Then return all of their projects in the 
        /// format "--<ProjectName> - <StartDate> - <EndDate>", each on a new row. 
        /// If a project has no end date, print "not finished" instead.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 7
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employees = context
                .Employees
                .Include(e => e.EmployeesProjects)
                .ThenInclude(e => e.Project)
                .Where(e => e.EmployeesProjects
                .Any(ep => ep.Project.StartDate.Year >= 2001
                 && ep.Project.StartDate.Year <= 2003))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirtName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    AllProjects = e.EmployeesProjects
                   .Select(ep => new
                   {
                       ep.Project.Name,
                       StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                       EndDate = ep.Project.EndDate.HasValue ?
                       ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished"
                   })
                })
                .Take(10)
                .ToArray();

            foreach (var e in employees)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirtName} {e.ManagerLastName}");

                foreach (var ep in e.AllProjects)
                {
                    output.AppendLine($"--{ep.Name} - {ep.StartDate} - {ep.EndDate}");
                }
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Find all addresses, ordered by the number of employees who live there (descending),
        /// then by town name (ascending) and finally by address text (ascending).
        /// Take only the first 10 addresses.
        /// For each address return it in the format "<AddressText>,
        /// <TownName> - <EmployeeCount> employees"
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 8
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var addresses = context
                   .Addresses
                   .Select(a => new
                   {
                       a.AddressText,
                       townName = a.Town.Name,
                       count = a.Employees.Count()
                   })
                   .OrderByDescending(a => a.count)
                   .ThenBy(a => a.townName)
                   .ThenBy(a => a.AddressText)
                   .Take(10)
                   .ToArray();

            foreach (var a in addresses)
            {
                output.AppendLine($"{a.AddressText}, {a.townName} - {a.count} employees");
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Get the employee with id 147. Return only his/her first name, last name,
        /// job title and projects (print only their names).
        /// The projects should be ordered by name (ascending).
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 9
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employeesProjects = context
                .Employees
                .Where(e => e.EmployeeId == 147)
                .Include(t => t.EmployeesProjects)
                .ThenInclude(t => t.Project)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    AllProjects = e.EmployeesProjects
                    .Select(t => new
                    {
                        ProjectName = t.Project.Name
                    })
                })
                .ToArray();

            foreach (var e in employeesProjects)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                foreach (var p in e.AllProjects.OrderBy(a => a.ProjectName))
                {
                    output.AppendLine($"{p.ProjectName}");
                }
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Find all departments with more than 5 employees. Order them by employee count (ascending)
        /// then by department name (alphabetically). 
        /// For each department, print the department name and the manager's first and last name on the first row. 
        /// Then print the first name, the last name and the job title of every employee on a new row.
        /// Order the employees by first name(ascending), then by last name(ascending).
        /// Format of the output: For each department print it in the format
        /// "<DepartmentName> - <ManagerFirstName>  <ManagerLastName>"  
        /// and for each employee print it in the format
        /// "<EmployeeFirstName> <EmployeeLastName> - <JobTitle>".
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var departments = context
                .Departments
                .Where(d => d.Employees.Count > 5)
                .Select(d => new
                {
                    d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    AllEmployees = d.Employees
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    })
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToArray()
                })
                .ToArray();

            foreach (var d in departments)
            {
                output.AppendLine($"{d.Name} - {d.ManagerFirstName}  {d.ManagerLastName}");

                foreach (var e in d.AllEmployees)
                {
                    output.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Write a program that returns information about the last 10 started projects.
        /// Sort them by name lexicographically and return their name,
        /// description and start date, each on a new row.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var projects = context
                .Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate
                   .ToString("M/d/yyyy h:mm:ss tt")
                })
                .OrderBy(p => p.Name)
                .ToArray();

            foreach (var p in projects)
            {
                output.AppendLine($"{p.Name}");
                output.AppendLine($"{p.Description}");
                output.AppendLine($"{p.StartDate}");
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Write a program that increases salaries of all employees that are in the Engineering,
        /// Tool Design, Marketing, or Information Services department by 12%.
        /// Then return first name,last name and salary (2 symbols after the decimal separator) 
        /// for those employees whose salary was increased. Order them by first name (ascending),
        /// then by last name (ascending).
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employess = context
                 .Employees
                 .Where(e =>
                     e.Department.Name == "Engineering" ||
                     e.Department.Name == "Tool Design" ||
                     e.Department.Name == "Marketing" ||
                     e.Department.Name == "Information Services")
                 .OrderBy(e => e.FirstName)
                 .ThenBy(e => e.LastName)
                 .ToArray();

            foreach (Employee e in employess)
            {
                e.Salary *= 1.12m;
            }

            context.SaveChanges();

            foreach (Employee e in employess)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Write a program that finds all employees whose first name starts with "Sa".
        /// Return their first, last name, their job title and salary
        /// rounded to 2 symbols after the decimal separator in the format
        /// given in the example below. Order them by the first name,
        /// then by last name (ascending).
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.FirstName.ToLower().StartsWith("sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var e in employees)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Let's delete the project with id 2.
        /// Then, take 10 projects and return their names, each on a new line.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            Project deleteProject = context
                .Projects
                .Find(2);

            EmployeeProject[] dltEmpProj = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == deleteProject.ProjectId)
                .ToArray();

            context.RemoveRange(dltEmpProj);
            context.Remove(deleteProject);
            context.SaveChanges();

            var projects = context
                .Projects
                .Take(10)
                .Select(p => p.Name)
                .ToArray();

            foreach (var p in projects)
            {
                output.AppendLine(p);
            }

            return output.ToString().Trim();
        }

        /// <summary>
        /// Write a program that deletes a town with name "Seattle".
        /// Also, delete all addresses that are in those towns.
        /// Return the number of addresses that were deleted in
        /// format "{count} addresses in Seattle were deleted".
        /// There will be employees living at those addresses,
        /// which will be a problem when trying to delete the addresses.
        /// So, start by setting the AddressId of each employee for the
        /// given address to null. After all of them are set to null,
        /// you may safely remove all the addresses from the context 
        /// and finally remove the given town.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // Problem 15
        public static string RemoveTown(SoftUniContext context)
        {
            var employeesLivingSeatle = context
                .Employees
                .Where(e => e.Address.Town.Name == "Seattle")
                .ToArray();

            foreach (var e in employeesLivingSeatle)
            {
                e.AddressId = null;
            }

            var dltAddresses = context
                .Addresses
                .Where(a => a.Town.Name == "Seattle")
                .ToArray();

            Town seattle = context
                .Towns
                .First(t => t.Name == "Seattle");

            int addressesCount = dltAddresses.Count(); 

            context.RemoveRange(dltAddresses);

            context.Remove(seattle);

            context.SaveChanges();

            return $"{addressesCount} addresses in Seattle were deleted";
        }
    }
}
//    StringBuilder output = new StringBuilder();