using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text;
using ApplicationTracker.Data;

namespace ApplicationTracker.Models
{
    public class Candidate: IEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        // 1. Private backing field to hold the structured MailAddress object
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required]
        public string Email { get; set; }
    }
        
    }

