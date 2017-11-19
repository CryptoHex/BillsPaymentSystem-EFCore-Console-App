using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_BillsPaymentSystem.Data.Models
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }  

        public decimal Balance { get; private set; }  

        [Required]
        [StringLength(50)]
        public string BankName { get; set; }

        [Required]
        [StringLength(20)]
        public string SwiftCode { get; set; }

        [NotMapped]
        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }


        public decimal Withdrawal(decimal amount)
        {

            if (this.Balance>amount)
            {
                this.Balance -= amount;
                amount = 0;
            }
            else if (amount >= this.Balance)
            {  
                amount -= this.Balance;
                this.Balance = 0;
            }
            return amount;
        }

        public decimal Deposit(decimal amount)
        {
            this.Balance += amount;
            return Balance;
        }
    }
}
