namespace WebApi.Services.Dto
{
    public class CharacterDto : CharacterCollectionsDto
    {
        public long CharacterId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Name { get; set; }

        public PlanetDto Planet { get; set; }
    }
}
