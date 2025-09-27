using System.Collections.Generic;

namespace ELT_Piplene.Models
{
    public class Project
    {
        public int Pnum { get; set; }      // Primary Key
        public string Pname { get; set; }
        public string? Location { get; set; }
        public string? City { get; set; }

        // FK
        public int? Dnum { get; set; }     // FK to Department

        // Navigation
        public Department? Department { get; set; }
        public ICollection<Employee_Projects> EmployeeProjects { get; set; } = new List<Employee_Projects>();
    }
}

