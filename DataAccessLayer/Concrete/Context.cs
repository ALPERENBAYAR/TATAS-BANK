using EntityLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-GRF6F3E\\SQLEXPRESS;Database=Bankacilik; integrated security=True; TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           


            // Account - Bank
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Bank)
                .WithMany(b => b.Accounts)
                .HasForeignKey(a => a.BankId)
                .OnDelete(DeleteBehavior.NoAction);

         

            // Customer - Branch
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Branch)
                .WithMany()
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.NoAction);

            // Employee - Account
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            // Employee - Branch
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Branch)
                .WithMany()
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.NoAction);

           

            
            

            // Transaction - Account
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            // Decimal hassasiyetleri
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.TransactionFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Balance)
                .HasPrecision(18, 2);
        }



        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
