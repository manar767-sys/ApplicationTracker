# ApplicationTracker
A job portal in the console. Companies are approved by an admin, recruiters post jobs for their company, and candidates apply. The project practises collections, generics, delegates, events, lambdas, file handling and exceptions. Sprint 1 covers collections, generics and exceptions; delegates, events, lambdas and file handling come in Sprint 2.

## About the project
This is Part 1 (Sprint 1) of the project: the data foundation of the portal. Data lives in memory while the app runs and is loaded from fixed seed data at start: 3 companies, 3 recruiters, 6 jobs and 5 candidates.

## Features

### Requirement 1: Find a Job
A user can search for a job using its ID. If a job with that ID exists, the system returns its details. Otherwise, the system reports that the job was not found.

### Requirement 2: List Companies by Status
A user can request companies with a particular status. The system returns all companies whose status matches the requested status. If none match, the result is empty.

### Requirement 3: Apply to a Job
A candidate can apply to a job by providing their candidate ID and the job ID.

The candidate and job must exist. The job must be open, and its company must have Approved status. A candidate cannot apply to the same job more than once.

If any rule fails, the system must explain why the application was rejected. A rejected attempt must not be saved. If all rules pass, the application is saved and the system reports success.

### Requirement 4: Check Email Availability
A user can check whether an email address is available for a new candidate. Email comparison is case-insensitive. The result is available only if no existing candidate uses that email.

## Business rules
- A candidate can apply only to a job that is open and belongs to a company with status Approved.
- A candidate cannot apply to the same job twice.
- A Pending or Rejected company cannot post new jobs.
- A candidate email must be unique, ignoring letter case.
- A recruiter can create, edit, close and remove jobs of their own company only (planned for Sprint 2).

### Company status changes
| From | To | Allowed |
|---|---|---|
| Pending | Approved or Rejected | Yes |
| Approved | Pending or Rejected | Yes |
| Rejected | Any | No (Rejected is final) |

Every status change is saved on a Stack, so the last change can be undone. Undo restores the previous status, including a change the normal rules would not allow.

## Data model
| Entity | Fields |
|---|---|
| Company | Id, Name, Industry, City, Status (Pending, Approved, Rejected) |
| Recruiter | Id, Name, Email, CompanyId |
| Job | Id, Title, Description, Salary, Location, IsOpen, PostedDate, CompanyId, RecruiterId |
| Candidate | Id, FullName, Email, Phone, YearsOfExperience |
| Application | Id, JobId, CandidateId, AppliedDate, Status (Submitted, UnderReview, Interview, Accepted, Rejected) |

Every model implements `IEntity`, which guarantees an `Id`.

## Design
- **Generic store:** `DataStore<T> where T : class, IEntity` provides Add, Update, Remove, GetById and GetAll. The store assigns each item a unique Id from a counter that is never reused.
- **Services:** `CompanyService`, `RecruiterService`, `JobService`, `CandidateService` and `ApplicationService` inherit from `DataStore<T>` and override methods where a business rule applies.
- **Dictionary (E5):** `JobService` keeps a `Dictionary<int, Job>` in sync with the job list and uses it to find a job by Id.
- **HashSet of pairs (H1):** `ApplicationService` keeps a `HashSet<(int JobId, int CandidateId)>` to block duplicate applications.
- **HashSet of emails:** `CandidateService` keeps a case-insensitive `HashSet<string>` to block duplicate candidate emails.
- **Stack (H2):** `CompanyService` keeps a `Stack` of (company Id, old status) to undo status changes.
- **Errors:** a refused action throws an exception with a clear message, and the menus catch it and print the message. "Not found" always throws.
- **Input:** menus read numbers with `int.TryParse` and `decimal.TryParse`, so wrong input prints a message and returns to the menu instead of crashing.

## Project structure
```
ApplicationTracker/
├── Models/        Company, Recruiter, Job, Candidate, Application, IEntity, enums
├── Repository/    IDataStore<T>, DataStore<T>
├── Data/          SeedingData
├── Services/      CompanyService, RecruiterService, JobService, CandidateService, ApplicationService
├── Menu/          MenuHandler (console menus)
└── Program.cs     creates the services once, loads the seed data, starts the menu
```

## Setup and run

