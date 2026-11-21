using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTracker.Models;

namespace ApplicationTracker.Events
{
    public class CompanyApprovedEventArgs : EventArgs
    {
        public Company Company { get; }
        public CompanyApprovedEventArgs(Company company)
        {
            Company = company;
        }
    }
    
    
}
