using cshite.Model;
using cshite.UI;
using System;
using System.IO;
using System.Linq;

namespace cshite
{
    class Program
    {
        const string Title = "Cyber Bank Systems";

        static void Main(string[] args)
        {
            Login();

            WelcomeScreen();
        }

        static void Login()
        {
            while (true)
            {
                var c = new ConsoleScreen(header: Art.AsHeader(Title, Art.BitCoin));
                c.AddText("Login to start");
                c.AddBlankLines();

                var username = c.AddInput("User name: ", Validate.AsString());
                var password = c.AddPassword("Password: ");

                c.Show();

                if (CheckLogin(username.Response, password.Response))
                {
                    return;
                }

                ConsoleScreen.ShowError("Login Error", "Username-password combination was not valid.");
            }
        }

        static bool CheckLogin(string name, string pw)
            => File.ReadLines("login.txt").Contains(name + ", " + pw);

        static void WelcomeScreen()
        {
            var bank = new Bank("accounts");
            while (true)
            {
                var console = new ConsoleScreen(header: Art.AsHeader(Title, Art.BitCoin));
                console.AddText(@"1. Create a new account
2. Search for an account
3. Deposit
4. Withdraw
5. A/C statement
6. Delete Account
7. Exit
 ");
                var choice = console.AddInput("Enter your choice (1-7): ", Validate.NumberBetween(1, 7));
                console.Show();

                switch (choice.Response)
                {
                    case 1: CreateNewAccount(bank); break;
                    case 2: SearchForAccount(bank); break;
                    case 3: Deposit(bank); break;
                    case 4: Withdraw(bank); break;
                    case 5: AccountStatement(bank); break;
                    case 6: DeleteAccount(bank); break;
                    default: return;
                }
            }
        }

        static void CreateNewAccount(Bank bank)
        {
            var screen = new ConsoleScreen(header: Art.AsHeader("Create a new account", Art.Hello));
            var firstName = screen.AddInput("First Name: ", Validate.Name());
            var lastName = screen.AddInput("Last Name: ", Validate.Name());
            var address = screen.AddInput("Address: ", Validate.AsString(3));
            var phone = screen.AddInput("Phone: ", Validate.NumberBetween(0, 99_9999_9999));
            var email = screen.AddInput("Email: ", Validate.Email());

            screen.Show();

            if (ConsoleScreen.ShowConfirmation("Confirm new account", "Are you sure you would like to create this account?"))
            {
                var account = bank.CreateAccount(firstName.Response, lastName.Response, address.Response, email.Response, phone.Response);
                ConsoleScreen.ShowMessage("Account Created", $"Created new account with ID {account.ID}.");
            }
         }

        static void SearchForAccount(Bank bank)
        {
            var screen = new ConsoleScreen("Search an account");
            screen.AddText("Enter account details to search", TextJustification.Center);

            var account = screen.AddInput("Account number:", Validate.AsAccount(bank));
            if (screen.Show())
            {
                ShowSuccess(account.Response);
            }
        }

        static void Deposit(Bank bank)
        {
            var screen = new ConsoleScreen("Deposit");
            screen.AddText("Enter the details", TextJustification.Center);

            var account = screen.AddInput("Account Number: ", Validate.AsAccount(bank));
            var deposit = screen.AddInput("Amount: $", Validate.Money(() => decimal.MaxValue - account.Response.Balance));

            if (screen.Show())
            {
                bank.Transact(account.Response, deposit.Response);
                ShowSuccess(account.Response);
            }
        }

        static void Withdraw(Bank bank)
        {
            while (true)
            {
                var screen = new ConsoleScreen("Withdraw");
                screen.AddText("Enter the details", TextJustification.Center);
                var account = screen.AddInput("Account Number: ", Validate.AsAccount(bank));
                var amount = screen.AddInput("Amount: $", Validate.Money());

                if (!screen.Show())
                {
                    return;
                }
                else if (amount.Response <= account.Response.Balance)
                {
                    bank.Transact(account.Response, -amount.Response);
                    ShowSuccess(account.Response);

                    return;
                }
                else if (!ConsoleScreen.ShowConfirmation("You're too broke", "The selected account's balance is too low.", "Would you like to try again (y/n)? ", ConsoleColor.Black, ConsoleColor.Red))
                {
                    return;
                }
            }
        }

        static void AccountStatement(Bank bank)
        {
            var screen = new ConsoleScreen(header: Art.AsHeader("Statement"));
            var account = screen.AddInput("Account number: ", Validate.AsAccount(bank));
            if (screen.Show())
            {
                screen = new ConsoleScreen(header: Art.AsHeader($"Statement of {account.Response.ID}"));
                screen.AddText($"Account Balance: ${account.Response.Balance.ToString("0.00")}");
                screen.AddArt((true, " \r\n%\r\n "));
                foreach (var transaction in account.Response.Transactions)
                {
                    screen.AddText(transaction.ToString("$0.00"), forgroundColor: transaction < 0 ? ConsoleColor.Red : ConsoleColor.White);
                }

                screen.AddArt((true, " \r\n \r\n -"));

                var shouldEmail = screen.AddInput($"Email to {account.Response.Email} (y/n)?", Validate.Bool(), ConsoleColor.Green, ConsoleColor.Black);
                if (screen.Show() && shouldEmail.Response)
                {
                }
            }
        }

        static void DeleteAccount(Bank bank)
        {
            var screen = new ConsoleScreen(ConsoleColor.Yellow, ConsoleColor.Red, Art.AsHeader("Delete account", Art.Goodbye));
            screen.AddText("Enter the details", TextJustification.Center);

            var account = screen.AddInput("Account number: ", Validate.AsAccount(bank));
            if (screen.Show())
            {
                if (ConsoleScreen.ShowConfirmation("DELETE", "About to delete this account: \r\n" + account.Response.ToString(false), forground: ConsoleColor.Red))
                {
                    bank.Delete(account.Response);
                    ConsoleScreen.ShowMessage("Account deleted", "Account was successfully deleted.");
                }
            }
        }

        static void ShowSuccess(Account account)
            => ConsoleScreen.ShowMessage("Success", account.ToString(false));
    }
}
