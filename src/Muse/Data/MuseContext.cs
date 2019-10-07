using Microsoft.EntityFrameworkCore;

namespace Muse
{
    public class MuseContext : DbContext
    {
        public MuseContext()
            : base((new DbContextOptionsBuilder<MuseContext>().UseNpgsql("")).Options)
        {
        }

        public MuseContext(DbContextOptions<MuseContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Local.Band> Bands { get; set; }
        public DbSet<Models.Local.Album> Albums { get; set; }
        public DbSet<Models.Local.Song> Songs { get; set; }
        public DbSet<Models.Local.Genre> Genres { get; set; }
        public DbSet<Models.Local.SingleTrack> SingleTracks { get; set; }
        public DbSet<Models.Local.PlaylistTrack> PlaylistTracks { get; set; }
        public DbSet<Models.Local.Playlist> Playlists { get; set; }
        public DbSet<Models.Local.BandGenre> BandGenre { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Local.BandGenre>(entity =>
            {
                entity.HasKey(e => new { e.BandId, e.GenreId });
            });
        }
    }
}
