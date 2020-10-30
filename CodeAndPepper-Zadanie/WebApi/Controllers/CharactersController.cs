using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
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

        [HttpGet("GetCharacter/{userId}")]
        public IActionResult GetCharacter(long userId)
        {
            var character = _characterService.GetCharacter(userId);

            var model = new CharacterModel
            {
                Name = character.Name,
                Episodes = character.Episodes.Select(e => e.Name).ToList(),
                Planet = character.Planet?.Name,
                Friends = character.Friends.Select(f => f.Name).ToList()
            };

            var json = JsonConvert.SerializeObject(model,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return Content(json, "application/json");
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

            return Content(json, "application/json");
        }

        [HttpPost("Delete")]
        public IActionResult DeleteCharacter(long characterId)
        {
            _characterService.DeleteCharacter(characterId);
            return Ok();
        }

        [HttpPost("DeleteCascade")]
        public IActionResult DeleteCharacterCascade(long characterId)
        {
            _characterService.DeleteCharacterCascade(characterId);
            return Ok();
        }


        #region Async methods

        [HttpGet("GetCharacterAsync/{userId}")]
        public async Task<IActionResult> GetCharacterAsync(long userId)
        {
            var character = await _characterService.GetCharacterAsync(userId);

            var model = new CharacterModel
            {
                Name = character.Name,
                Episodes = character.Episodes.Select(e => e.Name).ToList(),
                Planet = character.Planet?.Name,
                Friends = character.Friends.Select(f => f.Name).ToList()
            };

            var json = JsonConvert.SerializeObject(model,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return Content(json, "application/json");
        }

        [HttpGet("GetCharactersAsync")]
        public async Task<IActionResult> GetCharactersAsync()
        {
            var result = new List<CharacterModel>();
            var characters = await _characterService.GetCharactersListAsync();

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

            return Content(json, "application/json");
        }

        [HttpPost("DeleteAsync")]
        public async Task<IActionResult> DeleteCharacterAsync(long characterId)
        {
            await _characterService.DeleteCharacterAsync(characterId);
            return Ok();
        }

        [HttpPost("DeleteCascadeAsync")]
        public async Task<IActionResult> DeleteCharacterCascadeAsync(long characterId)
        {
            await _characterService.DeleteCharacterCascadeAsync(characterId);
            return Ok();
        }

        #endregion
    }
}
