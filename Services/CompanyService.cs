
using ApplicationTracker.Data;
using ApplicationTracker.Models;

namespace ApplicationTracker.Services
{
    public class CompanyService : DataStore<Company>
    {
        //not to be reassigned, so readonly
        private readonly Stack<(int CompanyId, CompanyStatus OldStatus)> statusChangeStack = new Stack<(int, CompanyStatus)>();
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

            bool allowed =
                (company.Status == CompanyStatus.Pending && newStatus != CompanyStatus.Pending)
                || (company.Status == CompanyStatus.Approved && newStatus == CompanyStatus.Pending);

            if (!allowed)
                throw new InvalidOperationException(
                    $"A company cannot change from {company.Status} to {newStatus}.");

            // H2: push
            statusChangeStack.Push((companyId, company.Status));

            company.Status = newStatus;
        }
        //H2: pop
        public bool UndoLastChange()
        {
            if (statusChangeStack.Count == 0)
                return false;                               // nothing to undo, not an error

            var lastChange = statusChangeStack.Pop();       // most recent change first

            Company company = GetById(lastChange.CompanyId);
            company.Status = lastChange.OldStatus;          

            return true;
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
