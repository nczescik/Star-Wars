using System.Collections.Generic;

namespace WebApi.Models
{
    public class EpisodeModel
    {
        public long? EpisodeId { get; set; }
        public string Name { get; set; }

        public IList<long> CharacterIds { get; set; }
    }
}
