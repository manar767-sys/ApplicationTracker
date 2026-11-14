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

        // ===================== INPUT HELPERS =====================

        private string Input()
        {
            return (Console.ReadLine() ?? "").Trim();
        }
        //method to read an integer from the console with a prompt, and validate the input. It returns true if the input is a valid integer, and false otherwise. The parsed integer value is returned through the out parameter 'value'.
        private bool TryReadInt(string prompt, out int value)
        {
            Console.Write(prompt);
            string input = Input();
            if (!int.TryParse(input, out value))
            {
                Console.WriteLine($"'{input}' is not a valid number.");
                return false;
            }
            return true;
        }
        //method to read an integer ID from the console with a prompt, and validate that the input is a positive integer. It returns true if the input is a valid positive integer, and false otherwise. The parsed integer ID is returned through the out parameter 'id'.
        private bool TryReadId(string prompt, out int id)
        {
            if (!TryReadInt(prompt, out id)) return false;
            if (id <= 0)
            {
                Console.WriteLine("The Id must be greater than 0.");
                return false;
            }
            return true;
        }
        //method to read a non-empty string from the console with a prompt. It returns true if the input is not empty, and false otherwise. The entered text is returned through the out parameter 'text'.
        private bool TryReadText(string prompt, out string text)
        {
            Console.Write(prompt);
            text = Input();
            if (text.Length == 0)
            {
                Console.WriteLine("This field cannot be empty.");
                return false;
            }
            return true;
        }
        //method to confirm an action by asking the user a yes/no question. It returns true if the user answers "y" or "yes" (case-insensitive), and false otherwise.
        private bool Confirm(string question)
        {
            Console.Write(question + " (y/n): ");
            string answer = Input().ToLower();
            return answer == "y" || answer == "yes";
        }
        //method to truncate a string to a specified width and add ellipsis (...) if the string exceeds that width. It returns the original string if it is within the specified width, or a truncated version with ellipsis if it exceeds the width.
        private static string Cut(string text, int width)
        {
            if (text == null) return "";
            return text.Length <= width ? text : text.Substring(0, width - 3) + "...";
        }
        // ===================== Display Menu =====================
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
