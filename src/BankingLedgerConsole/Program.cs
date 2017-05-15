using System;
using System.Collections.Generic;
using System.Linq;

namespace BankingLedgerConsole
{
    public static class UserHelper // UserHelper acts as temporary storage for user account data. Storage only last duration of session.
    {
        public static List<User> AllUsers = new List<User>();

        public static List<User> GetAll()
        {
            return AllUsers;
        }

        public static void AddNew(User newUser)
        {
            AllUsers.Add(newUser);
        }

        public static User FindByName(string inputName)
        {
            return AllUsers.FirstOrDefault(u => u.Name == inputName);
        }

        public static bool UserNameAvailable(string inputUserName)
        {
            bool nameAvailable = true;

            foreach (User user in AllUsers)
            {
                if (user.Name == inputUserName)
                {
                    nameAvailable = false;
                }
            }

            return nameAvailable;
        }
    }

    public class User
    {
        public string Name { get; set; } // username rather than first or last
        public string Password { get; set; }
        public Account Account { get; set; }

        public User(string newName, string newPassword)
        {
            Name = newName;
            Password = newPassword;
            Account = new Account();
        }
    }

    public class Account
    {
        decimal Balance;
        List<Transaction> Transactions { get; set; }

        public Account()
        {
            Balance = 0;
            Transactions = new List<Transaction>();
        }

        public void Deposit(decimal amount, string description = "")
        {
            if (amount > 0)
            {
                Balance += amount;
                Transactions.Add(new Transaction(amount, "deposit", description));
            }
        }

        public void Withdraw(decimal amount, string description = "")
        {
            if (amount > 0 && amount <= Balance)
            {
                Balance -= amount;
                Transactions.Add(new Transaction(amount, "withdrawal", description));
            }
        }

        public decimal GetBalance()
        {
            return Balance;
        }

        public List<Transaction> GetTransactionHistory()
        {
            return Transactions.OrderByDescending(t => t.Date).ToList();
        }
    }

    public class Transaction
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // "deposit"/"withdrawal"

