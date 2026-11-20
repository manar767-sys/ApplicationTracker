using System.ComponentModel.DataAnnotations;

namespace ApplicationTracker.Models
{
    public class Recruiter: IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; } // Navigation property
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
               
            
        }


    }

