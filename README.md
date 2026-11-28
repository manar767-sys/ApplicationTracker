# ApplicationTracker: Job Application Tracker

A C# console application that manages companies, recruiters, jobs, candidates and job applications. Companies are approved by an admin, recruiters post jobs for their company, and candidates apply.

- **Target framework:** .NET 10 (`net10.0`)
- **Project type:** Console application
- **No LINQ is used anywhere.**

---

## 1. Project overview

The Job Application Tracker models a small piece of a recruiting platform:

- Companies must be approved before their recruiters can post jobs.
- Recruiters belong to a company and manage that company's jobs only.
- Candidates browse available jobs and apply.
- The system prevents the same candidate from applying to the same job twice.
- Important events (a company approved, an application submitted, an application's status changed) produce console notifications.

Data lives in memory while the app runs and is loaded from fixed seed data at start: 3 companies, 3 recruiters, 6 jobs and 5 candidates.

---

## 2. Project scope and objectives

| ID | Area | Sprint | Status |
|----|------|--------|--------|
| E1 | Collections and sample data | 1 | Done |
| E2 | Console menus and CRUD | 1 | Done |
| E3 | Company approval, list by status | 1 | Done |
| E4 | Admin operations on companies | 2 | Done |
| E5 | Dictionary-based job lookup | 1 | Done |
| E6 | Apply workflow, jobs with applications cannot be removed | 2 | Done |
| M1 | Generic `DataStore<T>` | 1 | Done |
| M2 | Events, named and lambda handlers, subscribe and unsubscribe | 2 | Done |
| M3 | `Find` with `Predicate<T>`, `Sort` with `Comparison<T>`, job search and sort | 2 | Done |
| M4 | Duplicate application protection with a `HashSet` | 2 | Done |
| M5 | Recruiters manage only their own company's jobs | 2 | Done |
| M7 | Custom exceptions | 2 | Done |
| H1 | Apply workflow with duplicate guard | 1 | Done |
| H2 | Undo company status changes with a `Stack` | 1 | Done |
| C1 | Optional challenge: isolate failing event subscribers | 2 | Done |

**Not implemented:** file persistence and file error handling (M6 and M8). Data is lost when the app closes.

---

## 3. Folder structure

```
ApplicationTracker/
├── Models/        Company, Recruiter, Job, Candidate, Application, IEntity,
│                  CompanyStatus, ApplicationStatus, UserRole
├── Repository/    IDataStore<T>, DataStore<T>
├── Data/          SeedingData
├── Services/      CompanyService, RecruiterService, JobService,
│                  CandidateService, ApplicationService
├── Events/        EventArgs classes, Notifier, EventRaiser
├── Exceptions/    CompanyNotApprovedException, JobClosedException,
│                  DuplicateApplicationException
├── Menu/          MenuHandler, CurrentUser
├── Program.cs     creates each service once, seeds the data, subscribes
│                  the notifier, starts the menu
└── README.md
```

---

## 4. Main features

There is no login. At startup the user chooses a role ("Continue as"), and "Switch user" returns to that screen. The admin is a role, not a stored entity.

| Role | What they can do |
|------|------------------|
| **Admin** | Company approval (list waiting, approve, reject, list approved, list rejected, undo), manage companies, recruiters and candidates, read-only job lists, search and sort jobs, list applications |
| **Recruiter** | Their own company's jobs: list, add, update, close, remove. Search and sort jobs, list applications for their jobs, review an application |
| **Candidate** | Available jobs, find a job by Id, search and sort jobs, apply, list their own applications |

All roles can toggle notifications on and off.

- **Find a job** by Id uses the Dictionary lookup, and reports clearly when the Id does not exist.
- **Search jobs** by a keyword in the title, by a minimum salary, or by both at once.
- **Sort jobs** by salary (low to high or high to low) or by title. The stored order never changes.
- **Apply** is the only way an application is created, and it runs every business rule first.

---

## 5. Collections used and why

### `List<T>`
The underlying storage inside `DataStore<T>`. Ordered, indexable and growing, a good default for a sequence that is scanned or iterated.

### `Dictionary<int, Job>` (E5)
An index kept by `JobService` for fast lookup by Id (average O(1), against O(n) for scanning a list). It is kept in sync with the job list:
- `Add` adds to both
- `Update` replaces in both
- `Remove` removes from both

### `HashSet<(int JobId, int CandidateId)>` (H1, M4)
Records every accepted (job, candidate) pair. `HashSet<T>.Add` returns `false` when the element already exists, so duplicate detection is a single call with no loop. If saving fails after the pair was reserved, the pair is removed again. Removing an application frees its pair.

### `HashSet<string>` (candidate emails)
A case-insensitive set (`StringComparer.OrdinalIgnoreCase`) in `CandidateService`. `Add` and `Update` check and register the email together, and `Remove` frees it.

### `Stack<(int CompanyId, CompanyStatus OldStatus)>` (H2)
Records each company status change, so the most recent change is undone first (LIFO). Each entry holds the company Id and the previous status, not the company object. Removing a company also removes its entries.

---

## 6. Explanation of `DataStore<T>`

```csharp
public class DataStore<T> : IDataStore<T> where T : class, IEntity
{
    public virtual void Add(T item);
    public virtual void Update(T item);
    public virtual void Remove(T item);
    public virtual T GetById(int id);
    public virtual List<T> GetAll();
    public List<T> Find(Predicate<T> predicate);
    public List<T> Sort(Comparison<T> comparison);
}
```

- **Constraint `where T : class, IEntity`:** only reference types that expose an `Id` can be stored, so the generic code can read `item.Id`.
- **Storage:** a private list. `GetAll()` returns a copy, so callers cannot change the store.
- **Ids:** `Add` assigns each item the next Id from a counter that is never reused, so removing an item never creates a duplicate Id.
- **Not found:** `GetById`, `Update` and `Remove` throw an `ArgumentException` with the message "Item not found". `Add` and `Update` throw `ArgumentNullException` for `null`.
- **`Find`:** loops over the stored items and returns the ones the predicate accepts.
- **`Sort`:** sorts a copy and returns it, so the stored order is never modified.
- **Reuse:** `DataStore` knows nothing about jobs or companies, so `Find` and `Sort` work for every entity.

Each service inherits from `DataStore<T>` and overrides the methods where a business rule applies.

---

## 7. Delegates and events

| Type | Used for | Where |
|------|----------|-------|
| `Predicate<T>` | A yes/no test on one item | `Find`, the job search conditions |
| `Comparison<T>` | Deciding which of two items comes first | `Sort`, the job sorts |
| `EventHandler<TEventArgs>` | Notifying others that something happened | The three events below |
| `Action<string>` | Doing something with a value, with no result | `EventRaiser.ReportFailure` |

| Event | Raised by | When | Data |
|-------|-----------|------|------|
| `CompanyApproved` | `CompanyService.ChangeStatus` | After a company has been approved | The company |
| `ApplicationSubmitted` | `ApplicationService.Apply` | After an application is saved | The application, the job and the candidate |
| `ApplicationStatusChanged` | `ApplicationService.ChangeStatus` | After an application's status changed | The application, the old status and the new status |

- Events are raised only after a successful operation, so a refused action never sends a notification.
- `Notifier` subscribes with named method handlers and one lambda handler (a counter of submitted applications). `Subscribe` and `Unsubscribe` switch all handlers on and off together, and the "Toggle notifications" menu option uses them.
- **Subscriber isolation (C1):** each event keeps its own list of subscribers. `EventRaiser.Raise` uses `GetInvocationList()` and calls each subscriber inside its own `try/catch`. A failing subscriber is reported on one line, the others still run, and the operation finishes normally.

---

## 8. Business rules

### Companies (E3, E4)
- New companies start as `Pending`.
- Allowed changes: Pending to Approved or Rejected, and Approved to Pending or Rejected. Rejected is final.
- Every real change is pushed on the undo stack before it is applied. Undo restores the previous status directly, and it does not raise the approval event.
- **Only Approved companies can post jobs.** This is enforced in `JobService`, not only in the menu.

### Jobs
- A recruiter can add, update, close and remove jobs of their own company only. The check is in `JobService`, so it holds for every menu.
- A job cannot be moved to another company.
- **A job that has applications cannot be removed** (it can be closed instead). `ApplicationService.RemoveJob` enforces this.
- Approval happens at the company level. A job of an Approved company is live immediately, and jobs have no approval step.

### Applications (H1, M4, E6)
`ApplicationService.Apply` validates, in this order:
1. The candidate exists.
2. The job exists.
3. The job is open (`JobClosedException`).
4. The job's company is Approved (`CompanyNotApprovedException`).
5. The candidate has not already applied to the job (`HashSet.Add`, `DuplicateApplicationException`).

If any check fails, nothing is saved. On success the application is created with `AppliedAt = DateTime.Now` and `Status = Submitted`.

Application status changes can be