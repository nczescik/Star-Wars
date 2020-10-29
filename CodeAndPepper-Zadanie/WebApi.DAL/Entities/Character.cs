using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.DAL.Entities
{
    public abstract class Character : Entity
    {
        [ForeignKey("Planet")]
        public long? PlanetId { get; set; }
        public virtual Planet Planet { get; set; }


        public List<CharacterEpisode> CharacterEpisode { get; set; }

        public IList<Friendship> Friends { get; set; }
        public IList<Friendship> MainCharacterFriends { get; set; }
    }

    public class Friendship
    {
        public long FriendId { get; set; }
        public Character Friend { get; set; }


        public long CharacterId { get; set; }
        public Character Character { get; set; }
    }

    public class CharacterEpisode
    {
        public long EpisodeId { get; set; }
        public Episode Episode { get; set; }
        public long CharacterId { get; set; }
        public Character Character { get; set; }
    }
}
