using System;
using ApplicationTracker.Services;

namespace ApplicationTracker.Events
{
    public class Notifier
    {
        private readonly CompanyService companyService;
        private readonly ApplicationService applicationService;

        private int submittedCount;

        // A lambda handler. It lives in a variable, so the same object can be used for += and -=
        private readonly EventHandler<ApplicationSubmittedEventArgs> submittedCounter;

        public bool IsOn { get; private set; }

        public Notifier(CompanyService companyService, ApplicationService applicationService)
        {
            this.companyService = companyService;
            this.applicationService = applicationService;

            submittedCounter = (sender, e) =>
            {
                submittedCount++;
                Console.WriteLine($"[Notification] New application received for '{e.Job.Title}' (applications this session: {submittedCount}).");
            };
        }


        // ---------- named method handlers ----------
        public void OnCompanyApproved(object sender, CompanyApprovedEventArgs e)
        {
            Console.WriteLine($"[Notification] Company '{e.Company.Name}' has been approved.");
        }

        public void OnApplicationSubmitted(object sender, ApplicationSubmittedEventArgs e)
        {
            Console.WriteLine($"[Notification] {e.Candidate.FullName} applied to '{e.Job.Title}' (application {e.Application.Id}).");
        }

        public void OnApplicationStatusChanged(object sender, ApplicationStatusChangedEventArgs e)
        {
            Console.WriteLine($"[Notification] Application {e.Application.Id} changed from {e.OldStatus} to {e.NewStatus}.");
        }

        // ---------- subscribing and unsubscribing ----------
        public void Subscribe()
        {
            if (IsOn) return;                                   // never subscribe twice

            companyService.CompanyApproved += OnCompanyApproved;
            applicationService.ApplicationSubmitted += OnApplicationSubmitted;
            applicationService.ApplicationSubmitted += submittedCounter;
            applicationService.ApplicationStatusChanged += OnApplicationStatusChanged;
            IsOn = true;
        }

        public void Unsubscribe()
        {
            if (!IsOn) return;

            companyService.CompanyApproved -= OnCompanyApproved;
            applicationService.ApplicationSubmitted -= OnApplicationSubmitted;
            applicationService.ApplicationSubmitted -= submittedCounter;
            applicationService.ApplicationStatusChanged -= OnApplicationStatusChanged;
            IsOn = false;
        }
    }
}