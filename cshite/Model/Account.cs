using System.Collections.Generic;
using System.Runtime.Serialization;

namespace cshite.Model
{
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
        public long Phone { get; internal set; }

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
