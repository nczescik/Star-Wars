using System.Collections.Generic;

namespace WebApi.Models.Machines
{
    public class MachineModel
    {
        public long? MachineId { get; set; }
        public string Name { get; set; }

        public long PlanetId { get; set; }

        public IList<long> FriendIds { get; set; }
        public IList<long> EpisodeIds { get; set; }
    }
}
