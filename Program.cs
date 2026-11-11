using System.Linq.Expressions;
using System.Text;
using ApplicationTracker.Data;
using ApplicationTracker.Models;
using ApplicationTracker.Practice;
using ApplicationTracker.Services;
using static System.Net.WebRequestMethods;

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


            //project for sprint one


            //passing services 
            SeedingData seedingData = new SeedingData();
            CompanyService companyService = new CompanyService();
            RecruiterService recruiterService = new RecruiterService();
            CandidateService candidateService = new CandidateService();
            JobService jobService = new JobService(companyService);

            seedingData.SeedData(companyService, recruiterService, jobService, candidateService);

            try
            {
                jobService.Add(new Job
                {
                    Title = "Test Job",
                    Description = "Test",
                    Salary = 50000,
                    Location = "Cairo",
                    IsOpen = true,
                    PostedDate = new DateTime(2026, 11, 1),
                    CompanyId = 3,        // the Pending company
                    RecruiterId = 3
                });
                Console.WriteLine("Added (this should NOT happen)");
            }
            catch (IndexOutOfRangeException ex)
            {
                // Runs only if an array index is out of bounds
                Console.WriteLine($"Index error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Catches any other unexpected exceptions
                Console.WriteLine($"General error: {ex.Message}");
            }
            finally
            {
                // Always runs, whether an exception occurred or not
                Console.WriteLine("Execution finished.");
            }












        }

    }
}
