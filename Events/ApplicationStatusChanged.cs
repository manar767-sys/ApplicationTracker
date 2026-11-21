using System;
using ApplicationTracker.Models;

namespace ApplicationTracker.Events
{
    public class ApplicationStatusChangedEventArgs : EventArgs
    {
        public Application Application { get; }
        public ApplicationStatus OldStatus { get; }
        public ApplicationStatus NewStatus { get; }

        public ApplicationStatusChangedEventArgs(Application application,
            ApplicationStatus oldStatus, ApplicationStatus newStatus)
        {
            Application = application;
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }
}