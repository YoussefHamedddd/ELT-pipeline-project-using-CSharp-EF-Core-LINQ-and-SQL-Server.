namespace ELT_Piplene.Models
{
    public class Employee_Projects
    {
        public string ESSN { get; set; } // FK to Employee
        public int Pnum { get; set; }    // FK to Project
        public int? NumOfHours { get; set; }

        // Navigation
        public Employee? Employee { get; set; }
        public Project? Project { get; set; }
    }
}
