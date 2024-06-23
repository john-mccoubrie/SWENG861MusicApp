using Microsoft.EntityFrameworkCore;

namespace MusicApp.Models
{
    public class MusicDbContext : DbContext
    {
        public MusicDbContext(DbContextOptions<MusicDbContext> options) : base(options)
        {
        }

        public DbSet<ArtistModel> Artists { get; set; }
        public DbSet<SongModel> Songs { get; set; }
        public DbSet<PlaylistModel> Playlists { get; set; }
        public DbSet<PlaylistItemModel> PlaylistItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtistModel>()
                .HasKey(a => a.ArtistId);

            modelBuilder.Entity<SongModel>()
                .HasKey(s => s.SongId);

            modelBuilder.Entity<SongModel>()
                .HasOne(s => s.Artist)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.ArtistId);

            modelBuilder.Entity<PlaylistModel>()
                .HasKey(p => p.PlaylistId);

            modelBuilder.Entity<PlaylistItemModel>()
                .HasKey(pi => new { pi.PlaylistId, pi.SongId });

            modelBuilder.Entity<PlaylistItemModel>()
                .HasOne(pi => pi.Playlist)
                .WithMany(p => p.Items)
                .HasForeignKey(pi => pi.PlaylistId);

            modelBuilder.Entity<PlaylistItemModel>()
                .HasOne(pi => pi.Song)
                .WithMany(s => s.PlaylistItems)
                .HasForeignKey(pi => pi.SongId);

            modelBuilder.Entity<ArtistModel>().ToTable("Artist");
            modelBuilder.Entity<SongModel>().ToTable("Song");
            modelBuilder.Entity<PlaylistModel>().ToTable("Playlist");
            modelBuilder.Entity<PlaylistItemModel>().ToTable("PlaylistItem");
        }
    }
}
