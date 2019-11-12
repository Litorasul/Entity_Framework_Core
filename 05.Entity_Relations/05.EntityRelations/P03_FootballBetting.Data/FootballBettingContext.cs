using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Models;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {

        }

        public FootballBettingContext(DbContextOptions options)
            :base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(
                        "Server=DESKTOP-7JEJ5UL\\SQLEXPRESS01;Database=FootballBetting;Integrated Security=True;");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(e =>
            {
                e.HasKey(t => t.TeamId);

                e.Property(t => t.Name)
                    .HasMaxLength(50)
                    .IsRequired()
                    .IsUnicode();

                e.Property(t => t.LogoUrl)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                e.Property(t => t.Initials)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode();

                e.HasOne(t => t.PrimaryKitColor)
                    .WithMany(c => c.PrimaryKitTeams)
                    .HasForeignKey(t => t.PrimaryKitColorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(t => t.SecondaryKitColor)
                    .WithMany(c => c.SecondaryKitTeams)
                    .HasForeignKey(t => t.SecondaryKitColorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(t => t.Town)
                    .WithMany(to => to.Teams)
                    .HasForeignKey(t => t.TownId);

            });

            modelBuilder.Entity<Color>(e =>
            {
                e.HasKey(c => c.ColorId);
                e.Property(c => c.Name)
                    .HasMaxLength(30)
                    .IsRequired();
            });

            modelBuilder.Entity<Town>(e =>
            {
                e.HasKey(t => t.TownId);

                e.Property(t => t.Name)
                    .HasMaxLength(50)
                    .IsRequired();

                e.HasOne(t => t.Country)
                    .WithMany(c => c.Towns)
                    .HasForeignKey(t => t.CountryId);
            });

            modelBuilder.Entity<Country>(e =>
            {
                e.HasKey(c => c.CountryId);

                e.Property(c => c.Name)
                    .HasMaxLength(50)
                    .IsRequired();
            });

            modelBuilder.Entity<Player>(e =>
            {
                e.HasKey(p => p.PlayerId);

                e.Property(p => p.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                e.HasOne(p => p.Team)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TeamId);

                e.HasOne(p => p.Position)
                    .WithMany(po => po.Players)
                    .HasForeignKey(p => p.PositionId);
            });

            modelBuilder.Entity<Position>(e =>
            {
                e.HasKey(p => p.PositionId);

                e.Property(p => p.Name)
                    .HasMaxLength(50)
                    .IsRequired();
            });

            modelBuilder.Entity<PlayerStatistic>(e =>
            {
                e.HasKey(p => new
                {
                    p.PlayerId, p.GameId
                });

                e.HasOne(ps => ps.Game)
                    .WithMany(g => g.PlayerStatistics)
                    .HasForeignKey(ps => ps.GameId);

                e.HasOne(ps => ps.Player)
                    .WithMany(p => p.PlayerStatistics)
                    .HasForeignKey(ps => ps.PlayerId);
            });

            modelBuilder.Entity<Game>(e =>
            {
                e.HasKey(g => g.GameId);

                e.HasOne(g => g.HomeTeam)
                    .WithMany(ht => ht.HomeGames)
                    .HasForeignKey(g => g.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(g => g.AwayTeam)
                    .WithMany(at => at.AwayGames)
                    .HasForeignKey(g => g.AwayTeamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bet>(e =>
            {
                e.HasKey(b => b.BetId);

                e.HasOne(b => b.Game)
                    .WithMany(g => g.Bets)
                    .HasForeignKey(b => b.GameId);

                e.HasOne(b => b.User)
                    .WithMany(u => u.Bets)
                    .HasForeignKey(b => b.UserId);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.UserId);

                e.Property(u => u.Name)
                    .HasMaxLength(100)
                    .IsRequired(false);

                e.Property(u => u.Email)
                    .HasMaxLength(50)
                    .IsRequired()
                    .IsUnicode(false);

                e.Property(u => u.Username)
                    .HasMaxLength(50)
                    .IsRequired();
            });
        }
    }
}
