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
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        // 1. Private backing field to hold the structured MailAddress object
        private MailAddress _email;
        [Required]
        public string Email
        {
            get => _email?.Address; // Returns the clean, standardized email string
            set
            {
                // Validate and parse the incoming string
                if (MailAddress.TryCreate(value, out MailAddress validAddress))
                {
                    _email = validAddress;
                }
                else
                {
                    throw new ArgumentException("The provided email address is not in a valid format.", nameof(value));
                }
            }
        }
    }
}
