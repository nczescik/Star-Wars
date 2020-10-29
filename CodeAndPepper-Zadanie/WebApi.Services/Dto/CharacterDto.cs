using System.Collections.Generic;

namespace WebApi.Services.Dto
{
    public class CharacterDto
    {
        public long CharacterId { get; set; }
        public string Name { get; set; }

        public PlanetDto Planet { get; set; }

        public IList<EpisodeDto> Episodes { get; set; }
        public IList<FriendDto> Friends { get; set; }
    }
}
