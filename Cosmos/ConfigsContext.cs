using System;

using Hanna.Cosmos.Entitys;

using Microsoft.EntityFrameworkCore;

namespace Hanna.Cosmos {
    public class ConfigsContext : DbContext{
        public DbSet<EntitySettings> ChannelSettings { get; set; }
        public DbSet<EntitySettings> UserSettings { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			=> optionsBuilder.UseCosmos(
				Environment.GetEnvironmentVariable("COSMOS_ENDPOINT"),
				Environment.GetEnvironmentVariable("COSMOS_PKEY"),
				databaseName: "ConfigsDB");
	}
}