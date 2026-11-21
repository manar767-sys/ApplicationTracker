using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTracker.Data;
using ApplicationTracker.Models;

namespace ApplicationTracker.Services
{
    public class JobService : DataStore<Job>
    {

        private CompanyService companyService;
        private Dictionary<int, Job> jobsById = new Dictionary<int, Job>();
        public JobService(CompanyService companyService)
        {
            this.companyService = companyService;
        }

        public override void Add(Job job)
        {

            if (job == null)
                throw new ArgumentNullException(nameof(job), "Item cannot be null");

            if (!companyService.IsApproved(job.CompanyId))
                throw new InvalidOperationException(
                    "Jobs can only be posted for an Approved company.");

            base.Add(job);                  // assigns the Id and adds to the list
            jobsById.Add(job.Id, job);      // key is the job's own Id

        }
        public override void Update(Job job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job), "Item cannot be null");
            if (!companyService.IsApproved(job.CompanyId))
                throw new InvalidOperationException(
                    "Jobs can only be posted for an Approved company.");

            base.Update(job);               // throws "Item not found" if the Id is unknown
            jobsById[job.Id] = job;         // point the Dictionary at the new object
        }

        public override void Remove(Job job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job), "Item cannot be null");

            base.Remove(job);               // if this throws, the Dictionary is untouched
            jobsById.Remove(job.Id);
        }


        // E5: find a job by Id using the Dictionary, no loop needed here
        //The Dictionary<TKey, TValue>.TryGetValue method in C# safely checks if a key exists and retrieves its value in a single operation. It returns true if the key is found and false if it is not, avoiding a KeyNotFoundException without requiring two separate lookups
        public Job FindById(int id)
        {
            if (jobsById.TryGetValue(id, out Job job))
                return job;

            throw new ArgumentException("Item not found");
        }
        public override List<Job> GetAll()
        {
            return jobsById.Values.ToList();
        }
        // M5: the job must belong to the acting recruiter's company
        public void EnsureOwnedBy(Job job, Recruiter actingRecruiter)
        {
            if (actingRecruiter == null)
                throw new ArgumentNullException(nameof(actingRecruiter), "Recruiter cannot be null");

            if (job.CompanyId != actingRecruiter.CompanyId)
                throw new InvalidOperationException("You can only manage jobs of your own company.");
        }

        public void Update(Job job, Recruiter actingRecruiter)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job), "Item cannot be null");

            Job existing = FindById(job.Id);                    // throws "Item not found" if the Id is unknown
            EnsureOwnedBy(existing, actingRecruiter);

            if (job.CompanyId != existing.CompanyId)
                throw new InvalidOperationException("A job cannot be moved to another company.");

            Update(job);                                        // the override: list and Dictionary stay in sync
        }

        public void Close(int jobId, Recruiter actingRecruiter)
        {
            Job job = FindById(jobId);
            EnsureOwnedBy(job, actingRecruiter);

            if (!job.IsOpen)
                throw new InvalidOperationException("This job is already closed.");

            job.IsOpen = false;                                 // FindById returns the stored object
        }
        //overload Remove to ensure the acting recruiter owns the job before allowing removal by the actual Remove method above
        public void Remove(Job job, Recruiter actingRecruiter)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job), "Item cannot be null");

            Job existing = FindById(job.Id);
            EnsureOwnedBy(existing, actingRecruiter);

            Remove(existing);                                   // the override: list and Dictionary stay in sync
        }
    }
}