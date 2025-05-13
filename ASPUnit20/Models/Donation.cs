using System;

namespace ASPUnit20.Models
{
    public class Donation
    {
        public string Name { get; set; }
        public string Party { get; set; }
        public string Donor { get; set; }
        public float Amount { get; set; }

        public Donation(string name, string party, string donor, float amount)
        {
            Name = name;
            Party = party;
            Donor = donor;
            Amount = amount;
        }
    }
}