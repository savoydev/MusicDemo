using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Mime;
namespace BeatStreamr.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BeatStreamrContext(
            serviceProvider.GetRequiredService<DbContextOptions<BeatStreamrContext>>()))
            {
                //Look for any users that already exist
                if (!context.User.Any())
                {
                    SeedUsers();
                }

                //Look for any beats that already exist
                if (!context.Beat.Any())
                {
                    SeedBeats();
                }

                void SeedUsers()
                {
                    context.User.AddRange(
                    new User
                    {
                        Name = "Kanye West",
                        UserName = "Kanye West"
                    },
                    new User
                    {
                        Name = "John Smith",
                        UserName = "Lil Smith"
                    },
                    new User
                    {
                        Name = "Daniel Bradford",
                        UserName = "Raddy Brad"
                    },
                    new User
                    {
                        Name = "Hafsa Ahmed",
                        UserName = "HafStar"
                    },
                    new User
                    {
                        Name = "Lisa Bradford",
                        UserName = "Yung Tallent"
                    },
                    new User
                    {
                        Name = "Nathan Bradford",
                        UserName = "Nate da Skate"
                    }
                );
                }



                void SeedBeats()
                {
                    context.Beat.AddRange(
                    new Beat
                    {
                        Title = "Soul Beat",
                        BPM = 120,
                        Key = "A#m",
                        Price = 299.99M,
                        ArtistID = 1,
                        BeatFilePath = "/media/beats/horse.mp3"
                    },
                    new Beat
                    {
                        Title = "Rock Beat",
                        BPM = 130,
                        Key = "Dm",
                        Price = 250.99M,
                        ArtistID = 1,
                        BeatFilePath = "/media/beats/horse.mp3"
                    },
                    new Beat
                    {
                        Title = "Electro Beat",
                        BPM = 156,
                        Key = "C",
                        Price = 499.99M,
                        ArtistID = 1,
                        BeatFilePath = "/media/beats/horse.mp3"
                    },
                    new Beat
                    {
                        Title = "Classical Beat",
                        BPM = 99,
                        Key = "Bbm",
                        Price = 199.99M,
                        ArtistID = 1,
                        BeatFilePath = "/media/beats/horse.mp3"
                    }
                );
                }



                context.SaveChanges();
            }
        }
    }
}
