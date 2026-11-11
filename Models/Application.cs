using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationTracker.Models
{
    public enum ApplicationStatus { Submitted,UnderReview,Interview, Accepted, Rejected }
    public class Application:IEntity
    {
        [Key]
        public int Id { get; set; }
        public int JobId { get; set; }
        public int CandidateId { get; set; }
        public DateTime AppliedDate { get; set; }
        public ApplicationStatus Status { get; set; }
        public Candidate Candidate { get; set; }
        public Job Job { get; set; } // Navigation property
    }
}
