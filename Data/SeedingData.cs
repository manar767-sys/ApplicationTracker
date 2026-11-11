using System;
using System.Collections.Generic;
using ApplicationTracker.Models;

namespace ApplicationTracker.Data
{
    public class SeedingData
    {
        public List<Company> Companies { get; } = new List<Company>
        {
            new Company { Id = 1, Name = "TechNova",   Industry = "Software",   City = "San Francisco", Status = CompanyStatus.Approved },
            new Company { Id = 2, Name = "DataBridge", Industry = "Analytics",  City = "Austin",        Status = CompanyStatus.Approved },
            new Company { Id = 3, Name = "BuildCore",  Industry = "Consulting", City = "Chicago",       Status = CompanyStatus.Pending }
        };

        public List<Recruiter> Recruiters { get; } = new List<Recruiter>
        {
            new Recruiter { Id = 1, Name = "Nadia Farouk", Email = "nadia.farouk@technova.com",  CompanyId = 1 },
            new Recruiter { Id = 2, Name = "Karim Salem",  Email = "karim.salem@databridge.com", CompanyId = 2 },
            new Recruiter { Id = 3, Name = "Hana Mostafa", Email = "hana.mostafa@buildcore.com", CompanyId = 3 }
        };

        public List<Job> Jobs { get; } = new List<Job>
        {
            new Job { Id = 1, Title = "Software Engineer", Description = "Develop and maintain software.",         Salary = 85000, Location = "San Francisco", IsOpen = true,  PostedDate = new DateTime(2026, 10, 1),  CompanyId = 1, RecruiterId = 1 },
            new Job { Id = 2, Title = "Data Analyst",      Description = "Analyze and interpret complex data.",     Salary = 68000, Location = "San Francisco", IsOpen = true,  PostedDate = new DateTime(2026, 10, 5),  CompanyId = 1, RecruiterId = 1 },
            new Job { Id = 3, Title = "Project Manager",   Description = "Lead and manage project teams.",          Salary = 80000, Location = "Austin",        IsOpen = true,  PostedDate = new DateTime(2026, 10, 8),  CompanyId = 2, RecruiterId = 2 },
            new Job { Id = 4, Title = "UX Designer",       Description = "Design user interfaces and experiences.", Salary = 75000, Location = "San Francisco", IsOpen = true,  PostedDate = new DateTime(2026, 10, 12), CompanyId = 1, RecruiterId = 1 },
            new Job { Id = 5, Title = "Backend Developer", Description = "Build and maintain server-side APIs.",    Salary = 90000, Location = "Austin",        IsOpen = true,  PostedDate = new DateTime(2026, 10, 15), CompanyId = 2, RecruiterId = 2 },
            new Job { Id = 6, Title = "QA Engineer",       Description = "Test software and report defects.",       Salary = 68000, Location = "Austin",        IsOpen = false, PostedDate = new DateTime(2026, 9, 20),  CompanyId = 2, RecruiterId = 2 }
        };

        public List<Candidate> Candidates { get; } = new List<Candidate>
        {
            new Candidate { Id = 1, FullName = "Sara Ahmed",   Email = "sara.ahmed@mail.com" },
            new Candidate { Id = 2, FullName = "Omar Hassan",  Email = "omar.hassan@mail.com" },
            new Candidate { Id = 3, FullName = "Laila Nabil",  Email = "laila.nabil@mail.com" },
            new Candidate { Id = 4, FullName = "Youssef Adel", Email = "youssef.adel@mail.com" },
            new Candidate { Id = 5, FullName = "Mona Samir",   Email = "mona.samir@mail.com" }
        };
    }
}