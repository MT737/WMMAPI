using Microsoft.EntityFrameworkCore;
using WMMAPI.Database.Models;
using System;

namespace WMMAPI.Database
{
    public class WMMContext : DbContext
    {
        // DT Sets
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransactionType> TransactionTypes { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Vendor> Vendors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
            @"Server=(localdb)\MSSQLLocalDB;Database=WMM;Integrated Security=True");
        }

        // On Model Creation
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Set Non-key uniques
            builder.Entity<User>()
                .HasIndex(u => u.EmailAddress)
                .IsUnique();

            // Set delete behaviors outside the universal setting
            builder.Entity<Transaction>()
                .HasOne(e => e.Account)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Entity<Transaction>()
                .HasOne(e => e.Category)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Entity<Transaction>()
                .HasOne(e => e.TransactionType)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Entity<Transaction>()
                .HasOne(e => e.Vendor)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
