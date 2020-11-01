using System.Collections.Generic;
using WebApi.Services.Dto;

namespace WebApi.Services.Services.Planets
{
    public interface IPlanetService
    {
        long CreatePlanet(PlanetDto dto);
        PlanetDto GetPlanet(long id);
        IList<PlanetDto> GetPlanetsList();
        long UpdatePlanet(PlanetDto dto);
        void DeletePlanet(long planetId);
        void DeletePlanetCascade(long planetId);
    }
}
