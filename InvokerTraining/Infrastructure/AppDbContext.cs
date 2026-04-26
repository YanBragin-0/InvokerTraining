using InvokerTraining.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace InvokerTraining.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> contextOptions) : base(options: contextOptions) { }
        public DbSet<Player> Players { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(x => 
            { 
                x.HasKey(x => x.Id);
                x.HasMany<GameSession>().WithOne().HasForeignKey(gs => gs.PlayerId);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
