
using ApplicationTracker.Exceptions;
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
        private CurrentUser currentUser;      // null until someone picks a role

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

        // Update helper: pressing Enter keeps the current number
        private bool TryReadKeepInt(string label, int current, out int value)
        {
            Console.Write($"{label} [{current}]: ");
            string input = Input();
            if (input.Length == 0)
            {
                value = current;
                return true;
            }
            if (!int.TryParse(input, out value))
            {
                Console.WriteLine($"'{input}' is not a valid number.");
                return false;
            }
            return true;
        }
        // ===================== Display Menu =====================
        public void DisplayMenu()
        {
            Console.WriteLine("Welcome to the Application Tracker!");

            bool running = true;
            while (running)
            {
                if (currentUser == null)
                {
                    currentUser = SelectUser();
                    if (currentUser == null) break;          // the user chose Exit
                }

                if (currentUser.IsAdmin)
                    running = AdminMainMenu();
                else if (currentUser.IsRecruiter)
                    running = RecruiterMainMenu();
                else
                    running = CandidateMainMenu();
            }

            Console.WriteLine("Goodbye!");
        }
        private bool CandidateMainMenu()
        {
            Candidate candidate = FindCandidate(currentUser.UserId.Value);
            if (candidate == null)
            {
                currentUser = null;                  // the candidate no longer exists: back to "Continue as"
                return true;
            }

            Console.WriteLine();
            Console.WriteLine($"===== CANDIDATE MENU: {candidate.FullName} =====");
            Console.WriteLine("1. Available jobs");
            Console.WriteLine("2. Find a job by Id");
            Console.WriteLine("3. Search jobs");
            Console.WriteLine("4. Sort jobs");
            Console.WriteLine("5. Apply for a job");
            Console.WriteLine("6. My applications");
            Console.WriteLine("7. Switch user");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            try
            {
                switch (Input())
                {
                    case "1": ViewAvailableJobs(); break;
                    case "2": ShowJob(); break;
                    case "3": SearchJobs(); break;
                    case "4": SortJobs(); break;
                    case "5": ApplyAsCandidate(candidate); break;
                    case "6": ListMyApplications(candidate); break;
                    case "7": currentUser = null; break;
                    case "0": return false;
                    default: Console.WriteLine("Invalid option. Please try again."); break;
                }
            }
            catch (JobClosedException ex)
            {
                Console.WriteLine("Cannot apply: " + ex.Message);
            }
            catch (CompanyNotApprovedException ex)
            {
                Console.WriteLine("Cannot apply: " + ex.Message);
            }
            catch (DuplicateApplicationException)
            {
                Console.WriteLine("You have already applied to this job.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return true;
        }

        private void ApplyAsCandidate(Candidate candidate)
        {
            try
            {
                if (!TryReadId("Job Id: ", out int jobId)) return;
                if (FindJob(jobId) == null) return;

                applicationService.Apply(candidate.Id, jobId);      // closed job, company not Approved, duplicate -> clear message
                Console.WriteLine("Application submitted successfully!");
            }
            catch (JobClosedException ex)
            {
                Console.WriteLine("Cannot apply: " + ex.Message);
            }
            catch (CompanyNotApprovedException ex)
            {
                Console.WriteLine("Cannot apply: " + ex.Message);
            }
            catch (DuplicateApplicationException)
            {
                Console.WriteLine("You have already applied to this job.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void ListMyApplications(Candidate candidate)
        {
            List<Application> mine = new List<Application>();
            foreach (Application application in applicationService.GetAll())
            {
                if (application.CandidateId == candidate.Id)
                    mine.Add(application);
            }
            PrintApplications(mine);
        }
        private bool RecruiterMainMenu()
        {
            Recruiter recruiter = FindRecruiter(currentUser.UserId.Value);
            if (recruiter == null)
            {
                currentUser = null;                  // the recruiter no longer exists: back to "Continue as"
                return true;
            }

            Console.WriteLine();
            Console.WriteLine($"===== RECRUITER MENU: {recruiter.Name} ({CompanyName(recruiter.CompanyId)}) =====");
            Console.WriteLine("1. My jobs");
            Console.WriteLine("2. Add a job");
            Console.WriteLine("3. Update a job");
            Console.WriteLine("4. Close a job");
            Console.WriteLine("5. Remove a job");
            Console.WriteLine("6. Search jobs");
            Console.WriteLine("7. Sort jobs");
            Console.WriteLine("8. Applications for my jobs");
            Console.WriteLine("9. Switch user");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            try
            {
                switch (Input())
                {
                    case "1": ListMyJobs(recruiter); break;
                    case "2": AddMyJob(recruiter); break;
                    case "3": UpdateMyJob(recruiter); break;
                    case "4": CloseMyJob(recruiter); break;
                    case "5": RemoveMyJob(recruiter); break;
                    case "6": SearchJobs(); break;
                    case "7": SortJobs(); break;
                    case "8": ListApplicationsForMyJobs(recruiter); break;
                    case "9": currentUser = null; break;
                    case "0": return false;
                    default: Console.WriteLine("Invalid option. Please try again."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return true;
        }
        private bool DefaultMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine($"===== MAIN MENU ({currentUser.Role}: {currentUser.Name}) =====");
            Console.WriteLine("1. Companies");
            Console.WriteLine("2. Recruiters");
            Console.WriteLine("3. Jobs");
            Console.WriteLine("4. Candidates");
            Console.WriteLine("5. Applications");
            Console.WriteLine("6. Exit");
            Console.WriteLine("7. Switch user");
            Console.Write("Choose an option: ");

            return HandleChoice(Input());
        }
        private bool AdminMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== ADMIN MENU =====");
            Console.WriteLine("1. Company approval");
            Console.WriteLine("2. Recruiters");
            Console.WriteLine("3. Jobs");
            Console.WriteLine("4. Candidates");
            Console.WriteLine("5. Applications");
            Console.WriteLine("6. Switch user");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            switch (Input())
            {
                case "1": AdminMenu(); break;
                case "2": RecruitersMenu(); break;
                case "3": AdminJobsMenu(); break;                       
                case "4": CandidatesMenu(); break;
                case "5": PrintApplications(applicationService.GetAll()); break;   
                case "6": currentUser = null; break;
                case "0": return false;
                default: Console.WriteLine("Invalid option. Please try again."); break;
            }
            return true;
        }
        private void AdminMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n===== COMPANY APPROVAL =====");
                Console.WriteLine("1. List companies waiting for approval");
                Console.WriteLine("2. Approve a company");
                Console.WriteLine("3. Reject a company");
                Console.WriteLine("4. List approved companies");
                Console.WriteLine("5. List rejected companies");
                Console.WriteLine("6. Undo last status change");
                Console.WriteLine("7. Manage companies (add, update, remove)");
                Console.WriteLine("0. Back to admin menu");
                Console.Write("Choose an option: ");

                try
                {
                    switch (Input())
                    {
                        case "1": PrintCompanies(companyService.ListByStatus(CompanyStatus.Pending)); break;
                        case "2": ChangeStatusTo(CompanyStatus.Approved, "approved"); break;
                        case "3": ChangeStatusTo(CompanyStatus.Rejected, "rejected"); break;
                        case "4": PrintCompanies(companyService.ListByStatus(CompanyStatus.Approved)); break;
                        case "5": PrintCompanies(companyService.ListByStatus(CompanyStatus.Rejected)); break;
                        case "6": UndoStatusChange(); break;
                        case "7": CompaniesMenu(); break;
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
        private void AdminJobsMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n===== JOBS (read only) =====");
                Console.WriteLine("1. List");
                Console.WriteLine("2. Find a job by Id");
                Console.WriteLine("3. Search jobs (title and/or minimum salary)");
                Console.WriteLine("4. Sort jobs");
                Console.WriteLine("0. Back to admin menu");
                Console.Write("Choose an option: ");

                try
                {
                    switch (Input())
                    {
                        case "1": PrintJobs(jobService.GetAll()); break;
                        case "2": ShowJob(); break;
                        case "3": SearchJobs(); break;
                        case "4": SortJobs(); break;
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
        private void ListMyJobs(Recruiter recruiter)
        {
            List<Job> mine = jobService.Find(job => job.CompanyId == recruiter.CompanyId);
            if (mine.Count == 0)
            {
                Console.WriteLine("Your company has no jobs yet.");
                return;
            }
            PrintJobs(mine);
        }
        private void ShowJob()
        {
            if (!TryReadId("Job Id: ", out int id)) return;
            Job job = FindJob(id);
            if (job == null) return;

            Console.WriteLine();
            Console.WriteLine($"Job {job.Id}: {job.Title}");
            Console.WriteLine($"Company:     {CompanyName(job.CompanyId)}");
            Console.WriteLine($"Salary:      {job.Salary:N0}");
            Console.WriteLine($"Location:    {job.Location}");
            Console.WriteLine($"Open:        {(job.IsOpen ? "Yes" : "No")}");
            Console.WriteLine($"Posted:      {job.PostedDate:yyyy-MM-dd}");
            Console.WriteLine($"Description: {job.Description}");
        }
        private void AddMyJob(Recruiter recruiter)
        {
            if (!TryReadText("Title: ", out string title)) return;
            string description = ReadOptionalText("Description: ");
            if (!TryReadDecimal("Salary: ", out decimal salary)) return;
            string location = ReadOptionalText("Location: ");

            Job job = new Job
            {
                Title = title,
                Description = description,
                Salary = salary,
                Location = location,
                IsOpen = true,
                PostedDate = DateTime.Now,
                CompanyId = recruiter.CompanyId,      // always the recruiter's own company
                RecruiterId = recruiter.Id
            };
            jobService.Add(job);                      // refused if the company is not Approved
            Console.WriteLine($"Job added with Id {job.Id}.");
        }

        private void UpdateMyJob(Recruiter recruiter)
        {
            if (!TryReadId("Job Id to update: ", out int id)) return;
            Job existing = FindJob(id);
            if (existing == null) return;
            jobService.EnsureOwnedBy(existing, recruiter);      // refuse before asking for any field

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
            jobService.Update(updated, recruiter);
            Console.WriteLine("Job updated.");
        }

        private void CloseMyJob(Recruiter recruiter)
        {
            if (!TryReadId("Job Id to close: ", out int id)) return;
            if (FindJob(id) == null) return;
            jobService.Close(id, recruiter);
            Console.WriteLine("Job closed.");
        }

        private void RemoveMyJob(Recruiter recruiter)
        {
            if (!TryReadId("Job Id to remove: ", out int id)) return;
            Job job = FindJob(id);
            if (job == null) return;
            jobService.EnsureOwnedBy(job, recruiter);

            if (!Confirm($"Remove job '{job.Title}'?"))
            {
                Console.WriteLine("Cancelled.");
                return;
            }
            applicationService.RemoveJob(id, recruiter);        // refused if the job has applications
            Console.WriteLine("Job removed.");
        }

        private void ListApplicationsForMyJobs(Recruiter recruiter)
        {
            List<Application> mine = new List<Application>();
            foreach (Application application in applicationService.GetAll())
            {
                try
                {
                    Job job = jobService.FindById(application.JobId);
                    if (job.CompanyId == recruiter.CompanyId)
                        mine.Add(application);
                }
                catch (ArgumentException) { }                   // the job no longer exists: skip it
            }
            PrintApplications(mine);
        }
        private void ChangeStatusTo(CompanyStatus newStatus, string pastTense)
        {
            if (!TryReadId("Company Id: ", out int id)) return;
            Company company = FindCompany(id);
            if (company == null) return;

            Console.WriteLine($"Current status: {company.Status}");
            companyService.ChangeStatus(id, newStatus);     // refuses changes the rules forbid, with a clear message
            Console.WriteLine($"Company '{company.Name}' {pastTense}.");
        }
        private CurrentUser SelectUser()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("===== CONTINUE AS =====");
                Console.WriteLine("1. Admin");
                Console.WriteLine("2. Recruiter");
                Console.WriteLine("3. Candidate");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                switch (Input())
                {
                    case "1":
                        return new CurrentUser(UserRole.Admin, null, "Admin");
                    case "2":
                        {
                            CurrentUser user = PickRecruiter();
                            if (user != null) return user;
                            break;
                        }
                    case "3":
                        {
                            CurrentUser user = PickCandidate();
                            if (user != null) return user;
                            break;
                        }
                    case "0":
                        return null;
                    default:
                        Console.WriteLine("Invalid option. Enter 0, 1, 2 or 3.");
                        break;
                }
            }
        }

        private CurrentUser PickRecruiter()
        {
            List<Recruiter> recruiters = recruiterService.GetAll();
            if (recruiters.Count == 0)
            {
                Console.WriteLine("There are no recruiters yet.");
                return null;
            }

            PrintRecruiters(recruiters);
            if (!TryReadId("Your recruiter Id: ", out int id)) return null;

            Recruiter recruiter = FindRecruiter(id);      // prints "No recruiter with Id ... was found."
            if (recruiter == null) return null;

            return new CurrentUser(UserRole.Recruiter, recruiter.Id, recruiter.Name);
        }
       

        private CurrentUser PickCandidate()
        {
            List<Candidate> candidates = candidateService.GetAll();
            if (candidates.Count == 0)
            {
                Console.WriteLine("There are no candidates yet.");
                return null;
            }

            PrintCandidates(candidates);
            if (!TryReadId("Your candidate Id: ", out int id)) return null;

            Candidate candidate = FindCandidate(id);
            if (candidate == null) return null;

            return new CurrentUser(UserRole.Candidate, candidate.Id, candidate.FullName);
        }
        //==================================FINDERS=========================================
        private Recruiter FindRecruiter(int id)
        {
            try { return recruiterService.GetById(id); }
            catch (ArgumentException)
            {
                Console.WriteLine($"No recruiter with Id {id} was found.");
                return null;
            }
        }
        private Company FindCompany(int id)
        {
            try { return companyService.GetById(id); }
            catch (ArgumentException)
            {
                Console.WriteLine($"No company with Id {id} was found.");
                return null;
            }
        }
        private Candidate FindCandidate(int id)
        {
            try { return candidateService.GetById(id); }
            catch (ArgumentException)
            {
                Console.WriteLine($"No candidate with Id {id} was found.");
                return null;
            }
        }
        private Job FindJob(int id)
        {
            try { return jobService.GetById(id); }       // E5: Dictionary lookup
            catch (ArgumentException)
            {
                Console.WriteLine($"No job with Id {id} was found.");
                return null;
            }
        }
        private Application FindApplication(int id)
        {
            try { return applicationService.GetById(id); }
            catch (ArgumentException)
            {
                Console.WriteLine($"No application with Id {id} was found.");
                return null;
            }
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
                case "7": currentUser = null; break;     // back to "Continue as" on the next round
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
                Console.WriteLine("1. List comapies");
                Console.WriteLine("2. Change company status (approve / reject)");
                Console.WriteLine("3. List comapies by status");
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
                        default: Console.WriteLine("Invalid option. Try again and enter 0 or 1"); break;
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

            Job job = FindJob(id);
            if (job == null) return;

            if (applicationService.HasApplications(id))
            {
                Console.WriteLine("Cannot remove this job because it has existing applications.");
                return;
            }

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

        private void SearchJobs()
        {
            string keyword = ReadOptionalText("Keyword in the title (Enter to skip): ");
            string salaryText = ReadOptionalText("Minimum salary (Enter to skip): ");

            bool hasKeyword = keyword.Length > 0;
            bool hasSalary = salaryText.Length > 0;

            if (!hasKeyword && !hasSalary)
            {
                Console.WriteLine("Enter a keyword, a minimum salary, or both.");
                return;
            }

            decimal minSalary = 0;
            if (hasSalary)
            {
                if (!decimal.TryParse(salaryText, out minSalary))
                {
                    Console.WriteLine($"'{salaryText}' is not a valid number.");
                    return;
                }
                if (minSalary < 0)
                {
                    Console.WriteLine("The minimum salary cannot be negative.");
                    return;
                }
            }

            List<Job> matches = jobService.Find(job =>
                (!hasKeyword || (job.Title != null &&
                                 job.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                && (!hasSalary || job.Salary >= minSalary));

            if (matches.Count == 0)
            {
                Console.WriteLine("No jobs match your search.");
                return;
            }

            Console.WriteLine($"{matches.Count} job(s) found:");
            PrintJobs(matches);
        }

        private void SortJobs()
        {
            Console.WriteLine("Sort by:");
            Console.WriteLine("1. Salary, low to high");
            Console.WriteLine("2. Salary, high to low");
            Console.WriteLine("3. Title, A to Z");
            if (!TryReadInt("Choose: ", out int choice)) return;

            Comparison<Job> comparison;
            string description;

            switch (choice)
            {
                case 1:
                    description = "salary, low to high";
                    comparison = (a, b) =>
                    {
                        int result = a.Salary.CompareTo(b.Salary);
                        return result != 0 ? result : a.Id.CompareTo(b.Id);   // tie: lower Id first
                    };
                    break;
                case 2:
                    description = "salary, high to low";
                    comparison = (a, b) =>
                    {
                        int result = b.Salary.CompareTo(a.Salary);            // sides swapped
                        return result != 0 ? result : a.Id.CompareTo(b.Id);
                    };
                    break;
                case 3:
                    description = "title, A to Z";
                    comparison = (a, b) =>
                    {
                        int result = string.Compare(a.Title, b.Title, StringComparison.OrdinalIgnoreCase);
                        return result != 0 ? result : a.Id.CompareTo(b.Id);
                    };
                    break;
                default:
                    Console.WriteLine("Invalid choice. Enter 1, 2 or 3.");
                    return;
            }

            List<Job> sorted = jobService.Sort(comparison);    // a sorted copy, the stored jobs are untouched

            Console.WriteLine($"Jobs sorted by {description}:");
            PrintJobs(sorted);
        }
        
        private void ViewAvailableJobs()
        {
            List<Job> available = new List<Job>();
            foreach (Job job in jobService.GetAll())
            {
                if (IsAvailable(job))
                    available.Add(job);
            }

            if (available.Count == 0)
            {
                Console.WriteLine("No jobs are open for applications right now.");
                return;
            }
            PrintJobs(available);
        }

        private void ApplyForJob()
        {
            if (!TryReadId("Candidate Id: ", out int candidateId)) return;
            if (FindCandidate(candidateId) == null) return;

            if (!TryReadId("Job Id: ", out int jobId)) return;
            if (FindJob(jobId) == null) return;

            applicationService.Apply(candidateId, jobId);   // closed job, company not Approved, duplicate -> clear message
            Console.WriteLine("Application submitted successfully!");
        }

        private void RemoveApplication()
        {
            if (!TryReadId("Application Id to remove: ", out int id)) return;

            Application application;
            try
            {
                application = applicationService.GetById(id);
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"No application with Id {id} was found.");
                return;
            }

            if (!Confirm("Remove this application?"))
            {
                Console.WriteLine("Cancelled.");
                return;
            }
            applicationService.Remove(application);
            Console.WriteLine("Application removed.");
        }

        private void PrintApplications(List<Application> applications)
        {
            if (applications.Count == 0)
            {
                Console.WriteLine("No applications found.");
                return;
            }

            Console.WriteLine("{0,-5}{1,-20}{2,-22}{3,-12}{4,-12}", "Id", "Candidate", "Job", "Date", "Status");
            Console.WriteLine(new string('-', 71));
            foreach (Application a in applications)
            {
                Console.WriteLine("{0,-5}{1,-20}{2,-22}{3,-12:yyyy-MM-dd}{4,-12}",
                    a.Id, Cut(CandidateName(a.CandidateId), 19), Cut(JobTitle(a.JobId), 21),
                    a.AppliedAt, a.Status);
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
  

private string CandidateName(int candidateId)
{
    try { return candidateService.GetById(candidateId).FullName; }
    catch (ArgumentException) { return "Unknown"; }
}

private string JobTitle(int jobId)
{
    try { return jobService.FindById(jobId).Title; }
    catch (ArgumentException) { return "Unknown"; }
}

private bool IsAvailable(Job job)
{
    if (!job.IsOpen) return false;
    try { return companyService.IsApproved(job.CompanyId); }
    catch (ArgumentException) { return false; }
}
        private void CandidatesMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n===== CANDIDATES =====");
                Console.WriteLine("1. Add a candidate");
                Console.WriteLine("2. Update candidate");
                Console.WriteLine("3. List candidates");
                Console.WriteLine("4. Remove candidate");
                Console.WriteLine("0. Back to main menu");
                Console.Write("Choose an option: ");

                try
                {
                    switch (Input())
                    {
                        case "1": AddCandidate(); break;
                        case "2": UpdateCandidate(); break;
                        case "3": PrintCandidates(candidateService.GetAll()); break;
                        case "4": RemoveCandidate(); break;
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
        private void JobsMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n===== JOBS =====");
                Console.WriteLine("1. Add a job");
                Console.WriteLine("2. Update job");
                Console.WriteLine("3. List jobs");
                Console.WriteLine("4. Remove job");
                Console.WriteLine("5. Close a job");
                Console.WriteLine("6. Search jobs by title/ and by minimum salary");
                Console.WriteLine("7. Sort jobs");
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
                        case "6": SearchJobs(); break;
                        case "7": SortJobs(); break;
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
        private void ApplicationsMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n===== APPLICATIONS =====");
                Console.WriteLine("1. View available jobs");
                Console.WriteLine("2. Apply for a job");
                Console.WriteLine("3. List applications");
                Console.WriteLine("4. Remove an application");
                Console.WriteLine("0. Back to main menu");
                Console.Write("Choose an option: ");

                try
                {
                    switch (Input())
                    {
                        case "1": ViewAvailableJobs(); break;
                        case "2": ApplyForJob(); break;
                        case "3": PrintApplications(applicationService.GetAll()); break;
                        case "4": RemoveApplication(); break;
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
        private void RecruitersMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n===== RECRUITERS =====");
                Console.WriteLine("1. Add a recruiter");
                Console.WriteLine("2. Update a recruiter");
                Console.WriteLine("3. List recruiters");
                Console.WriteLine("4. Remove a recruiter");
                Console.WriteLine("0. Back to main menu");
                Console.Write("Choose an option: ");

                try
                {
                    switch (Input())
                    {
                        case "1": AddRecruiter(); break;
                        case "2": UpdateRecruiter(); break;
                        case "3": PrintRecruiters(recruiterService.GetAll()); break;
                        case "4": RemoveRecruiter(); break;
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
        private void AddRecruiter()
        {
            if (!TryReadText("Name: ", out string name)) return;
            if (!TryReadText("Email: ", out string email)) return;
            if (!TryReadId("Company Id: ", out int companyId)) return;
            if (companyService.GetById(companyId) == null) return;

            Recruiter recruiter = new Recruiter
            {
                Name = name,
                Email = email,             // an invalid email throws and the catch prints the message
                CompanyId = companyId
            };
            recruiterService.Add(recruiter);
            Console.WriteLine($"Recruiter added with Id {recruiter.Id}.");
        }

        private void UpdateRecruiter()
        {
            if (!TryReadId("Recruiter Id to update: ", out int id)) return;
            Recruiter existing = recruiterService.GetById(id);
            if (existing == null) return;

            Console.WriteLine("Press Enter to keep the current value.");
            string name = ReadKeepText("Name", existing.Name);
            string email = ReadKeepText("Email", existing.Email);
            if (!TryReadKeepInt("Company Id", existing.CompanyId, out int companyId)) return;
            if (companyService.GetById(companyId) == null) return;

            Recruiter updated = new Recruiter
            {
                Id = existing.Id,
                Name = name,
                Email = email,
                CompanyId = companyId
            };
            recruiterService.Update(updated);
            Console.WriteLine("Recruiter updated.");
        }

        private void RemoveRecruiter()
        {
            if (!TryReadId("Recruiter Id to remove: ", out int id)) return;
            Recruiter recruiter = recruiterService.GetById(id);
            if (recruiter == null) return;

            if (!Confirm($"Remove recruiter '{recruiter.Name}'?"))
            {
                Console.WriteLine("Cancelled.");
                return;
            }
            recruiterService.Remove(recruiter);
            Console.WriteLine("Recruiter removed.");
        }

        private void PrintRecruiters(List<Recruiter> recruiters)
        {
            if (recruiters.Count == 0)
            {
                Console.WriteLine("No recruiters found.");
                return;
            }

            Console.WriteLine("{0,-5}{1,-20}{2,-30}{3,-18}", "Id", "Name", "Email", "Company");
            Console.WriteLine(new string('-', 73));
            foreach (Recruiter r in recruiters)
            {
                Console.WriteLine("{0,-5}{1,-20}{2,-30}{3,-18}",
                    r.Id, Cut(r.Name, 19), Cut(r.Email, 29), Cut(CompanyName(r.CompanyId), 17));
            }
        }
        private void AddCandidate()
        {
            if (!TryReadText("Full name: ", out string fullName)) return;
            if (!TryReadText("Email: ", out string email)) return;

            Candidate candidate = new Candidate
            {
                FullName = fullName,
                Email = email              // an invalid email throws and the catch prints the message
            };
            candidateService.Add(candidate);
            Console.WriteLine($"Candidate added with Id {candidate.Id}.");
        }

        private void UpdateCandidate()
        {
            if (!TryReadId("Candidate Id to update: ", out int id)) return;
            Candidate existing = candidateService.GetById(id);
            if (existing == null) return;

            Console.WriteLine("Press Enter to keep the current value.");
            Candidate updated = new Candidate
            {
                Id = existing.Id,
                FullName = ReadKeepText("Full name", existing.FullName),
                Email = ReadKeepText("Email", existing.Email)
            };
            candidateService.Update(updated);
            Console.WriteLine("Candidate updated.");
        }

        private void RemoveCandidate()
        {
            if (!TryReadId("Candidate Id to remove: ", out int id)) return;
            Candidate candidate = candidateService.GetById(id);
            if (candidate == null) return;

            if (!Confirm($"Remove candidate '{candidate.FullName}'?"))
            {
                Console.WriteLine("Cancelled.");
                return;
            }
            candidateService.Remove(candidate);
            Console.WriteLine("Candidate removed.");
        }

        private void PrintCandidates(List<Candidate> candidates)
        {
            if (candidates.Count == 0)
            {
                Console.WriteLine("No candidates found.");
                return;
            }

            Console.WriteLine("{0,-5}{1,-22}{2,-32}", "Id", "Name", "Email");
            Console.WriteLine(new string('-', 59));
            foreach (Candidate c in candidates)
            {
                Console.WriteLine("{0,-5}{1,-22}{2,-32}",
                    c.Id, Cut(c.FullName, 21), Cut(c.Email, 31));
            }
        }
    }
}