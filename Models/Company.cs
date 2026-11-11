using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationTracker.Models
{
    public interface IEntity
    {
        public int Id { get; set; }
    }
    public enum CompanyStatus { Pending, Approved, Rejected }

    public class Company: IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Industry { get; set; }
        public string City { get; set; }
        public CompanyStatus Status { get; set; }

    }

   
}
