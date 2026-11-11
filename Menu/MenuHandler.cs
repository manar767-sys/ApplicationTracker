using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTracker.Services;

namespace ApplicationTracker.Menu
{
    public class MenuHandler
    {
        private CompanyService companyService;
        private RecruiterService recruiterService;
        private CandidateService candidateService;
        private JobService jobService;
        private ApplicationService applicationService;

        public MenuHandler(CompanyService companyService, RecruiterService recruiterService, CandidateService candidateService, JobService jobService, ApplicationService applicationService)
        {
            this.companyService = companyService;
            this.recruiterService = recruiterService;
            this.candidateService = candidateService;
            this.jobService = jobService;
            this.applicationService = applicationService;
        }
        public void DisplayMenu()
        {
            Console.WriteLine("Welcome to the Application Tracker!");

            bool running = true;
            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. List Companies");
                Console.WriteLine("2. List Recruiters");
                Console.WriteLine("3. List Candidates");
                Console.WriteLine("4. List Jobs");
                Console.WriteLine("5. Apply for a Job");
                Console.WriteLine("6. Exit");

                string choice = Console.ReadLine();
                running = HandleChoice(choice);     // false only when the user picks Exit
            }

            Console.WriteLine("Goodbye!");
        }

       
            private bool HandleChoice(string choice)
        {
            int number;
            if (!int.TryParse(choice, out number))
            {
                number = 0;
            }

            switch (number)
            {
                case 1:
                    foreach (var company in companyService.GetAll())
                    {
                        Console.WriteLine($"Company ID: {company.Id}, Name: {company.Name}");
                    }
                    break;
                case 2:
                    foreach (var recruiter in recruiterService.GetAll())
                    {
                        Console.WriteLine($"Recruiter ID: {recruiter.Id}, Name: {recruiter.Name}");
                    }
                    break;
                case 3:
                    foreach (var candidate in candidateService.GetAll())
                    {
                        Console.WriteLine($"Candidate ID: {candidate.Id}, Name: {candidate.FullName} {candidate.Id}");
                    }
                    break;
                case 4:
                    foreach (var job in jobService.GetAll())
                    {
                        //error:company name cannot be fetched
                        Console.WriteLine($"Job ID: {job.Id}, Title: {job.Title}, Company: {job.Company?.Name ?? "Unknown"}");
                    }   
                    break;
                case 5:
                    {
                        bool applyMenu = true;

                        while (applyMenu)
                        {
                            Console.WriteLine("\n===== APPLY FOR A JOB =====");
                            Console.WriteLine("1. View Available Jobs");
                            Console.WriteLine("2. Apply for a Job");
                            Console.WriteLine("0. Back to Main Menu");
                            Console.Write("Choose an option: ");

                            string subChoice = Console.ReadLine();

                            switch (subChoice)
                            {
                                case "1":
                                    foreach (var job in jobService.GetAll())
                                    {
                                        Console.WriteLine(
                                            $"Job ID: {job.Id}, " +
                                            $"Title: {job.Title}, " +
                                            $"Company: {job.Company?.Name ?? "Unknown"}"
                                        );
                                    }
                                    break;

                                case "2":
                                    try
                                    {
                                        Console.Write("Enter Candidate ID: ");
                                        int candidateId = int.Parse(Console.ReadLine());

                                        Console.Write("Enter Job ID: ");
                                        int jobId = int.Parse(Console.ReadLine());

                                        applicationService.Apply(candidateId, jobId);

                                        Console.WriteLine("Application submitted successfully!");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error: {ex.Message}");
                                    }
                                    break;

                                case "0":
                                    applyMenu = false;
                                    break;

                                default:
                                    Console.WriteLine("Invalid option. Try again.");
                                    break;
                            }
                        }

                        break;
                    }
                case 6:
                    return false;                   // exit loop
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            return true;                   
        }
    }
}
