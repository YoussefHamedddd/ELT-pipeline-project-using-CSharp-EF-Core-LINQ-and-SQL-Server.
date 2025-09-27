using System;

namespace ELT_Piplene.Models
{
    public class DepartmentLocation
    {
        public int Dnum { get; set; } // FK to Department
        public string Location { get; set; }

        // Navigation
        public Department? Department { get; set; }
    }
}
