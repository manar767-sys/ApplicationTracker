using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationTracker.Models
{
    public class Job:IEntity
    {
        private decimal _salary;
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Salary
        {
            get { return _salary; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Salary must be above 0.");
                _salary = value;
            }
        }

        public string Location { get; set; }

        public bool IsOpen { get; set; } = true;   // default true

        public DateTime PostedDate { get; set; }

        public int CompanyId { get; set; }

        public int RecruiterId { get; set; }
        public Company Company { get; set; }//navigation property
        public Recruiter Recruiter { get; set; }//navigation property

        public override string ToString()
        {
            return $"{Title}: {Location}:{IsOpen}:{CompanyId}:{RecruiterId}";
            ;
        }
    }
}
