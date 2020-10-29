using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using WebApi.Models;
using WebApi.Services.Dto;
using WebApi.Services.Services.Characters;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        public CharactersController(
            ICharacterService characterService
            )
        {
            _characterService = characterService;
        }

        [HttpGet("GetCharacters")]
        public IActionResult GetCharacters()
        {
            var result = new List<CharacterModel>();
            var characters = _characterService.GetCharactersList();

            foreach (var character in characters)
            {
                var model = new CharacterModel
                {
                    Name = character.Name,
                    Episodes = character.Episodes.Select(e => e.Name).ToList(),
                    Planet = character.Planet?.Name,
                    Friends = character.Friends.Select(f => f.Name).ToList()
                };

                result.Add(model);
            }

            var json = "{\"characters\":" + JsonConvert.SerializeObject(result,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            }) + "}";

            return Ok(json);

        }
    }
}
