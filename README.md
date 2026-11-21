# ApplicationTracker
A job portal in the console. Companies are approved by an admin, recruiters post jobs for their company, and candidates apply. The project practises collections, generics, delegates, events, lambdas, file handling and exceptions.

## About the project
Sprint 1 built the data foundation: models, a generic store, the services and the console menus. Sprint 2 adds role-based menus, job search and sort with delegates, custom exceptions and events. Data lives in memory while the app runs and is loaded from fixed seed data at start: 3 companies, 3 recruiters, 6 jobs and 5 candidates.

## Features

### Find a job
A user can look up a job by its Id. If it exists, the job's details are shown. Otherwise the app reports that the job was not found. The lookup uses a `Dictionary<int, Job>` kept in sync with the job list.

### Company approval
The admin can list companies waiting for approval, approve or reject a company by Id, list approved companies, list rejected companies, and undo the last status change.

### Apply to a job
A candidate applies by choosing a job (the candidate is the logged-in user). The candidate and the job must exist, the job must be open, its company must be Approved, and the candidate cannot apply to the same job twice. If a rule fails, the app explains why, and nothing is saved.

### Check email availability
A candidate email must be unique, ignoring letter case. A case-insensitive `HashSet<string>` blocks duplicates when a candidate is added or updated.

### Search and sort jobs
- **Search** by a keyword in the title, by a minimum salary, or by both at once. Each condition is a `Predicate<Job>` passed to `DataStore<T>.Find`.
- **Sort** by salary (low to high or high to low) or by title. Each ordering is a `Comparison<Job>` passed to `DataStore<T>.Sort`, which returns a sorted copy and never changes the stored list.

### Review applications
A recruiter can move an application of their own company through its statuses.

### Notifications (events)
The app raises three events and prints a notification for each. Notifications can be switched on and off from the menu.

## Roles and menus
There is no login. At startup the user chooses a role, and "Switch user" returns to that screen.

| Role | What they can do |
|---|---|
| Admin | Company approval (lists by status, approve, reject, undo), manage companies, recruiters and candidates, read-only job lists, search and sort, list applications |
| Recruiter | Their own company's jobs only: list, add, update, close, remove. Search and sort jobs, list applications for their jobs, review an application |
| Candidate | Available jobs, find a job by Id, search and sort, apply, list their own applications |

The admin is a role, not a stored entity, so there is no `Admin` class.

## Business rules
- A candidate can apply only to a job that is open and belongs to a company with status Approved.
- A candidate cannot apply to the same job twice.
- A Pending or Rejected company cannot post new jobs.
- A recruiter can create, edit, close and remove jobs of their own company only.
- A job that has applications cannot be removed (it can be closed instead).
- A candidate email must be unique, ignoring letter case.
- Approval happens at the company level. A job posted by a recruiter of an Approved company is live immediately; jobs have no approval step.

### Company status changes
| From | To | Allowed |
|---|---|---|
| Pending | Approved or Rejected | Yes |
| Approved | Pending or Rejected | Yes |
| Rejected | Any | No (Rejected is final) |

Every status change is saved on a Stack, so the last change can be undone. Undo restores the previous status, including a change the normal rules would not allow, and it does not raise the approval event.

### Application status changes
| From | To | Allowed |
|---|---|---|
| Submitted | UnderReview | Yes |
| UnderReview | Accepted or Rejected | Yes |
| Accepted, Rejected | Any | No (final) |

Only a recruiter of the job's company can change an application's status.

## Data model
| Entity | Fields |
|---|---|
| Company | Id, Name, Industry, City, Status (Pending, Approved, Rejected) |
| Recruiter | Id, Name, Email, CompanyId |
| Job | Id, Title, Description, Salary, Location, IsOpen, PostedDate, CompanyId, RecruiterId |
| Candidate | Id, FullName, Email |
| Application | Id, JobId, CandidateId, AppliedAt, Status (Submitted, UnderReview, Accepted, Rejected) |

Every model implements `IEntity`, which guarantees an `Id`.

## Design
- **Generic store:** `DataStore<T> where T : class, IEntity` provides Add, Update, Remove, GetById, GetAll, `Find(Predicate<T>)` and `Sort(Comparison<T>)`. The store assigns each item a unique Id from a counter that is never reused. `Sort` returns a copy, so the stored order never changes.
- **Services:** `CompanyService`, `RecruiterService`, `JobService`, `CandidateService` and `ApplicationService` inherit from `DataStore<T>` and override methods where a business rule applies.
- **Dictionary:** `JobService` keeps a `Dictionary<int, Job>` in sync with the job list and uses it to find a job by Id.
- **HashSet of pairs:** `ApplicationService` keeps a `HashSet<(int JobId, int CandidateId)>`. The return value of `Add` detects a duplicate, and removing an application frees its pair.
- **HashSet of emails:** `CandidateService` keeps a case-insensitive `HashSet<string>`.
- **Stack:** `CompanyService` keeps a `Stack` of (company Id, old status) to undo status changes.
- **Ownership:** `JobService` and `ApplicationService` refuse an action when the job's company is not the acting recruiter's company. The check lives in the services, not only in the menus.
- **Custom exceptions:** `CompanyNotApprovedException`, `JobClosedException` and `DuplicateApplicationException` inherit from `InvalidOperationException`. They are thrown by the services and caught by the menus, with the specific `catch` blocks before the general one.
- **Errors and input:** menus read numbers with `int.TryParse` and `decimal.TryParse`, so wrong input prints a message and returns to the menu instead of crashing. "Not found" always throws, and the menus turn it into a message that names the Id.

