using System.Collections.Generic;

namespace WebApi.Services.Dto
{
    public class MachineDto
    {
        public long MachineId { get; set; }
        public string Name { get; set; }

        public long? PlanetId { get; set; }
        public PlanetDto Planet { get; set; }


        public IList<long> EpisodeIds { get; set; }
        public IList<EpisodeDto> Episodes { get; set; }
        public IList<long> FriendIds { get; set; }
        public IList<CharacterDto> Friends { get; set; }
    }
}
