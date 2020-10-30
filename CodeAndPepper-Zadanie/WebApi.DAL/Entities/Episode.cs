using System.Collections.Generic;

namespace WebApi.DAL.Entities
{
    public class Episode : Entity
    {
        public Episode()
        {
            Characters = new List<CharacterEpisode>();
        }
        public string EpisodeName { get; set; }
        public List<CharacterEpisode> Characters { get; set; }
    }
}
