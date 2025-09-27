using System;

namespace ELT_Piplene.Models
{
    public class Dependent
    {
        public string Name { get; set; }        // Part of Composite Key
        public string ESSN { get; set; }        // FK to Employee
        
        public string? Birthdate { get; set; }
        public string? Gender { get; set; }

        // Navigation
        public Employee Employee { get; set; }
    }
}
