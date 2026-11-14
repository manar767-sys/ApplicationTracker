# ApplicationTracker
A job portal in the console. Companies are approved by an admin, recruiters post jobs for their company, and candidates apply. The project practises collections, generics, delegates, events, lambdas, file handling and exceptions.

## About the project
This is Part 1 (Sprint 1) of the project: the data foundation of the portal. Data lives in memory while the app runs and is loaded from fixed seed data at start: 3 companies, 3 recruiters, 6 jobs and 5 candidates.

### Business rules
- A candidate can apply only to a job that is open and belongs to a company with status Approved.
- A candidate cannot apply to the same job twice.
- A Pending or Rejected company cannot post new jobs.
- A recruiter can create, edit, close and remove jobs of their own company only (planned for Sprint 2).

### Company status changes
| From | To | Allowed |
|---|---|---|
| Pending | Approved or Rejected | Yes |
| Approved | Pending | Yes |
| Approved | Rejected | No |
| Rejected | Any | No |

Every status change is saved on a Stack, so the last change can be undone.

### Data model
| Entity | Fields |
|---|---|
| Company | Id, Name, Industry, City, Status (Pending, Approved, Rejected) |
| Recruiter | Id, Name, Email, CompanyId |
| Job | Id, Title, Description, Salary, Location, IsOpen, PostedDate, CompanyId, RecruiterId |
| Candidate | Id, FullName, Email, Phone, YearsOfExperience |
| Application | Id, JobId, CandidateId, AppliedDate, Status (Submitted, UnderReview, Interview, Accepted, Rejected) |

Every model implements `IEntity`, which guarantees an `Id`.

### Design
- **Generic store:** `DataStore<T> where T : class, IEntity` provides Add, Update, Remove, GetById and GetAll. The store assigns each item a unique Id from a counter that is never reused.
- **Services:** `CompanyService`, `RecruiterService`, `JobService`, `CandidateService` and `ApplicationService` inherit from `DataStore<T>` and override methods where a business rule applies.
- **Dictionary (E5):** `JobService` keeps a `Dictionary<int, Job>` in sync with the job list and uses it to find a job by Id.
- **HashSet (H1):** `ApplicationService` keeps a `HashSet<(int JobId, int CandidateId)>` to block duplicate applications.
- **Stack (H2):** `CompanyService` keeps a `Stack` of (company Id, old status) to undo status changes.
- **Errors:** a refused action throws an exception with a clear message, and the menus catch it and print the message. "Not found" always throws.
- **Sprint rule:** no LINQ and no delegates in this sprint.

### Project structure
```
ApplicationTracker/
├── Models/        Company, Recruiter, Job, Candidate, Application, IEntity, enums
├── Repository/    IDataStore<T>
├── Data/          DataStore<T>, SeedingData
├── Services/      CompanyService, RecruiterService, JobService, CandidateService, ApplicationService
├── Menu/          MenuHandler (console menus)
└── Program.cs     creates the services once, loads the seed data, starts the menu
```

## Setup and run

### Requirements
- .NET SDK 10 (the project targets `net10.0`)
- Git
- Visual Studio 2022 or later (optional)

### Setup
```
git clone <repository-url>
cd ApplicationTracker
```

### Run from the command line
```
dotnet build
dotnet run
```

### Run from Visual Studio
Open the `.sln` file and press F5.

### Using the app
The main menu lists the entities. Choose a number and press Enter. Wrong input shows a message and the menu returns, so the app does not crash.

## Test scenarios
- **Scenario 1:** on startup the seed data exists (3 companies, 3 recruiters, 6 jobs, 5 candidates) and every list menu shows it.
- Add the remaining scenarios (2 to 7) here as you run them.

## Status (Sprint 1)
- [x] E1: sample data loaded at start
- [ ] E2: menus
- [x] E3: company approval, list by status
- [x] E4 and H1: apply with refusal rules and duplicate check
- [x] E5: Dictionary lookup for jobs
- [x] M1: generic `DataStore<T>`
- [x] H2: undo company status changes
- Next (Sprint 2): E6 and the Medium level (events, lambdas, file saving, custom exceptions)

## Git workflow

git commands i used in terminal using Cmder

```
git clone <url>
dotnet new gitignore           to create .gitignore file
git status
git commit -m "E1: seed data loaded"
git push
git pull
git reset --hard
git commit --amend -m "your new commit message"      to change the last commit message
```

- `.gitignore` is created with `dotnet new gitignore`, and it excludes `bin/` and `obj/`.
- Commit messages start with the requirement (E1, E3, H1...) so the history shows one commit per requirement.
- The end of the sprint is tagged `sprint-1`.(not yet pushed to remote)

## Known limitations
- Data is kept in memory only and is lost when the app closes (file saving is Sprint 2).
- Candidate email uniqueness (`HashSet<string>`) is not finished yet.
- Recruiter ownership of jobs is planned for Sprint 2.
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

“ISP violation in the interface design caused the LSP violation in the implementation.” Fix the first by splitting or narrowing interfaces, and the second disappears.

<img src="https://drive.google.com/file/d/1sOf-wiTTT2qT66tbQofiS3r709lmmte0/view?usp=sharing" alt="Liskov Substitution Principle" width="600"/>