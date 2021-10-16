using System;

using Hanna.Cosmos.Entitys;


using HannaCS.Cosmos.Entitys;

using Microsoft.EntityFrameworkCore;

namespace Hanna.Cosmos {
	public class UsersContext : DbContext {
		public DbSet<WCUser> Users { get; set; }
		public DbSet<Wallet> Wallets { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseCosmos(
                Environment.GetEnvironmentVariable("COSMOS_ENDPOINT"),
                Environment.GetEnvironmentVariable("COSMOS_PKEY"),
                databaseName: "UsersDB");

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<WCUser>()
				.HasOne(u => u.LastMessage)
				.WithOne(lm => lm.Author)
				.HasForeignKey<LastMessage>(lm => lm.AuthorId);

			modelBuilder.Entity<WCUser>()
		    	.HasOne(u => u.Wallet)
	    		.WithOne(w => w.User)
    			.HasForeignKey<Wallet>(w => w.WalletId);

			modelBuilder.Entity<WCUser>()
				.HasOne(u => u.Inventory)
				.WithOne(i => i.User)
				.HasForeignKey<Inventory>(i => i.InventoryId);

			/**modelBuilder.Entity<Wallet>()
				.HasKey(w => w.WalletId);

			modelBuilder.Entity<Inventory>()
				.HasKey(i => i.InventoryId); **/
        }
	}
}