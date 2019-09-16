using System;
using System.Collections.Generic;
using System.Linq;

namespace cshite.Model
{
    /// <summary>
    /// A plain ol data object for representing a user's account.
    /// 
    /// By limiting functionality available on the account directly, we can force all operations to go through the bank.
    /// </summary>
    public class Account
    {
        public int ID { get; internal set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
        public string Address { get; internal set; }
        public string Email { get; internal set; }
        public long Phone { get; internal set; } // Usually phone numbers would be stored as strings to not lose any leading zeros, but the spec demands a number
        public decimal Balance { get; internal set; }
        public List<decimal> Transactions { get; internal set; } = new List<decimal>();

        public override string ToString()
            => ToString(true);

        public string ToString(bool shortString)
        {
            if (shortString)
            {
                return $"{ID} ({FirstName} {LastName})";
            }

            return $@"Account no: {ID}
Balance: ${Balance.ToString("0.00")}
First Name: {FirstName}
Last Name: {LastName}
Address: {Address}
Phone: {Phone.ToString("00-0000-0000")}
Email: {Email}";
        }

        public string Serialise()
            => string.Join(Environment.NewLine, ID, FirstName, LastName, Address, Email, Phone, Balance, string.Join(",", Transactions));

        public static Account Deserialise(string serialised)
        {
            var lines = serialised.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return new Account
            {
                ID = int.Parse(lines[0]),
                FirstName = lines[1],
                LastName = lines[2],
                Address = lines[3],
                Email = lines[4],
                Phone = long.Parse(lines[5]),
                Balance = decimal.Parse(lines[6]),
                Transactions = lines.Length >= 8 ? lines[7].Split(',').Select(decimal.Parse).ToList() : new List<decimal>()
            };
        }
    }
}
