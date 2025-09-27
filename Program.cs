using ELT_Piplene.AppContext;
using ELT_Piplene.LoadFunction;
using ELT_Piplene.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Main
{

    public class Program
    {
        public static void Main(string[] args)
        {


          
            using var context = new ApplicationDbContext();

         
            string filePath = "DataSource/Company.json";

           
            DataSeeder.LoadAllData(context, filePath);

            Console.WriteLine("Data loading finished.");




        }
    }
}