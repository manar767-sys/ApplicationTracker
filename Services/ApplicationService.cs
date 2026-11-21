using System;
using System.Collections.Generic;
using ApplicationTracker.Data;
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
            candidateService.GetById(candidateId);

            // 2. Job exists (throws "Item not found" if not)
            Job job = jobService.FindById(jobId);

            // 3. Job is open
            if (!job.IsOpen)
                throw new InvalidOperationException("This job is closed.");

            // 4. Company is Approved
            if (!companyService.IsApproved(job.CompanyId))
                throw new InvalidOperationException("This job's company is not Approved.");

            // 5. M4: HashSet.Add returns false when the pair is already there
            if (!appliedPairs.Add((jobId, candidateId)))
                throw new InvalidOperationException("This candidate already applied to this job.");

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
        }

        // Keep the HashSet in sync when an application is removed
        public override void Remove(Application application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application), "Item cannot be null");

            base.Remove(application);                                          // throws first if not found
            appliedPairs.Remove((application.JobId, application.CandidateId)); // same order as in Apply
        }
    }
}