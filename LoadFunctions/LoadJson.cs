using ELT_Piplene.AppContext;
using ELT_Piplene.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ELT_Piplene.LoadFunction
{
    public static class DataSeeder
    {
        public static void LoadAllData(ApplicationDbContext context, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            try
            {
                var jsonData = File.ReadAllText(filePath);
                using var doc = JsonDocument.Parse(jsonData);

                // ================== 1. Departments (without Manager temporarily) ==================
                List<Department> departments = new();
                Dictionary<int, string?> originalManagerMap = new();

                if (doc.RootElement.TryGetProperty("Departments", out var departmentsJson))
                {
                    departments = JsonSerializer.Deserialize<List<Department>>(departmentsJson.GetRawText()) ?? new List<Department>();
                    departments.RemoveAll(d => d.Dnum == 0 || d.Dname == null);

                    // Save original Manager_SSN values
                    originalManagerMap = departments.ToDictionary(d => d.Dnum, d => d.Manager_SSN);

                    // Temporarily set Manager_SSN to null to avoid FK conflict
                    foreach (var d in departments)
                        d.Manager_SSN = null;

                    context.Departments.AddRange(departments);
                    context.SaveChanges();
                    Console.WriteLine($"Inserted {departments.Count} Departments (without Manager)");
                }

                // ================== 2. Employees ==================
                List<Employee> employees = new();
                Dictionary<string, string?> originalSupervisorMap = new();

                if (doc.RootElement.TryGetProperty("Employees", out var employeesJson))
                {
                    employees = JsonSerializer.Deserialize<List<Employee>>(employeesJson.GetRawText()) ?? new List<Employee>();
                    employees.RemoveAll(e => e.SSN == null || !DepartmentExists(context, e.Dnum));

                    // Save original Super_SSN values
                    originalSupervisorMap = employees.ToDictionary(e => e.SSN, e => e.Super_SSN);

                    // Temporarily set Super_SSN to null
                    foreach (var e in employees)
                        e.Super_SSN = null;

                    context.Employees.AddRange(employees);
                    context.SaveChanges();
                    Console.WriteLine($"Inserted {employees.Count} Employees (without Supervisor)");
                }

                // ================== 3. Update Super_SSN after inserting employees ==================
                foreach (var e in employees)
                {
                    if (originalSupervisorMap.TryGetValue(e.SSN, out var originalSupervisor))
                    {
                        // Only update if the supervisor actually exists
                        if (string.IsNullOrEmpty(originalSupervisor) || context.Employees.Find(originalSupervisor) != null)
                        {
                            e.Super_SSN = originalSupervisor;
                        }
                        else
                        {
                            Console.WriteLine($"⚠ Ignored Super_SSN = {originalSupervisor} for Employee {e.SSN} because it doesn't exist.");
                            e.Super_SSN = null;
                        }
                    }
                }

                context.Employees.UpdateRange(employees);
                context.SaveChanges();
                Console.WriteLine("✅ Super_SSN updated for all employees (invalid ones ignored).");

                // ================== 4. DepartmentLocation ==================
                if (doc.RootElement.TryGetProperty("DepartmentLocation", out var locationsJson))
                {
                    var locations = JsonSerializer.Deserialize<List<DepartmentLocation>>(locationsJson.GetRawText());
                    if (locations != null)
                    {
                        Console.WriteLine("📍 Loaded DepartmentLocation: " + locations.Count);
                        context.DepartmentLocations.AddRange(locations);
                        context.SaveChanges();
                        Console.WriteLine($"Inserted {locations.Count} DepartmentLocations");
                    }
                    else
                    {
                        Console.WriteLine("❌ locations list is null");
                    }
                }
                else
                {
                    Console.WriteLine("❌ JSON key 'DepartmentLocation' not found.");
                }

                // ================== 4. Projects ==================
                if (doc.RootElement.TryGetProperty("Projects", out var projectsJson))
                {
                    var projects = JsonSerializer.Deserialize<List<Project>>(projectsJson.GetRawText());
                    if (projects != null)
                    {
                        projects.RemoveAll(p => !DepartmentExists(context, p.Dnum));
                        context.Projects.AddRange(projects);
                        context.SaveChanges();
                        Console.WriteLine($"Inserted {projects.Count} Projects");
                    }
                }

                //================== 5.Employee_Projects ==================

                if (doc.RootElement.TryGetProperty("Employee_Projects", out var epJson))
                {
                    var empProjects = JsonSerializer.Deserialize<List<Employee_Projects>>(epJson.GetRawText());
                    if (empProjects != null)
                    {
                        // Remove duplicates based on composite key
                        empProjects = empProjects
                            .GroupBy(ep => new { ep.ESSN, ep.Pnum })
                            .Select(g => g.First())
                            .ToList();

                        // Remove records with invalid foreign keys
                        empProjects.RemoveAll(ep => !EmployeeExists(context, ep.ESSN) || !ProjectExists(context, ep.Pnum));

                        context.EmployeeProjects.AddRange(empProjects);
                        context.SaveChanges();
                        Console.WriteLine($"Inserted {empProjects.Count} Employee_Projects (duplicates removed)");
                    }
                }

                // ================== 6. Dependents ==================
                if (doc.RootElement.TryGetProperty("Dependents", out var dependentsJson))
                {
                    var dependents = JsonSerializer.Deserialize<List<Dependent>>(dependentsJson.GetRawText());
                    if (dependents != null)
                    {
                        // Remove duplicates based on composite key
                        dependents = dependents
                            .GroupBy(d => new { d.Name, d.ESSN })
                            .Select(g => g.First())
                            .ToList();

                        // Remove records with invalid foreign keys
                        dependents.RemoveAll(d => !EmployeeExists(context, d.ESSN));

                        context.Dependents.AddRange(dependents);
                        context.SaveChanges();
                        Console.WriteLine($"Inserted {dependents.Count} Dependents (duplicates removed)");
                    }
                }

                // ================== 7. Update Department Managers ==================
                foreach (var dept in departments)
                {
                    if (originalManagerMap.TryGetValue(dept.Dnum, out var managerSSN) &&!string.IsNullOrEmpty(managerSSN) && context.Employees.Find(managerSSN) != null)
                    {
                        dept.Manager_SSN = managerSSN;
                    }
                  

                    else
                    {
                        Console.WriteLine($"Ignored Manager_SSN for Department {dept.Dnum} because Employee not found.");
                        dept.Manager_SSN = null;
                    }
                }

                context.Departments.UpdateRange(departments);
                context.SaveChanges();
                Console.WriteLine("Manager_SSN updated for all Departments (invalid ones ignored).");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data from {filePath}: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }


        // ======== Helper Methods ========
        private static bool DepartmentExists(ApplicationDbContext context, int? dnum)
        {
            return dnum != null && context.Departments.Find(dnum) != null;
        }

        private static bool EmployeeExists(ApplicationDbContext context, string? ssn)
        {
            return ssn != null && context.Employees.Find(ssn) != null;
        }

        private static bool ProjectExists(ApplicationDbContext context, int? pnum)
        {
            return pnum != null && context.Projects.Find(pnum) != null;
        }




    }
}