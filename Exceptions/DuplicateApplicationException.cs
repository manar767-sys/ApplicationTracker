using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTracker.Exceptions
{
    public class DuplicateApplicationException : InvalidOperationException
    {
        public int JobId { get; }
        public int CandidateId { get; }

        public DuplicateApplicationException(int jobId, int candidateId)
            : base($"Candidate {candidateId} has already applied to job {jobId}.")
        {
            JobId = jobId;
            CandidateId = candidateId;
        }
    }
}
