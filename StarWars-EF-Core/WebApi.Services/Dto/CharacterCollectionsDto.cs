using System.Collections.Generic;

namespace WebApi.Services.Dto
{
    public class CharacterCollectionsDto
    {
        public IList<long> EpisodeIds { get; set; }
        public IList<EpisodeDto> Episodes { get; set; }
        public IList<long> FriendIds { get; set; }
        public IList<CharacterDto> Friends { get; set; }
    }
}
