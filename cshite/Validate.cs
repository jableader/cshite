﻿using cshite.Model;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace cshite.UI
{
    /// <summary>
    /// Validation methods for use with the ConsoleScreen's AddInput method
    /// </summary>
    public static class Validate
    {
        readonly static string[] recognisedDomains = new[] { "outlook.com", "gmail.com", "uts.edu.au", "student.uts.edu.au" };

        /// <summary>
        /// Validate the parameter as an email
        /// </summary>
        /// <returns></returns>
        public static Validator<string> Email()
        {
            var regex = new Regex(@"^[\w\+\.]+@[\w\.]+$");
            return response =>
            {
                // Look for letters, digits, underscores, '+' and '.' either side of an '@' symbol
                if (!regex.IsMatch(response))
                {
                    return Validated<string>.Error("Please enter a valid email in the form xyz@abc.com");
                }

                // Confirm with the user if the email isn't from a recognisable domain.
                var domain = response.Split('@').Last();
                if (!recognisedDomains.Contains(domain) && !ConsoleScreen.ShowConfirmation("Unrecognised email domain", $"That email address is not from a recognised domain. Is {response} correct?"))
                {
                    return Validated<string>.Error();
                }

                return Validated<string>.Success(response);
            };
        }

        /// <summary>
        /// Validate for a number within the provided range
        /// </summary>
        public static Validator<long> NumberBetween(long min, long max)
        {
            return response =>
                long.TryParse(response, out var result) && result >= min && result <= max ?
                    Validated<long>.Success(result) :
                    Validated<long>.Error($"Please input a number without spaces between {min} & {max}");
        }

        /// <summary>
        /// Validate as a name
        /// </summary>
        public static Validator<string> Name()
            => MatchingRegex(@"[\w\-]{2,}", "A name must have atleast two letters and contain only alphanumerics, underscores and hyphens");

        /// <summary>
        /// Validate any string within the length provided
        /// </summary>
        public static Validator<string> AsString(int minlength = 0, int maxlength = 1_000)
        {
            return response =>
                response.Length >= minlength && response.Length <= maxlength ?
                    Validated<string>.Success(response) :
                    Validated<string>.Error($"Please enter at least {minlength} characters and at most {maxlength}");
        }

        /// <summary>
        /// Use a regex to validate the user input
        /// </summary>
        /// <param name="pattern">The regex the whole response must match</param>
        /// <param name="requirements">A message to show the user if their response does not match</param>
        public static Validator<string> MatchingRegex(string pattern, string requirements)
        {
            var regex = new Regex($"^{pattern}$");

            return response =>
                regex.IsMatch(response) ? Validated<string>.Success(response) : Validated<string>.Error(requirements);
        }

        /// <summary>
        /// Search for a valid bank account (the user may cancel this operation)
        /// </summary>
        public static Validator<Account> AsAccount(Bank bank)
        {
            return response =>
            {
                var account = int.TryParse(response, out var id) ? bank.Load(id) : null;
                if (account != null)
                {
                    return Validated<Account>.Success(account);
                }

                return ConsoleScreen.ShowConfirmation("Account not found", "An account with that ID could not be found.", "Would you like to try again (y/n)? ") ?
                    Validated<Account>.Error() :
                    Validated<Account>.Cancel();
            };
        }

        /// <summary>
        /// For validating financial values
        /// </summary>
        public static Validator<decimal> Money()
        {
            return response =>
            {
                if (!decimal.TryParse(response, out var amount) || amount <= 0)
                {
                    return Validated<decimal>.Error("Please enter a valid, positive number.");
                }

                if (amount > Bank.MaxAccountBalance) 
                {
                    return Validated<decimal>.Error("Wow, that's a lot of money. Sorry, we can't process that transaction.");
                }

                if (amount % 0.01m != 0m)
                {
                    return Validated<decimal>.Error("You cannot add partial cents.");
                }

                return Validated<decimal>.Success(amount);
            };
        }

        /// <summary>
        /// For a simple yes/no question where 'y' is true
        /// </summary>
        public static Validator<bool> Bool()
        {
            return response => Regex.IsMatch(response, "^(y|n|yes|no)$", RegexOptions.IgnoreCase) ?
                Validated<bool>.Success(response.StartsWith("y", StringComparison.OrdinalIgnoreCase)) :
                Validated<bool>.Error("Please enter 'y' or 'n'.");
        }
    }
}
