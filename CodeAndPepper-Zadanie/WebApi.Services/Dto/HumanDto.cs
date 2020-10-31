using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WebApi.Services.Dto
{
    public class HumanDto
    {
        public long HumanId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public long? PlanetId { get; set; }
        public PlanetDto Planet { get; set; }


        public IList<long> EpisodeIds { get; set; }
        public IList<EpisodeDto> Episodes { get; set; }
        public IList<long> FriendIds { get; set; }
        public IList<CharacterDto> Friends { get; set; }

    }
}
