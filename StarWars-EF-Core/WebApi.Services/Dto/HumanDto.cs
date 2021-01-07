namespace WebApi.Services.Dto
{
    public class HumanDto : CharacterCollectionsDto
    {
        public long HumanId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public long? PlanetId { get; set; }
        public PlanetDto Planet { get; set; }

    }
}
