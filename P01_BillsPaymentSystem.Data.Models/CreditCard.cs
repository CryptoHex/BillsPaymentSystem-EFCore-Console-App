using System;
using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_BillsPaymentSystem.Data.Models
{
    public class CreditCard
    {
        public int CreditCardId { get; set; }  

        public DateTime ExpirationDate { get; set; }  

        public decimal Limit { get; private set; }  

        public decimal MoneyOwed { get; private set; }  

        [NotMapped]
        public int PaymentMethodId { get; set; }  
        public PaymentMethod PaymentMethod { get; set; }

        [NotMapped]
        public decimal LimitLeft => Limit - MoneyOwed;



        public decimal Withdrawal(decimal amount)
        {

            if (this.LimitLeft >= amount)
            {
                MoneyOwed += amount;
                return amount = 0;
            } 
            else
            {
                amount -= this.LimitLeft;
                this.MoneyOwed = this.Limit;
                return amount;
            }
        }

        public decimal Deposit(decimal amount)
        {
            if (this.MoneyOwed > 0)
            {
                if (this.MoneyOwed > amount)
                {
                    var toLimit = amount - this.MoneyOwed;
                    this.MoneyOwed = 0;
                    this.Limit += toLimit;
                }
                else
                {
                    this.MoneyOwed -= amount;
                }
            }
            if (this.MoneyOwed <= 0)
            {
                Limit += amount;
            }
            return this.MoneyOwed;
        }
    }
}
