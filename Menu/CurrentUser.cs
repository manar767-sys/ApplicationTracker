using ApplicationTracker.Models;

namespace ApplicationTracker.Menu
{
    public class CurrentUser
    {
        //read only - one value per session
        public UserRole Role { get; }
        public int? UserId { get; }        // the recruiter or candidate Id, null for the admin
        public string Name { get; }        // shown in the menu header

        public CurrentUser(UserRole role, int? userId, string name)
        {
            Role = role;
            UserId = userId;
            Name = name;
        }

        public bool IsAdmin => Role == UserRole.Admin;
        public bool IsRecruiter => Role == UserRole.Recruiter;
        public bool IsCandidate => Role == UserRole.Candidate;
    }
}