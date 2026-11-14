using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTracker.Models;
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
        // Read a line of text from the console with a prompt, and trim whitespace. Returns the entered text.
        private string ReadOptionalText(string prompt)
        {
            Console.Write(prompt);
            return Input();
        }

        // Update helper: pressing Enter keeps the current value
        private string ReadKeepText(string label, string current)
        {
            Console.Write($"{label} [{current}]: ");
            string input = Input();
            return input.Length == 0 ? current : input;
        }
        private bool TryReadDecimal(string prompt, out decimal value)
        {
            Console.Write(prompt);
            string input = Input();
            if (!decimal.TryParse(input, out value))
            {
                Console.WriteLine($"'{input}' is not a valid number.");
                return false;
            }
            if (value <= 0)
            {
                Console.WriteLine("The value must be greater than 0.");
                return false;
            }
            return true;
        }
        //method to read a decimal number from the console with a prompt, and validate that the input is a positive decimal. It returns true if the input is valid, and false otherwise. The parsed decimal value is returned through the out parameter 'value'. If the user presses Enter without entering anything, the current value is kept.
        private bool TryReadKeepDecimal(string label, decimal current, out decimal value)
        {
            Console.Write($"{label} [{current}]: ");
            string input = Input();
            if (input.Length == 0)
            {
                value = current;
                return true;
            }
            if (!decimal.TryParse(input, out value))
            {
                Console.WriteLine($"'{input}' is not a valid number.");
                return false;
            }
            if (value <= 0)
            {
                Console.WriteLine("The value must be greater than 0.");
                return false;
            }
            return true;
        }
        //method to read a line of text from the console, trim whitespace, and return the result. If the user presses Enter without entering anything, an empty string is returned.
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
                Console.WriteLine("===== MAIN MENU =====");
                Console.WriteLine("1. Companies");
                Console.WriteLine("2. Recruiters");
                Console.WriteLine("3. Jobs");
                Console.WriteLine("4. Candidates");
                Console.WriteLine("5. Applications");
                Console.WriteLine("6. Exit");
                Console.Write("Choose an option: ");

                running = HandleChoice(Input());
            }

            Console.WriteLine("Goodbye!");
        }

        // ===================== choice Handler =====================

        private bool HandleChoice(string choice)
        {
            switch (choice)
            {
                case "1": CompaniesMenu(); break;
                case "2": RecruitersMenu(); break;
                case "3": JobsMenu(); break;
                case "4": CandidatesMenu(); break;
                case "5": ApplicationsMenu(); break;
                case "6": return false;
                default:
                    Console.WriteLine("Invalid option. Please enter a number from 1 to 6.");
                    break;
            }
            return true;
        }

        private void CompaniesMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n===== COMPANIES =====");
                Console.WriteLine("1. List");
                Console.WriteLine("2. Change status (approve / reject)");
                Console.WriteLine("3. List by status");
                Console.WriteLine("4. Undo last status change");
                Console.WriteLine("0. Back to main menu");
                Console.Write("Choose an option: ");

                try
                {
                    switch (Input())
                    {
                        case "1": PrintCompanies(companyService.GetAll()); break;
                        case "2": ChangeCompanyStatus(); break;
                        case "3": ListCompaniesByStatus(); break;
                        case "4": UndoStatusChange(); break;
                        case "0": back = true; break;
                        default: Console.WriteLine("Invalid option. Try again."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                try
                {
                    switch (Input())
                    {
                        case "1": PrintCompanies(companyService.GetAll()); break;
                        case "0": back = true; break;
                        default: Console.WriteLine("Invalid option. Try again and enter a number from"); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void PrintCompanies(List<Company> companies)
        {
            if (companies.Count == 0)
            {
                Console.WriteLine("No companies found.");
                return;
            }

            Console.WriteLine("{0,-5}{1,-20}{2,-16}{3,-16}{4,-10}", "Id", "Name", "Industry", "City", "Status");
            Console.WriteLine(new string('-', 67));
            foreach (Company c in companies)
            {
                Console.WriteLine("{0,-5}{1,-20}{2,-16}{3,-16}{4,-10}",
                    c.Id, Cut(c.Name, 19), Cut(c.Industry, 15), Cut(c.City, 15), c.Status);
            }
        }
        private void AddJob()
        {
            if (!TryReadText("Title: ", out string title)) return;
            string description = ReadOptionalText("Description: ");
            if (!TryReadDecimal("Salary: ", out decimal salary)) return;
            string location = ReadOptionalText("Location: ");
            if (!TryReadId("Recruiter Id (the job is posted for this recruiter's company): ", out int recruiterId)) return;

            Recruiter recruiter = recruiterService.GetById(recruiterId);
            if (recruiter == null) return;

            Job job = new Job
            {
                Title = title,
                Description = description,
                Salary = salary,
                Location = location,
                IsOpen = true,
                PostedDate = DateTime.Now,
                CompanyId = recruiter.CompanyId,
                RecruiterId = recruiter.Id
            };
            jobService.Add(job);      // refused if the company is not Approved
            Console.WriteLine($"Job added with Id {job.Id}.");
        }

        private void UpdateJob()
        {
            if (!TryReadId("Job Id to update: ", out int id)) return;
            Job existing = jobService.GetById(id);
            if (existing == null) return;

            Console.WriteLine("Press Enter to keep the current value.");
            string title = ReadKeepText("Title", existing.Title);
            string description = ReadKeepText("Description", existing.Description);
            if (!TryReadKeepDecimal("Salary", existing.Salary, out decimal salary)) return;
            string location = ReadKeepText("Location", existing.Location);

            Job updated = new Job
            {
                Id = existing.Id,
                Title = title,
                Description = description,
                Salary = salary,
                Location = location,
                IsOpen = existing.IsOpen,
                PostedDate = existing.PostedDate,
                CompanyId = existing.CompanyId,
                RecruiterId = existing.RecruiterId
            };
            jobService.Update(updated);
            Console.WriteLine("Job updated.");
        }

        private void RemoveJob()
        {
            if (!TryReadId("Job Id to remove: ", out int id)) return;
            Job job = jobService.GetById(id);
            if (job == null) return;

            if (!Confirm($"Remove job '{job.Title}'?"))
            {
                Console.WriteLine("Cancelled.");
                return;
            }
            jobService.Remove(job);
            Console.WriteLine("Job removed.");
        }

        private void CloseJob()
        {
            if (!TryReadId("Job Id to close: ", out int id)) return;
            Job job = jobService.GetById(id);
            if (job == null) return;

            if (!job.IsOpen)
            {
                Console.WriteLine("This job is already closed.");
                return;
            }
            job.IsOpen = false;     // FindJob returns the stored object, so this changes the real job
            Console.WriteLine("Job closed.");
        }

        private void PrintJobs(List<Job> jobs)
        {
            if (jobs.Count == 0)
            {
                Console.WriteLine("No jobs found.");
                return;
            }

            Console.WriteLine("{0,-5}{1,-22}{2,-18}{3,-10}{4,-16}{5,-6}", "Id", "Title", "Company", "Salary", "Location", "Open");
            Console.WriteLine(new string('-', 77));
            foreach (Job j in jobs)
            {
                Console.WriteLine("{0,-5}{1,-22}{2,-18}{3,-10:N0}{4,-16}{5,-6}",
                    j.Id, Cut(j.Title, 21), Cut(CompanyName(j.CompanyId), 17), j.Salary,
                    Cut(j.Location, 15), j.IsOpen ? "Yes" : "No");
            }
        }
        //a method to fetsh company name
        private string CompanyName(int companyId)
        {
            try { return companyService.GetById(companyId).Name; }
            catch (ArgumentException) { return "Unknown"; }
        }
        private void ChangeCompanyStatus()
        {
            if (!TryReadId("Company Id: ", out int id)) return;
            Company company = companyService.GetById(id);
            if (company == null) return;

            Console.WriteLine($"Current status: {company.Status}");
            if (!TryReadStatus(out CompanyStatus newStatus)) return;

            companyService.ChangeStatus(id, newStatus);   // throws a clear message if the change isn't allowed
            Console.WriteLine($"Status changed to {newStatus}.");
        }

        private void ListCompaniesByStatus()
        {
            if (!TryReadStatus(out CompanyStatus status)) return;
            PrintCompanies(companyService.ListByStatus(status));
        }

        private void UndoStatusChange()
        {
            if (companyService.UndoLastChange())
                Console.WriteLine("Last status change undone.");
            else
                Console.WriteLine("There is nothing to undo.");
        }

        private bool TryReadStatus(out CompanyStatus status)
        {
            Console.WriteLine("Status: 1. Pending  2. Approved  3. Rejected");
            status = CompanyStatus.Pending;
            if (!TryReadInt("Choose: ", out int choice)) return false;

            switch (choice)
            {
                case 1: status = CompanyStatus.Pending; return true;
                case 2: status = CompanyStatus.Approved; return true;
                case 3: status = CompanyStatus.Rejected; return true;
                default:
                    Console.WriteLine("Invalid status choice. Enter 1, 2 or 3.");
                    return false;
            }
        }
        private void RecruitersMenu() { Console.WriteLine("Recruiters: coming soon."); }
        private void JobsMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n===== JOBS =====");
                Console.WriteLine("1. Add");
                Console.WriteLine("2. Update");
                Console.WriteLine("3. List");
                Console.WriteLine("4. Remove");
                Console.WriteLine("5. Close a job");
                Console.WriteLine("0. Back to main menu");
                Console.Write("Choose an option: ");

                try
                {
                    switch (Input())
                    {
                        case "1": AddJob(); break;
                        case "2": UpdateJob(); break;
                        case "3": PrintJobs(jobService.GetAll()); break;
                        case "4": RemoveJob(); break;
                        case "5": CloseJob(); break;
                        case "0": back = true; break;
                        default: Console.WriteLine("Invalid option. Try again."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
        private void CandidatesMenu() { Console.WriteLine("Candidates: coming soon."); }
        private void ApplicationsMenu() { Console.WriteLine("Applications: coming soon."); }
    }
}