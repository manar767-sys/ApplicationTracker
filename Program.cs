using System.Linq.Expressions;
using System.Text;
using ApplicationTracker.Data;
using ApplicationTracker.Menu;
using ApplicationTracker.Models;
using ApplicationTracker.Practice;
using ApplicationTracker.Services;


namespace ApplicationTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CollectionsPractice practice = new CollectionsPractice();
            //practice.practiceCollections();

            var people = new Any<Person>();
            people.Add(new Person { id = 1, firstName = "John", lastName = "Doe" });
            people.Add(new Person { id = 2, firstName = "Jane", lastName = "Smith" });
            //people.PrintItems();
            //Console.WriteLine($"Count of people: {people.Count()}");


            //sprint2 practice 
            FuncPractice funcPractice = new FuncPractice();
            Console.WriteLine($"Square of 5 is: {funcPractice.squareFunc(5)}");


            //project


            //passing services 
            SeedingData seedingData = new SeedingData();
            CompanyService companyService = new CompanyService();
            RecruiterService recruiterService = new RecruiterService();
            CandidateService candidateService = new CandidateService();
            JobService jobService = new JobService(companyService);

            seedingData.SeedData(companyService, recruiterService, jobService, candidateService);

            MenuHandler menuHandler = new MenuHandler(companyService, recruiterService, candidateService, jobService, new ApplicationService(jobService, candidateService, companyService));
            //menuHandler.DisplayMenu();







        }

    }
}
