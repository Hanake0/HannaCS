using Hanna.Cosmos.Entitys;

using Microsoft.EntityFrameworkCore;

namespace Hanna.Cosmos {
    public class ConfigsContext : DbContext{
        public DbSet<EntitySettings> ChannelSettings { get; set; }
        public DbSet<EntitySettings> UserSettings { get; set; }
    }
}