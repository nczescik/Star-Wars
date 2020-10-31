using System.Collections.Generic;

namespace WebApi.Services.Dto
{
    public class EpisodeDto
    {
        public long EpisodeId { get; set; }
        public string Name { get; set; }

        public IList<long> CharacterIds { get; set; }
    }
}
