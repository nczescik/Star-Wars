using System.Collections.Generic;

namespace WebApi.Models.Humans
{
    public class HumanModel
    {
        public long? HumanId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public IList<long> FriendIds { get; set; }
        public IList<long> EpisodeIds { get; set; }
    }
}
