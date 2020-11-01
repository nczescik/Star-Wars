using System.Collections.Generic;
using WebApi.DAL.Entities;
using WebApi.Services.Dto;
using WebAPI.DAL.Interfaces;

namespace WebApi.Services.Services.Planets
{
    public class PlanetService : IPlanetService
    {
        private readonly IRepository<Planet> _planetRepository;

        public PlanetService(
            IRepository<Planet> planetRepository
            )
        {
            _planetRepository = planetRepository;
        }

        public long CreatePlanet(PlanetDto dto)
        {
            var planet = new Planet
            {
                Name = dto.Name
            };

            var planetId = _planetRepository.Add(planet);

            return planetId;
        }

        public PlanetDto GetPlanet(long id)
        {
            var planet = _planetRepository.GetById(id);
            var planetDto = new PlanetDto
            {
                PlanetId = planet.Id,
                Name = planet.Name
            };

            return planetDto;
        }

        public IList<PlanetDto> GetPlanetsList()
        {
            var list = _planetRepository.GetAll();

            var dtos = new List<PlanetDto>();
            foreach (var planet in list)
            {
                var planetDto = new PlanetDto
                {
                    PlanetId = planet.Id,
                    Name = planet.Name
                };

                dtos.Add(planetDto);
            }

            return dtos;
        }

        public long UpdatePlanet(PlanetDto dto)
        {
            var planet = _planetRepository.GetById(dto.PlanetId);

            planet.Name = dto.Name;

            var id = _planetRepository.Update(planet);

            return id;
        }

        public void DeletePlanet(long planetId)
        {
            var planet = _planetRepository.GetById(planetId);
            planet.IsDeleted = true;
            _planetRepository.Update(planet);
        }

        public void DeletePlanetCascade(long planetId)
        {
            var planet = _planetRepository.GetById(planetId);
            _planetRepository.Delete(planet);
        }
    }
}
