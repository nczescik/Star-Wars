using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.DAL.Entities;
using WebApi.DAL.Enums;

namespace WebApi.DAL
{
    public class StarWarsDbContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Planet> Planets { get; set; }

        public StarWarsDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Created many to many relation for Character-Friend
            builder.Entity<Friendship>()
                .HasKey(e => new { e.FriendId, e.CharacterId });

            builder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Friendship>()
                .HasOne(f => f.Character)
                .WithMany(mf => mf.Friends)
                .HasForeignKey(f => f.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);


            //Created many to many relation for Character-Episode
            builder.Entity<CharacterEpisode>()
                .HasKey(ce => new { ce.CharacterId, ce.EpisodeId });

            builder.Entity<CharacterEpisode>()
                .HasOne(ce => ce.Character)
                .WithMany(c => c.Episodes)
                .HasForeignKey(ce => ce.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CharacterEpisode>()
                .HasOne(ce => ce.Episode)
                .WithMany(e => e.Characters)
                .HasForeignKey(ce => ce.EpisodeId);


            //Adding episodes to database
            var episodes = new List<Episode>();
            foreach (var episodeName in Enum.GetValues(typeof(EpisodeNameEnum)).Cast<EpisodeNameEnum>())
            {
                episodes.Add(new Episode { Id = (long)episodeName, EpisodeName = episodeName.ToString() });
            }

            builder
                .Entity<Episode>()
                .HasData(episodes);


            //Adding planets to database
            builder
                .Entity<Planet>()
                .HasData(new Planet { Id = 1, Name = "Alderaan" });


            //Mapping Discriminator
            builder.Entity<Character>()
                .HasDiscriminator(c => c.Discriminator);

            //Adding Humans to database
            builder
                .Entity<Human>()
                .HasData(new List<Human> {
                    new Human { Id = 1, Firstname = "Luke", Lastname = "Skywalker" },
                    new Human { Id = 2, Firstname = "Darth", Lastname = "Vader" },
                    new Human { Id = 3, Firstname = "Hans", Lastname = "Solo" },
                    new Human { Id = 4, Firstname = "Leia", Lastname = "Organa", PlanetId = 1 },
                    new Human { Id = 5, Firstname = "Wilhuff", Lastname = "Tarkin" }
                });

            //Adding Machines to database
            builder
                .Entity<Machine>()
                .HasData(new List<Machine>
                {
                    new Machine { Id = 6, Name = "C-3PO" },
                    new Machine { Id = 7, Name = "R2-D2" }
                });


            //Adding CharacterEpisode to database
            builder
                .Entity<CharacterEpisode>()
                .HasData(new List<CharacterEpisode> {
                    new CharacterEpisode { CharacterId = 1, EpisodeId = 1 },
                    new CharacterEpisode { CharacterId = 1, EpisodeId = 2 },
                    new CharacterEpisode { CharacterId = 1, EpisodeId = 3 },
                    new CharacterEpisode { CharacterId = 2, EpisodeId = 1 },
                    new CharacterEpisode { CharacterId = 2, EpisodeId = 2 },
                    new CharacterEpisode { CharacterId = 2, EpisodeId = 3 },
                    new CharacterEpisode { CharacterId = 3, EpisodeId = 1 },
                    new CharacterEpisode { CharacterId = 3, EpisodeId = 2 },
                    new CharacterEpisode { CharacterId = 3, EpisodeId = 3 },
                    new CharacterEpisode { CharacterId = 4, EpisodeId = 1 },
                    new CharacterEpisode { CharacterId = 4, EpisodeId = 2 },
                    new CharacterEpisode { CharacterId = 4, EpisodeId = 3 },
                    new CharacterEpisode { CharacterId = 5, EpisodeId = 1 },
                    new CharacterEpisode { CharacterId = 6, EpisodeId = 1 },
                    new CharacterEpisode { CharacterId = 6, EpisodeId = 2 },
                    new CharacterEpisode { CharacterId = 6, EpisodeId = 3 },
                    new CharacterEpisode { CharacterId = 7, EpisodeId = 1 },
                    new CharacterEpisode { CharacterId = 7, EpisodeId = 2 },
                    new CharacterEpisode { CharacterId = 7, EpisodeId = 3 }
                });

            //Adding Friendship to database
            builder
                .Entity<Friendship>()
                .HasData(new List<Friendship> {
                    new Friendship { CharacterId = 1, FriendId = 3 },
                    new Friendship { CharacterId = 1, FriendId = 4 },
                    new Friendship { CharacterId = 1, FriendId = 6 },
                    new Friendship { CharacterId = 1, FriendId = 7 },
                    new Friendship { CharacterId = 2, FriendId = 5 },
                    new Friendship { CharacterId = 3, FriendId = 1 },
                    new Friendship { CharacterId = 3, FriendId = 4 },
                    new Friendship { CharacterId = 3, FriendId = 7 },
                    new Friendship { CharacterId = 4, FriendId = 1 },
                    new Friendship { CharacterId = 4, FriendId = 3 },
                    new Friendship { CharacterId = 4, FriendId = 6 },
                    new Friendship { CharacterId = 4, FriendId = 7 },
                    new Friendship { CharacterId = 5, FriendId = 2 },
                    new Friendship { CharacterId = 6, FriendId = 1 },
                    new Friendship { CharacterId = 6, FriendId = 3 },
                    new Friendship { CharacterId = 6, FriendId = 4 },
                    new Friendship { CharacterId = 6, FriendId = 7 },
                    new Friendship { CharacterId = 7, FriendId = 1 },
                    new Friendship { CharacterId = 7, FriendId = 3 },
                    new Friendship { CharacterId = 7, FriendId = 4 }
                });
        }
    }
}
