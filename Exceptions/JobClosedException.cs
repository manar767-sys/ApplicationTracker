using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTracker.Exceptions
{
    public class JobClosedException : InvalidOperationException
    {
        public int JobId { get; }

        public JobClosedException(int jobId)
            : base($"Job {jobId} is closed and cannot receive applications.")
        {
            JobId = jobId;
        }
    }
}
