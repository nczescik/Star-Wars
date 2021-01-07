using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApi.Helpers;
using WebApi.Models.Planets;
using WebApi.Services.Dto;
using WebApi.Services.Services.Planets;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlanetsController : ControllerBase
    {
        private readonly IPlanetService _planetService;
        public PlanetsController(
             IPlanetService planetService
            )
        {
            _planetService = planetService;
        }

        [HttpPost("Create")]
        public IActionResult CreatePlanet(PlanetModel model)
        {
            var dto = new PlanetDto
            {
                Name = model.Name
            };

            var id = _planetService.CreatePlanet(dto);

            return Ok(new { PlanetId = id });
        }

        [HttpGet("GetPlanet/{planetId}")]
        public IActionResult GetPlanet(long planetId)
        {
            var planet = _planetService.GetPlanet(planetId);

            var model = new PlanetModel
            {
                PlanetId = planet.PlanetId,
                Name = planet.Name
            };

            return Ok(new { Model = model });
        }

        [HttpGet("GetPlanets")]
        public IActionResult GetPlanets()
        {
            var planets = _planetService.GetPlanetsList();

            var result = new List<PlanetModel>();

            foreach (var planet in planets)
            {
                var model = new PlanetModel
                {
                    Name = planet.Name
                };

                result.Add(model);
            }

            var json = JsonHelper<List<PlanetModel>>.JsonConverter(result, "planets");

            return Content(json, "application/json");
        }

        [HttpPut("Update")]
        public IActionResult Update(PlanetModel model)
        {
            if (!model.PlanetId.HasValue || model.PlanetId.Value == 0)
            {
                throw new System.Exception("Incorrect value of planet Id");
            }
            var dto = new PlanetDto
            {
                Name = model.Name
            };

            var id = _planetService.UpdatePlanet(dto);

            return Ok(new { PlanetId = id });
        }

        [HttpDelete("Delete")]
        public IActionResult DeletePlanet(long id)
        {
            _planetService.DeletePlanet(id);

            return Ok();
        }

        [HttpDelete("DeleteCascade")]
        public IActionResult DeletePlanetCascade(long id)
        {
            _planetService.DeletePlanetCascade(id);

            return Ok();
        }
    }
}
