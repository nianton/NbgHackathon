

using System;

namespace NbgHackathon.Models
{
    public class PassportInformation
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Nationality { get; set; }
        public string PassportNumber { get; set; }
        // TODO: More properties
    }
}