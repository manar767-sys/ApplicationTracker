using System;
using System.Collections.Generic;
using ApplicationTracker.Data;
using ApplicationTracker.Models;

namespace ApplicationTracker.Services
{
    public class CandidateService : DataStore<Candidate>
    {
        // Case-insensitive, so "Sara@Mail.com" and "sara@mail.com" count as the same email
        private readonly HashSet<string> emails =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Your method, fixed: no useless Compare line, and it says what it checks
        public bool IsEmailUnique(string email)
        {
            return !emails.Contains(email);
        }

        public override void Add(Candidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate), "Item cannot be null");

            if (string.IsNullOrWhiteSpace(candidate.Email))
                throw new InvalidOperationException("A candidate must have an email.");

            // HashSet.Add returns false when the email is already there: check and register in one step
            if (!emails.Add(candidate.Email))
                throw new InvalidOperationException(
                    $"A candidate with the email {candidate.Email} already exists.");

            base.Add(candidate);
        }

        public override void Update(Candidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate), "Item cannot be null");

            if (string.IsNullOrWhiteSpace(candidate.Email))
                throw new InvalidOperationException("A candidate must have an email.");

            Candidate existing = GetById(candidate.Id);   // throws "Item not found" if the Id is unknown
            string oldEmail = existing.Email;

            emails.Remove(oldEmail);                       // free the old email first, so keeping it isn't a duplicate
            if (!emails.Add(candidate.Email))
            {
                emails.Add(oldEmail);                      // the new email is taken: put the old one back
                throw new InvalidOperationException(
                    $"A candidate with the email {candidate.Email} already exists.");
            }

            base.Update(candidate);
        }

        public override void Remove(Candidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate), "Item cannot be null");

            base.Remove(candidate);                        // throws first if not found
            emails.Remove(candidate.Email);
        }
    }
}