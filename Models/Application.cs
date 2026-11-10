using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTracker.Models
{
    public enum ApplicationStatus { Submitted,UnderReview,Interview, Accepted, Rejected }
    public class Application:IEntity
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int CandidateId { get; set; }
        public DateTime AppliedDate { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
