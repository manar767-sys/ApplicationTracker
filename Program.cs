using System.Linq.Expressions;
using System.Text;
using ApplicationTracker.Data;
using ApplicationTracker.Models;
using ApplicationTracker.Practice;
using ApplicationTracker.Services;
using static System.Net.Mime.MediaTypeNames;
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

            ApplicationService applicationService =
     new ApplicationService(jobService, candidateService, companyService);

            try
            {
                applicationService.Apply(1, 1);     // candidate 1 applies to job 1
                Console.WriteLine("Applied.");
            }
            catch (Exception ex)
            {
                Console.WriteLine( ex.Message);
            }

            try
            {
                applicationService.Apply(1, 1);     // candidate 1 applies to job 1
                Console.WriteLine("Applied.");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            applicationService.GetAll().ToList().ForEach(a =>
            {
                Console.WriteLine($"Application: {a.Id}, CandidateId: {a.CandidateId}, JobId: {a.JobId}, Status: {a.Status}");
            });













        }

    }
}