### Delegates and events
| Type | Used for | Where |
|---|---|---|
| `Predicate<T>` | A yes/no test on one item | `Find`, the job search conditions |
| `Comparison<T>` | Deciding which of two items comes first | `Sort`, the job sorts |
| `EventHandler<TEventArgs>` | Notifying others when something happened | The three events below |

| Event | Raised by | When | Data |
|---|---|---|---|
| `CompanyApproved` | `CompanyService.ChangeStatus` | After a company has been approved | The company |
| `ApplicationSubmitted` | `ApplicationService.Apply` | After an application is saved | The application, the job and the candidate |
| `ApplicationStatusChanged` | `ApplicationService.ChangeStatus` | After an application's status changed | The application, the old status and the new status |

Events are raised only after a successful operation, so a refused action never sends a notification. `Notifier` subscribes with named method handlers and one lambda handler (a counter of submitted applications). `Subscribe` and `Unsubscribe` switch all handlers on and off together, and the "Toggle notifications" menu option uses them.

## Project structure
```
ApplicationTracker/
├── Models/        Company, Recruiter, Job, Candidate, Application, IEntity, enums, UserRole
├── Repository/    IDataStore<T>, DataStore<T>
├── Data/          SeedingData
├── Services/      CompanyService, RecruiterService, JobService, CandidateService, ApplicationService
├── Events/        EventArgs classes, Notifier
├── Exceptions/    CompanyNotApprovedException, JobClosedException, DuplicateApplicationException
├── Menu/          MenuHandler, CurrentUser
└── Program.cs     creates the services once, loads the seed data, subscribes the notifier, starts the menu
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
Choose "Continue as" Admin, Recruiter or Candidate. Recruiters and candidates are picked from the seeded list by Id. Choose a number and press Enter. Wrong input shows a message and the menu returns, so the app does not crash.

## Test scenarios
- **Scenario 1:** on startup the seed data exists (3 companies, 3 recruiters, 6 jobs, 5 candidates) and every list menu shows it.
- **Scenario 2:** adding a job to an Approved company succeeds, and the job can be found by Id through the Dictionary.
- **Scenario 3:** adding a job to a Pending or Rejected company is refused with a clear message.
- **Scenario 4:** adding a candidate whose email already exists (even with different letter case) is refused.
- **Scenario 5:** updating and removing a job keeps the Dictionary consistent, and typing letters instead of numbers in any menu does not crash the app.
- **Scenario 6:** a candidate can apply to an open job of an Approved company, applying again is refused, and applying to a closed job is refused.
- **Scenario 7:** change Company 3 from Pending to Approved, then to Rejected, then Undo twice (Approved, then Pending). A third Undo shows a friendly message and does not crash.
- **Scenario 8:** (add the text of scenario 8 here)

## Status
- [x] E1: sample data loaded at start
- [x] E2: menus
- [x] E3: company approval, list by status
- [x] E4: admin operations on companies
- [x] E5: Dictionary lookup for jobs
- [x] E6: apply workflow, jobs with applications cannot be removed
- [x] M1: generic `DataStore<T>`
- [x] M2: events with named and lambda handlers, subscribe and unsubscribe
- [x] M3: `Find` with `Predicate<T>`, `Sort` with `Comparison<T>`, job search and sort
- [x] M4: duplicate application protection with a `HashSet`
- [x] M5: recruiters manage only their own company's jobs
- [x] M7: custom exceptions
- [x] H2: undo company status changes
- [ ] M6 and M8: saving and loading data from files

## Git workflow
Every day: pull, work, commit, push. One small commit per requirement, with the requirement ID at the start of the message.

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
git commit -m "M3: sort jobs by salary and title"
git push
git pull
git log --oneline
git commit --amend -m "your new commit message"      # change the last commit message
git reset --hard                                       # WARNING: throws away all uncommitted changes
```

Tag the end of each sprint:
```
git tag sprint-1
git push origin sprint-1
git tag sprint-2
git push origin sprint-2
```

## Known limitations
- Data is kept in memory only and is lost when the app closes.
- There is no login. A user picks a role and a person from a list, so the person at the keyboard is trusted.
- Nothing records who approved a company.
- Removing a company, recruiter, job or candidate does not remove the records that point to it (a job with applications cannot be removed, but other links are not checked).
- Search and sort show every job, including closed ones and jobs of companies that are not Approved.
- An event handler that throws would surface as an error in the operation that raised it, so handlers are kept trivial.

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

