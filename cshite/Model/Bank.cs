using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace cshite.Model
{
    public class Bank
    {
        public const int MinAccountID = 1000_0000;
        public const int MaxAccountID = 9999_9999;

        readonly string directory;

        public Bank(string directory)
        {
            this.directory = directory;
        }

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

        int GetNextId()
        {
            var filenames = Directory.GetFiles(directory, "*.txt")
                .Select(Path.GetFileNameWithoutExtension);

            var max = MinAccountID;
            foreach (var name in filenames)
            {
                if (int.TryParse(name, out var id) && id > max)
                {
                    max = id;
                }
            }

            return max + 1;
        }

        public void Delete(Account account)
            => File.Delete(GetFilePath(account));

        public void Save(Account account)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serialiser = new DataContractJsonSerializer(typeof(Account));
            using (var stream = File.OpenWrite(GetFilePath(account)))
            {
                serialiser.WriteObject(stream, account);
            }
        }

        public Account Load(int id)
        {
            var path = GetFilePath(id);
            return File.Exists(path) ? Load(path) : null;
        }

        Account Load(string file)
        {
            var serialiser = new DataContractJsonSerializer(typeof(Account));
            using (var stream = File.OpenRead(file))
            {
                return (Account)serialiser.ReadObject(stream);
            }
        }

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
