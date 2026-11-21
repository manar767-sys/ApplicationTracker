using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTracker.Exceptions
{
    public class CompanyNotApprovedException : InvalidOperationException
    {
        public int CompanyId { get; }

        public CompanyNotApprovedException(int companyId)
            : base($"Company {companyId} is not Approved, so its jobs cannot be posted, changed or applied to.")
        {
            CompanyId = companyId;
        }
    }
}
