using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace cshite.Model
{
    /// <summary>
    /// Our programs representation of the bank itself. Every operation will be persisted to the database*.
    /// </summary>
    /// <remarks>Data is stored using a series of JSON files. They're human readable, easily modified by hand, corruptable and have no ACID properties. Outside of a uni assignment this would be silly, but I'm just following the spec here.</remarks>
    public class Bank
    {
        public const int MinAccountID = 1000_0000;
        public const int MaxAccountID = 9999_9999;
        public const decimal MaxAccountBalance = 103_500_000_000; // Bill Gate's net worth, an upper limit on how much money we can process (handy way to avoid overflow!)

        readonly string directory;

        public Bank(string directory)
        {
            this.directory = directory;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Creates a new account
        /// </summary>
        public Account CreateAccount(string firstname, string lastname, string address, string email, long phone)
        {
            var account = new Account
            {
                ID = GetNextId(),
                FirstName = firstname,
                LastName = lastname,
                Address = address,
                Email = email,
                Phone = phone,
                Balance = 0
            };

            Save(account);
            return account;
        }

        /// <summary>
        /// Gets the next available ID for a user account
        /// </summary>
        int GetNextId()
            => Directory.GetFiles(directory, "*.txt")
                .Select(Path.GetFileNameWithoutExtension)
                .Select(name => int.TryParse(name, out var id) ? id : MinAccountID)
                .Append(MinAccountID) // If this is the first account to be created, the directory will be empty. This provides a default value
                .Max() + 1;

        /// <summary>
        /// Delete a user account
        /// </summary>
        public void Delete(Account account)
            => File.Delete(GetFilePath(account));

        /// <summary>
        /// Save all changes for this account. Create/overwrite as needed.
        /// </summary>
        void Save(Account account)
        {
            var serialiser = new DataContractJsonSerializer(typeof(Account));
            using (var stream = File.OpenWrite(GetFilePath(account)))
            {
                serialiser.WriteObject(stream, account);
            }
        }

        /// <summary>
        /// Load account with the given ID
        /// </summary>
        /// <returns>The account if found, otherwise null</returns>
        public Account Load(int id)
        {
            var file = GetFilePath(id);
            if (!File.Exists(file))
                return null;

            var serialiser = new DataContractJsonSerializer(typeof(Account));
            using (var stream = File.OpenRead(file))
            {
                return (Account)serialiser.ReadObject(stream);
            }
        }

        /// <summary>
        /// Perform a transaction on the provided account.
        /// 
        /// To remove money, provide a negative amount.
        /// </summary>
        public void Transact(Account account, decimal amount)
        {
            account.Balance += amount;
            account.Transactions.Add(amount);
            Save(account);
        }

        string GetFilePath(Account account)
            => GetFilePath(account.ID);

        string GetFilePath(int id)
            => Path.Combine(directory, $"{id}.txt");
    }
}