### Requirements
- .NET SDK 10 (the project targets `net10.0`)
- Git
- Visual Studio 2022 or later 

### Setup
```
git clone <repository-url>


### Run from the command line
```
dotnet build
dotnet run
```

### Run from Visual Studio
Open the `.sln` file and press F5.

### Using the app
The main menu has one sub-menu per entity (Companies, Recruiters, Jobs, Candidates, Applications). Each has Add, Update, List and Remove, and Back returns to the main menu. Choose a number and press Enter. Wrong input shows a message and the menu returns, so the app does not crash.

## Test scenarios
- **Scenario 1:** on startup the seed data exists (3 companies, 3 recruiters, 6 jobs, 5 candidates) and every list menu shows it.
- **Scenario 2:** adding a job to an Approved company succeeds, and the job can be found by Id through the Dictionary.
- **Scenario 3:** adding a job to a Pending or Rejected company is refused with a clear message.
- **Scenario 4:** adding a candidate whose email already exists (even with different letter case) is refused.
- **Scenario 5:** updating and removing a job keeps the Dictionary consistent, and typing letters instead of numbers in any menu does not crash the app.
- **Scenario 6:** a candidate can apply to an open job of an Approved company, applying again is refused, and applying to a closed job is refused.
- **Scenario 7:** change Company 3 from Pending to Approved, then to Rejected, then Undo twice (Approved, then Pending). A third Undo shows a friendly message and does not crash.

## Status (Sprint 1)
- [x] E1: sample data loaded at start
- [x] E2: menus
- [x] E3: company approval, list by status
- [x] E4 and H1: apply with refusal rules and duplicate check
- [x] E5: Dictionary lookup for jobs
- [x] M1: generic `DataStore<T>`
- [x] H2: undo company status changes
- [x] Candidate email uniqueness (`HashSet<string>`)

## Git workflow

Create the repository and connect it from the command line:
```
git init
dotnet new gitignore           # creates .gitignore (excludes bin/ and obj/)
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin <url>
git push -u origin main
```

Commands I used in the terminal (Cmder):
```
git clone <url>
git status
git add .
git commit -m "E1: seed data loaded"
git push
git pull
git log --oneline
git commit --amend -m "your new commit message"        # change the last commit message
git reset --hard                                       # WARNING: throws away all uncommitted changes


## Known limitations
- Data is kept in memory only and is lost when the app closes (file saving is Sprint 2).
- Recruiter ownership of jobs is planned but not yet implemented
- Removing a company, recruiter, job or candidate does not remove the records that point to it.
- If a company is removed after a status change, undoing that change fails.

## Session One Assignment (Bonus Question)
what soild pricple does the code below break? explain

```csharp
    static void Main()
    {
        IList<int> values = new int[] { 10, 20, 30 };

        values[0] = 100; // Works: arrays allow replacing elements
        Console.WriteLine($"values[0] = {values[0]}");

        try { values.Add(40); }
        catch (NotSupportedException ex) { Console.WriteLine($"Add: {ex.Message}"); }

        try { values.Insert(1, 15); }
        catch (NotSupportedException ex) { Console.WriteLine($"Insert: {ex.Message}"); }

        try { values.RemoveAt(0); }
        catch (NotSupportedException ex) { Console.WriteLine($"RemoveAt: {ex.Message}"); }

        Console.WriteLine($"IsFixedSize via array: {((Array)values).IsFixedSize}");
        Console.WriteLine($"IsReadOnly: {values.IsReadOnly}"); // True for arrays via IList<T>
    }
```

Answer:

The code above breaks the **Liskov Substitution Principle (LSP)**, which is one of the SOLID principles of object-oriented design. The LSP states that objects of a superclass should be replaceable with objects of a subclass without affecting the correctness of the program.

Here, `IList<int>` promises Add, Insert and RemoveAt, but an array has a fixed size, so those three calls throw `NotSupportedException`. Code that accepts an `IList<int>` cannot tell that it is holding an array, so substituting the array for the interface breaks the program.

The root cause is the interface design. `IList<T>` is too wide: it bundles methods that not every implementation can support. This is an Interface Segregation Principle (ISP) problem, and it is what forces the LSP violation. The fix for the first is to split or narrow the interface, and the second disappears.
