using System.Collections.Generic;

namespace WebApi.Models.Characters
{
    public class CharacterModel
    {
        public long? PlanetId { get; set; }
        public long Discriminator { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Name { get; set; }

        public IList<long> EpisodeIds { get; set; }
        public IList<long> FriendIds { get; set; }
    }
}
