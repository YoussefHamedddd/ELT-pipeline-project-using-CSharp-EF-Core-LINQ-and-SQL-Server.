using System;
using System.Collections.Generic;

namespace ELT_Piplene.Models
{
    public class Department
    {
        public int Dnum { get; set; } // Primary Key
        public string Dname { get; set; }
       
        public string? Manager_SSN { get; set; }
        public string? Hiring_Date { get; set; }

        // Navigation
        public Employee? Manager { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<DepartmentLocation> Locations { get; set; } = new List<DepartmentLocation>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
