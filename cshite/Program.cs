using cshite.Model;
using cshite.UI;
using System;
using System.IO;
using System.Linq;

namespace cshite
{
    /// <summary>
    /// This is where we interact with the user. It manages creation of screens and tells the bank what the user would like to do.
    /// 
    /// General design guidelines
    /// - Architecture
    ///   I have seperated out three main pieces of functionality
    ///     + Display & Validation: Handled by the classes within the UI folder. These classes should be loosly coupled to the banking application and should be reusable for any UI-heavy console app.
    ///     + Banking operations: Handled by the classes within the Model folder. These classes need not know they're a console application and should be usable from a Winforms or Webserver app.
    ///     + Implementation specific: Handled by the Program class. This uses the APIs provided to actually implement the desired functionality. This is happy to know it's banking console app.
    ///
    /// - State
    ///   State is a necessary evil. Each 'unit' of state a class exposes exponentially increases the complexity of the class (since you have increased the number of potential data configurations).
    ///   I opt for immutability where possible and concede where it's more practical or efficient to utilize mutability.
    ///   + This is why const/readonly is prevalent throughout this program (if something doesn't need to change, don't let it!). 
    ///   + Methods that don't need state are generally made static, this makes it obvious to know which methods DO mutate state. Sometimes I pass field values as args into a method where I could make it non-static and access the field directly, personally I prefer this approach since it makes it most explicit what data an operation requires.
    ///   
    /// - Comments
    ///   Good code should be easily reabable and self documenting. A short, well-named method is usually better than a large multi-operation method with many comments.
    ///   + All public methods should include a comment on their intended operation, code within that method however should be readable without heavy commenting.
    ///   + It is reasonable to assume the reader of the code is familiar with the language's syntax and standard libraries, so they should not require additional comments.
    /// 
    /// - Struct vs Class
    ///   I have used a class where I do not wish state to be mutable externally once passed into a given function. Otherwise I have used a class.
    ///   + I understand in some cases by wrapping them in arrays I lose this benefit, but deemed the cost of a mem-copy overkill.
    ///
    /// - Access modifiers
    ///   I've taken a pretty straightforward "provide mimimum access required for this class's API to be useful"
    ///   + Public: These are the 'proper' way to access api methods. Public methods are callable anywhere, so I tried to minimise oppertunity for devs to shoot themselves in the foot by providing xml comments as documentation on how to use APIs.
    ///   + Internal: For module-specific functionality. Lets just pretend 'internal' is per namespace because it feels overkill to actually move each folder into its own seperate project.
    ///   + Protected: Certain helpers and subfunctions need only be accessed by subclasses, protected is perfect for this
    ///   + Private: Default access level (you won't find any private keywords here). If it has no reason to be accessed by other classes I leave it as default
    ///   
    /// </summary>
    static class Program
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
                var c = new ConsoleScreen(Art.AsHeader(Title, Art.BitCoin));
                c.AddText("Login to start");
                c.AddBlankLines();

                var username = c.AddInput("User name: ", Validate.AsString());
                var password = c.AddPassword("Password: ");

                if (c.Show() && IsValidLoginAccount(username.Response, password.Response))
                {
                    return;
                }

                ConsoleScreen.ShowError("Login Error", "Username-password combination was not valid.");
            }
        }

        static bool IsValidLoginAccount(string name, string pw)
            => File.ReadLines("login.txt").Contains(name + ", " + pw); // No salts & hashes here

        static void WelcomeScreen()
        {
            var bank = new Bank("accounts");
            while (true)
            {
                var console = new ConsoleScreen(Art.AsHeader(Title, Art.BitCoin));
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
            var screen = new ConsoleScreen(Art.AsHeader("Create a new account", Art.Hello));
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
            var deposit = screen.AddInput("Amount: $", Validate.Money());

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
            var screen = new ConsoleScreen(Art.AsHeader("Statement"));
            var account = screen.AddInput("Account number: ", Validate.AsAccount(bank));
            if (screen.Show())
            {
                screen = new ConsoleScreen(Art.AsHeader($"Statement of {account.Response.ID}"));
                screen.AddText($"Account Balance: ${account.Response.Balance.ToString("0.00")}");
                screen.AddSeperator(" \r\n%\r\n ");
                foreach (var transaction in account.Response.Transactions)
                {
                    screen.AddText(transaction.ToString("$0.00"), forgroundColor: transaction < 0 ? ConsoleColor.Red : ConsoleColor.White);
                }

                screen.AddSeperator(" \r\n \r\n -");

                var shouldEmail = screen.AddInput($"Email to {account.Response.Email} (y/n)?", Validate.Bool(), ConsoleColor.Green, ConsoleColor.Black);
                if (screen.Show() && shouldEmail.Response)
                {
                }
            }
        }

        static void DeleteAccount(Bank bank)
        {
            var screen = new ConsoleScreen(Art.AsHeader("Delete account", Art.Goodbye), ConsoleColor.Yellow, ConsoleColor.Red);
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
