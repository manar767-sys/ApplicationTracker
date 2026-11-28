
using ApplicationTracker.Data;
using ApplicationTracker.Events;
using ApplicationTracker.Exceptions;
using ApplicationTracker.Models;

namespace ApplicationTracker.Services
{
    public class ApplicationService : DataStore<Application>
    {
        private readonly JobService jobService;
        private readonly CandidateService candidateService;
        private readonly CompanyService companyService;

        // H1 / M4: every (job, candidate) pair that already applied
        private readonly HashSet<(int JobId, int CandidateId)> appliedPairs =
            new HashSet<(int JobId, int CandidateId)>();
        public event EventHandler<ApplicationSubmittedEventArgs> ApplicationSubmitted;
        public event EventHandler<ApplicationStatusChangedEventArgs> ApplicationStatusChanged;

        public ApplicationService(JobService jobService, CandidateService candidateService, CompanyService companyService)
        {
            this.jobService = jobService;
            this.candidateService = candidateService;
            this.companyService = companyService;
        }

        // Does this job have any application (or one from this candidate)?
        // Reads the real applications, so it can't go out of sync with the HashSet.
        public bool HasApplications(int jobId, int? candidateId = null)
        {
            foreach (Application application in GetAll())
            {
                if (application.JobId == jobId &&
                    (candidateId == null || application.CandidateId == candidateId))
                    return true;
            }
            return false;
        }

        public void Apply(int candidateId, int jobId)
        {
            // 1. Candidate exists (throws "Item not found" if not)
            Candidate candidate = candidateService.GetById(candidateId);

            // 2. Job exists (throws "Item not found" if not)
            Job job = jobService.FindById(jobId);

            // 3. Job is open
            if (!job.IsOpen)
                throw new JobClosedException(jobId);

            // 4. Company is Approved
            if (!companyService.IsApproved(job.CompanyId))
                throw new CompanyNotApprovedException(job.CompanyId);

            // 5. M4: HashSet.Add returns false when the pair is already there
            if (!appliedPairs.Add((jobId, candidateId)))
                throw new DuplicateApplicationException(jobId, candidateId);

            // 6. Create and save the application
            Application application = new Application
            {
                JobId = jobId,
                CandidateId = candidateId,
                AppliedAt = DateTime.Now,
                Status = ApplicationStatus.Submitted
            };

            try
            {
                base.Add(application);
            }
            catch
            {
                appliedPairs.Remove((jobId, candidateId));   // the save failed: don't leave the pair blocked
                throw;
            }

            EventRaiser.Raise(ApplicationSubmitted, this, new ApplicationSubmittedEventArgs(application, job, candidate));
        }

        // Keep the HashSet in sync when an application is removed
        public override void Remove(Application application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application), "Item cannot be null");

            base.Remove(application);                                          // throws first if not found
            appliedPairs.Remove((application.JobId, application.CandidateId)); // same order as in Apply
        }
        // E6: a job with applications cannot be removed
        public void RemoveJob(int jobId, Recruiter actingRecruiter)
        {
            Job job = jobService.FindById(jobId);               // throws "Item not found"
            jobService.EnsureOwnedBy(job, actingRecruiter);     // ownership first, so nothing is revealed about other companies' jobs

            if (HasApplications(jobId))
                throw new InvalidOperationException(
                    "This job has applications and cannot be removed. You can close it instead.");

            jobService.Remove(job, actingRecruiter);
        }
        // Only a recruiter of the job's company can review an application

        public void ChangeStatus(int applicationId, ApplicationStatus newStatus, Recruiter actingRecruiter)
        {
            Application application = GetById(applicationId);          // throws "Item not found"
            Job job = jobService.FindById(application.JobId);
            jobService.EnsureOwnedBy(job, actingRecruiter);            // M5: own company only

            ApplicationStatus oldStatus = application.Status;

            bool allowed =
                (oldStatus == ApplicationStatus.Submitted && newStatus == ApplicationStatus.UnderReview)
                || (oldStatus == ApplicationStatus.UnderReview &&
                    (newStatus == ApplicationStatus.Accepted || newStatus == ApplicationStatus.Rejected));

            if (!allowed)
                throw new InvalidOperationException(
                    $"An application cannot change from {oldStatus} to {newStatus}.");

            application.Status = newStatus;                            // GetById returns the stored object

            EventRaiser.Raise(ApplicationStatusChanged, this,
     new ApplicationStatusChangedEventArgs(application, oldStatus, newStatus));
        }
    }
}