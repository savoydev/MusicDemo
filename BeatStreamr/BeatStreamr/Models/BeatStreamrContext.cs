using System;
using Microsoft.EntityFrameworkCore;
namespace BeatStreamr.Models
{
    public class BeatStreamrContext : DbContext
    {
        public BeatStreamrContext(DbContextOptions<BeatStreamrContext> options)
            : base(options)
        {

        }

        public DbSet<BeatStreamr.Models.User> User { get; set; }
        public DbSet<BeatStreamr.Models.Beat> Beat { get; set; }
        public DbSet<BeatStreamr.Models.Album> Album { get; set; }

    }
}
