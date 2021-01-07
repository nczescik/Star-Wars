using System.Collections.Generic;

namespace WebApi.Services.Dto
{
    public class MachineDto : CharacterCollectionsDto
    {
        public long MachineId { get; set; }
        public string Name { get; set; }

        public long? PlanetId { get; set; }
        public PlanetDto Planet { get; set; }
    }
}
