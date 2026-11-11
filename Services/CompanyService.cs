using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
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

        //E3
        public void ChangeStatus(int companyId, CompanyStatus newStatus)
        {
            Company company = GetById(companyId);   // throws "Item not found" if no company has this Id

            if (company.Status != CompanyStatus.Pending)
                throw new InvalidOperationException(
                    "Only a company that is Pending can change its status.");

            if (newStatus == CompanyStatus.Pending)
                throw new InvalidOperationException(
                    "The new status must be Approved or Rejected. Not Pending");

            company.Status = newStatus;
        }

        //E3
        public List<Company> ListByStatus(CompanyStatus status)
        {
            List<Company> result = new List<Company>();

            foreach (Company company in GetAll())
            {
                if (company.Status == status)
                {
                    result.Add(company);
                }
            }

            return result;   
        }


    }
}
