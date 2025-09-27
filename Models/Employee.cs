using System;
using System.Collections.Generic;

namespace ELT_Piplene.Models
{
    public class Employee
    {
        public string SSN { get; set; }  // Primary Key
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string? Gender { get; set; }
        public string? Birthdate { get; set; }

        // FKs
        public int? Dnum { get; set; }        // FK to Department
        public string? Super_SSN { get; set; } // FK to Employee (Supervisor)

        // Navigation
        public Department? Department { get; set; }
        public Employee? Supervisor { get; set; }
        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
        public ICollection<Employee_Projects> EmployeeProjects { get; set; } = new List<Employee_Projects>();
        public ICollection<Dependent> Dependents { get; set; } = new List<Dependent>();
    }
}
