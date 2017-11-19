using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_BillsPaymentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_BillsPaymentSystem.Data.EntityConfig
{
    class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder
                .HasOne(pm => pm.User)
                .WithMany(u => u.PaymentMethods)
                .HasForeignKey(pm => pm.UserId);

            builder
                .HasOne(pm => pm.CreditCard)
                .WithOne(c => c.PaymentMethod)
                .HasForeignKey<PaymentMethod>(pm => pm.CreditCardId);

            builder
              .HasOne(pm => pm.BankAccount)
              .WithOne(b => b.PaymentMethod)
              .HasForeignKey<PaymentMethod>(pm => pm.BankAccountId);

            builder
                 .HasIndex(pm => new { pm.BankAccountId, pm.CreditCardId, pm.UserId}).IsUnique();

        }
    }
}
