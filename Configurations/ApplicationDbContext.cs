using ELT_Piplene.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;




namespace ELT_Piplene.AppContext
{

    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext()
        {

        }

     


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                
                optionsBuilder.UseSqlServer("Write Your connection String Here");
            }
        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ---------------- Employee ----------------

            modelBuilder.Entity<Employee>()
                .HasKey(e => e.SSN);

            modelBuilder.Entity<Department>()
                .Property(d => d.Dnum)
                .ValueGeneratedNever(); // No identity

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.Dnum)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Supervisor)
                .WithMany(s => s.Subordinates)
                .HasForeignKey(e => e.Super_SSN)
                .OnDelete(DeleteBehavior.Restrict); 

            // ---------------- Department ----------------
            modelBuilder.Entity<Department>()
                .HasKey(d => d.Dnum);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithMany()
                .HasForeignKey(d => d.Manager_SSN)
                .OnDelete(DeleteBehavior.Restrict); 

            // ---------------- DepartmentLocation ----------------
            modelBuilder.Entity<DepartmentLocation>()
                .HasKey(dl => new { dl.Dnum, dl.Location });

            modelBuilder.Entity<DepartmentLocation>()
                .HasOne(dl => dl.Department)
                .WithMany(d => d.Locations)
                .HasForeignKey(dl => dl.Dnum)
                .OnDelete(DeleteBehavior.Restrict); 

            // ---------------- Project ----------------
            modelBuilder.Entity<Project>()
                .HasKey(p => p.Pnum);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Department)
                .WithMany(d => d.Projects)
                .HasForeignKey(p => p.Dnum)
                .OnDelete(DeleteBehavior.Restrict); // منع cascade

            modelBuilder.Entity<Project>()
                .Property(p => p.Pnum)
                .ValueGeneratedNever();

            // ---------------- EmployeeProjects (Many-to-Many) ----------------
            modelBuilder.Entity<Employee_Projects>()
                .HasKey(ep => new { ep.ESSN, ep.Pnum });

            modelBuilder.Entity<Employee_Projects>()
                .HasOne(ep => ep.Employee)
                .WithMany(e => e.EmployeeProjects)
                .HasForeignKey(ep => ep.ESSN)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Employee_Projects>()
                .HasOne(ep => ep.Project)
                .WithMany(p => p.EmployeeProjects)
                .HasForeignKey(ep => ep.Pnum)
                .OnDelete(DeleteBehavior.Restrict); 

            // ---------------- Dependent ----------------
            modelBuilder.Entity<Dependent>()
                .HasKey(d => new { d.Name, d.ESSN });

            modelBuilder.Entity<Dependent>()
                .HasOne(d => d.Employee)
                .WithMany(e => e.Dependents)
                .HasForeignKey(d => d.ESSN)
                .OnDelete(DeleteBehavior.Cascade); 


        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentLocation> DepartmentLocations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Dependent> Dependents { get; set; }
        public DbSet<Employee_Projects> EmployeeProjects { get; set; }




    }
}