        public Transaction(decimal newAmount, string newDepositType, string newDescription = "")
        {
            Date = DateTime.Now;
            Amount = newAmount;
            Description = newDescription;
            Type = newDepositType;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            bool done = false; // Condition for main program loop
            bool isLoggedIn = false;

            string consoleInput = "";
            char cmdChar = ' '; // First character of input, used in switch to choose actions

            ConsoleColor oldFontColor; // Used for holding font color when switching colors for password masking

            string inputName = null;
            string inputPassword = null;
            User currentUser = null; // Holds user object when logged in

            Console.WriteLine("Welcome to CashSource, the alternate source for online banking! Please [L]ogin or [R]egister...");

            while (!done)
            {
                if (isLoggedIn)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Would you like to: View Account [B]alance, Make a [D]eposit, Make a [W]ithdrawal, View [T]ransaction History, or [L]ogout?");

                    cmdChar = ' '; // Reset command input
                    consoleInput = Console.ReadLine();

                    if (consoleInput != "")
                    {
                        cmdChar = consoleInput.ToUpper()[0];
                    }

                    bool validInput; // Used for checking if deposit and withdrawal inputs are valid numbers

                    switch (cmdChar)
                    {
                        case 'B':
                            Console.Clear();
                            Console.WriteLine("| Account Balance |");
                            Console.WriteLine("-------------------");
                            Console.WriteLine("");
                            Console.WriteLine($"Your balance is ${currentUser.Account.GetBalance()}");
                            break;
                        case 'D':
                            Console.Clear();
                            Console.WriteLine("| Deposit |");
                            Console.WriteLine("------------");
                            Console.WriteLine("");
                            Console.Write("Deposit amount: ");

                            string depositAmountString = Console.ReadLine();

                            Console.WriteLine("");
                            Console.Write("Description: ");

                            string depositDescription = Console.ReadLine();

                            decimal depositAmountInt; // Holds converted input if parse is successful
                            validInput = Decimal.TryParse(depositAmountString, out depositAmountInt);

                            if (validInput && depositAmountInt > 0)
                            {
                                currentUser.Account.Deposit(depositAmountInt, depositDescription);
                                Console.WriteLine("");
                                Console.WriteLine($"Deposit successful. Your new balance is {currentUser.Account.GetBalance()}");
                            }
                            else
                            {
                                Console.WriteLine("Invalid amount. Please enter a non negative number.");
                            }
                            break;
                        case 'W':
                            Console.Clear();
                            Console.WriteLine("| Withdraw |");
                            Console.WriteLine("------------");
                            Console.WriteLine("");
                            Console.Write("Withdrawal amount: ");

                            string withdrawalAmountString = Console.ReadLine();

                            Console.WriteLine("");
                            Console.Write("Description: ");

                            string withdrawalDescription = Console.ReadLine();

                            decimal withdrawalAmountInt; // Holds converted input if parse is successful
                            validInput = Decimal.TryParse(withdrawalAmountString, out withdrawalAmountInt);

                            if (validInput && withdrawalAmountInt > currentUser.Account.GetBalance())
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Withdraw amount is greater than current balance.");
                            }
                            else if (validInput && withdrawalAmountInt > 0)
                            {
                                currentUser.Account.Withdraw(withdrawalAmountInt, withdrawalDescription);
                                Console.WriteLine("");
                                Console.WriteLine($"Withdraw successful. Your new balance is {currentUser.Account.GetBalance()}");
                            }
                            else
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Invalid amount. Please enter a non negative number.");
                            }
                            break;
                        case 'T':
                            Console.Clear();
                            Console.WriteLine("|              Transaction History              |");
                            Console.WriteLine("-------------------------------------------------");
                            foreach (Transaction transaction in currentUser.Account.GetTransactionHistory())
                            {
                                oldFontColor = Console.ForegroundColor;
                                switch (transaction.Type)
                                {
                                    case "deposit":
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine($" {transaction.Date.ToString("d")} - {transaction.Date.ToString("t")} | +{transaction.Amount} | {transaction.Description}");
                                        break;
                                    case "withdrawal":
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($" {transaction.Date.ToString("d")} - {transaction.Date.ToString("t")} | -{transaction.Amount} | {transaction.Description}");
                                        break;
                                    default:
                                        break;
                                }
                                Console.ForegroundColor = oldFontColor;
                                Console.WriteLine("-------------------------------------------------");
                            }
                            break;
                        case 'L': // Logout
                            currentUser = null;
                            isLoggedIn = false;
                            Console.Clear();
                            Console.WriteLine("Welcome to CashSource, the alternate source for online banking! Please [L]ogin or [R]egister...");
                            break;
                        case 'H':
                            Console.Clear();
                            Console.WriteLine("| Available Commands |");
                            Console.WriteLine("----------------------");
                            Console.WriteLine(" [B]alance - View Account Balance\n\n [D]eposit - Make a deposit\n\n [W]ithdrawal - Make a withdrawal\n\n [T]ransactions - View transaction History\n\n [H]elp - View Available Commands\n\n [L]ogout\n\n [Q]uit");
                            break;
                        case 'Q': // Quit
                            done = true;
                            break;
                        default:
                            Console.WriteLine("");
                            Console.WriteLine($"{consoleInput} is not a valid command. Type [H]elp to view available commands.");
                            break;
                    }
                }
                else
                {
                    cmdChar = ' ';
                    consoleInput = Console.ReadLine();
                    if (consoleInput != "")
                    {
                        cmdChar = consoleInput.ToUpper()[0];
                    }

                    switch (cmdChar)
                    {
                        case 'L':
                            Console.Clear();
                            Console.WriteLine("| Login |");
                            Console.WriteLine("---------");
                            Console.WriteLine("");
                            Console.Write(" Username: ");
                            inputName = Console.ReadLine();
                            User foundUser = UserHelper.FindByName(inputName);
                            if (foundUser != null)
                            {
                                Console.WriteLine("");
                                Console.Write(" Password: ");
                                // Change font color to hide user password, not secure if user manually changes font or background color while typing
                                oldFontColor = Console.ForegroundColor;
                                Console.ForegroundColor = Console.BackgroundColor;

                                inputPassword = Console.ReadLine();
                                Console.ForegroundColor = oldFontColor;
                                if (foundUser.Password == inputPassword)
                                {
                                    currentUser = foundUser;
                                    isLoggedIn = true;
                                    Console.Clear();
                                    Console.WriteLine($"Welcome {currentUser.Name}");
                                }
                                else
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine("Invalid password. Please enter [L] to log in again or [R] to register a new user.");
                                }

                            }
                            else
                            {
                                Console.WriteLine("");
                                Console.WriteLine($"User {inputName} does not exist. Please enter [L] to log in again or [R] to register a new user.");
                            }
                            break;
                        case 'R':
                            Console.Clear();
                            Console.WriteLine("| Register |");
                            Console.WriteLine("------------");
                            Console.WriteLine("");
                            Console.Write(" Username: ");
                            inputName = Console.ReadLine();

                            if (UserHelper.UserNameAvailable(inputName))
                            {
                                Console.WriteLine("");
                                Console.Write(" Password: ");
                                // Change font color to hide user password, not secure if user manually changes font or background color while typing
                                oldFontColor = Console.ForegroundColor;
                                Console.ForegroundColor = Console.BackgroundColor;
                                inputPassword = Console.ReadLine();
                                Console.ForegroundColor = oldFontColor;

                                User newUser = new User(inputName, inputPassword);
                                UserHelper.AddNew(newUser);
                                Console.WriteLine("");
                                Console.WriteLine("You have successfully registered. Please [L]ogin...");

                            }
                            else
                            {
                                Console.WriteLine("");
                                Console.WriteLine($"Username {inputName} already exists in our system. Please [L]ogin or [R]egister with a different username.");
                            }
                            break;
                        case 'H':
                            Console.Clear();
                            Console.WriteLine("| Available Commands |");
                            Console.WriteLine("----------------------");
                            Console.WriteLine(" [L]ogin\n\n [R]egister\n\n [H]elp\n\n [Q]uit");
                            break;
                        case 'Q': // Quit
                            done = true;
                            break;
                        default:
                            Console.WriteLine("");
                            Console.WriteLine($"{consoleInput} is not a valid command. Type [H]elp to view available commands.");
                            break;
                    }
                }
            }
            Console.ReadLine();
        }
    }
}