using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTracker.Data;
using ApplicationTracker.Models;

namespace ApplicationTracker.Services
{
    public class CompanyService : DataStore<Company>
    {
        public CompanyService()
        {
        }

        public bool IsApproved(int companyId)
        {
            Company company = GetById(companyId);   // throws "Item not found" if the company doesn't exist
            return company.Status == CompanyStatus.Approved;
        }


    }
}
