using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTracker.Data;
using ApplicationTracker.Models;

namespace ApplicationTracker.Services
{
    public class ApplicationService:DataStore<Application>
    {
        private readonly JobService jobService;
        private readonly CandidateService candidateService;
        private readonly CompanyService companyService;

        // H1: remembers every (job, candidate) pair that already applied
        private readonly HashSet<(int JobId, int CandidateId)> appliedPairs =
            new HashSet<(int JobId, int CandidateId)>();


        public ApplicationService( JobService jobService, CandidateService candidateService, CompanyService companyService)
        {
            this.jobService = jobService;
            this.candidateService = candidateService;
            this.companyService = companyService;
            
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
                throw new InvalidOperationException(
                    "This job's company is not Approved.");

            // 5. H1: no applying twice
            if (appliedPairs.Contains((jobId, candidateId)))
                throw new InvalidOperationException(
                    "This candidate already applied to this job.");

            // 6. Save, then remember the pair
            Application application = new Application
            {
                JobId = jobId,
                CandidateId = candidateId,
                AppliedDate = DateTime.Now,
                Status = ApplicationStatus.Submitted
            };

            base.Add(application);
            appliedPairs.Add((jobId, candidateId));   // only after the save worked
        }
    }
}
