using Microsoft.EntityFrameworkCore;
using The9Books.Models;

namespace The9Books
{
    public interface IDBContext
    {
        DbSet<Hadith> Hadiths { get; set; }
    }

    public class SQLiteDBContext : DbContext, IDBContext
    {
        public DbSet<Hadith> Hadiths { get; set; }

        public SQLiteDBContext(DbContextOptions<SQLiteDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hadith>()
                .HasIndex(h => new { h.Book, h.Number })
                .IsUnique();

            modelBuilder.Entity<Hadith>()
                .HasIndex(h => h.Book);

            modelBuilder.Entity<Hadith>()
                .HasIndex(h => h.Number);

            modelBuilder.Entity<Hadith>()
                .Property(h => h.HadithText)
                .IsRequired(false);

            modelBuilder.Entity<Hadith>()
                .Property(h => h.Tafseel)
                .IsRequired(false);

            modelBuilder.Entity<Hadith>()
                .Property(h => h.Book)
                .IsRequired(false)
                .HasMaxLength(50);
        }
    }
}
