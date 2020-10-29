using System.Collections.Generic;

namespace WebApi.DAL.Entities
{
    public class Episode : Entity
    {
        public string EpisodeName { get; set; }
        public List<CharacterEpisode> CharacterEpisode { get; set; }
    }
}
