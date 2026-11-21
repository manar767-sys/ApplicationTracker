using System;
using ApplicationTracker.Models;

namespace ApplicationTracker.Events
{
    public class ApplicationSubmittedEventArgs : EventArgs
    {
        public Application Application { get; }
        public Job Job { get; }
        public Candidate Candidate { get; }

        public ApplicationSubmittedEventArgs(Application application, Job job, Candidate candidate)
        {
            Application = application;
            Job = job;
            Candidate = candidate;
        }
    }
}