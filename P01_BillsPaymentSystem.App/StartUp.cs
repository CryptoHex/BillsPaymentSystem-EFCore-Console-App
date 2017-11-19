using System;
using P01_BillsPaymentSystem.Data;
using Microsoft.EntityFrameworkCore;
using P01_BillsPaymentSystem.Data.Models;
using System.Linq;
using System.Globalization;

namespace P01_BillsPaymentSystem.App
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using (var db = new BillsPaymentSystemContext())
            {
                //First seed data, run the application and then comment it
                //
                db.Database.EnsureDeleted();
                db.Database.Migrate();
                Seed(db);

                ////then uncomment the lower functions to test the Deposit,  Withdrawal and PayBills functions
                ////
                //Console.Write("Id: ");
                //var id = int.Parse(Console.ReadLine());
                //DisplayInfo(id, db);
                //Console.WriteLine();
                //Console.Write("Amount: ");
                //decimal amount = decimal.Parse(Console.ReadLine());
                //PayBills(id,amount, db);
            }


        }

        private static void Seed(BillsPaymentSystemContext db)
        {
            using (db)
            {
                var user = new User()
                {
                    FirstName = "Pesho",
                    LastName = "Stamatov",
                    Email = "pesho@abv.bg",
                    Password = "SomePa$$"
                };

                var creditCards = new CreditCard[]
                {
                new CreditCard()
                {
                    ExpirationDate = DateTime.ParseExact("20.05.2020", "dd.MM.yyyy", null),
             //ToDo       Limit = 1000m,
             //ToDo       MoneyOwed = 5m
                },
                new CreditCard()
                {
                    ExpirationDate = DateTime.ParseExact("20.05.2032", "dd.MM.yyyy", null),
              //ToDo      Limit = 400m,
             //ToDo       MoneyOwed = 300m
                },
                };
                creditCards[0].Deposit(600m);
                creditCards[1].Deposit(900m);

                var bankAccount = new BankAccount()
                {
                    BankName = "Mrusnite Schweizarzi",
                    SwiftCode = "SWSSBANK"
                };
                bankAccount.Deposit(3000m);

                var paymentMethods = new PaymentMethod[]
                {
                    new PaymentMethod()
                    {
                        User = user,
                        CreditCard = creditCards[0],
                        Type = PaymentType.Card
                    },
                    new PaymentMethod()
                    {
                        User = user,
                        CreditCard = creditCards[1],
                        Type = PaymentType.Card
                    },
                    new PaymentMethod()
                    {
                        User = user,
                        BankAccount = bankAccount,
                        Type = PaymentType.Bank
                    },
                };

                db.Users.Add(user);
                db.CreditCards.AddRange(creditCards);
                db.BankAccounts.Add(bankAccount);
                db.PaymentMethods.AddRange(paymentMethods);
                db.PaymentMethods.AddRange(paymentMethods);
                db.SaveChanges();
            }
        }

        private static void DisplayInfo(int id, BillsPaymentSystemContext db)
        {

            var user = db.Users
                .Where(u => u.UserId == id)
                .Select(u => new
                {
                    Name = $"{u.FirstName} {u.LastName}",
                    CreditCards = u.PaymentMethods.Where(pm => pm.Type == PaymentType.Card).Select(pm => pm.CreditCard).ToList(),
                    BankAccounts = u.PaymentMethods.Where(pm => pm.Type == PaymentType.Bank).Select(pm => pm.BankAccount).ToList(),
                })
                .FirstOrDefault();





            Console.WriteLine($"User: {user.Name}");

            if (user.BankAccounts.Any())
            {
                Console.WriteLine("Bank Accounts: ");
                foreach (var b in user.BankAccounts)
                {
                    Console.WriteLine($"--- ID: {b.BankAccountId}");
                    Console.WriteLine($"--- Balance: {b.Balance}");
                    Console.WriteLine($"--- Bank: {b.BankName}");
                    Console.WriteLine($"--- Swift: {b.SwiftCode}");
                }
            }

            if (user.CreditCards.Any())
            {
                Console.WriteLine("Credit Cards: ");
                foreach (var c in user.CreditCards)
                {
                    Console.WriteLine($"--- ID: {c.CreditCardId}");
                    Console.WriteLine($"--- Limit: {c.Limit}");
                    Console.WriteLine($"--- Limit Left: {c.LimitLeft}");
                    Console.WriteLine($"--- Expiration Date: {c.ExpirationDate.ToString("yyyy/MM", CultureInfo.InvariantCulture)}");
                }
            }

        }

        private static void PayBills(int id, decimal amount, BillsPaymentSystemContext db)
        {
            var user = db.Users
                .Where(u => u.UserId == id)
                .Select(u => new
                {
                    Name = $"{u.FirstName} {u.LastName}",
                    CreditCards = u.PaymentMethods.Where(pm => pm.Type == PaymentType.Card).Select(pm => pm.CreditCard).ToList(),
                    BankAccounts = u.PaymentMethods.Where(pm => pm.Type == PaymentType.Bank).Select(pm => pm.BankAccount).ToList(),
                })
                .FirstOrDefault();

                if (amount > 0)
                {
                    foreach (var b in user.BankAccounts)
                    {
                        amount = b.Withdrawal(amount);
                    }
                }

                if (amount > 0 )
                {
                    foreach (var c in user.CreditCards)
                    {
                     amount = c.Withdrawal(amount);
                    }
                }

                if (amount == 0)
                {
                    foreach (var b in user.BankAccounts)
                    {
                          db.Entry(b).State = EntityState.Modified;
                    }
                    foreach (var c in user.CreditCards)
                    {
                          db.Entry(c).State = EntityState.Modified;  
                    }
                    db.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Insufficient funds!");
                }
        }
    }
}
