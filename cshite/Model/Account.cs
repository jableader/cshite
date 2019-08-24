using System.Collections.Generic;
using System.Runtime.Serialization;

namespace cshite.Model
{
    /// <summary>
    /// A plain ol data object for representing a user's account.
    /// 
    /// By limiting functionality available on the account directly, we can force all operations to go through the bank.
    /// </summary>
    [DataContract]
    public class Account
    {
        [DataMember]
        public int ID { get; internal set; }

        [DataMember]
        public string FirstName { get; internal set; }

        [DataMember]
        public string LastName { get; internal set; }

        [DataMember]
        public string Address { get; internal set; }

        [DataMember]
        public string Email { get; internal set; }

        [DataMember]
        public long Phone { get; internal set; } // Usually phone numbers would be stored as strings to not lose any leading zeros, but the spec demands a number

        [DataMember]
        public decimal Balance { get; internal set; }

        [DataMember]
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
    }
}
