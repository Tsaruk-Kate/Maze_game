using labyrinth_gamе.DataBase.Tables;
using Microsoft.EntityFrameworkCore;

namespace labyrinth_gamе.DataBase
{
    public class DataBaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Record> Records { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite
                (@"DataSource=.\..\..\..\DataBase\Records.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Record>()
                .HasKey(r => r.RecordId);

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<Record>()
                .HasOne(r => r.User)
                .WithOne()
                .HasForeignKey<Record>(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
