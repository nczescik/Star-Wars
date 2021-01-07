using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Models.Characters;
using WebApi.Services.Services.Characters;
using WebApi.Services.Services.Episodes;

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

            var model = new CharacterViewModel
            {
                Name = character.Name,
                Episodes = character.Episodes.Select(e => e.Name).ToList(),
                Planet = character.Planet?.Name,
                Friends = character.Friends.Select(f => f.Name).ToList()
            };

            var json = JsonHelper<CharacterViewModel>.JsonConverter(model, "character");

            return Content(json, "application/json");
        }

        [HttpGet("GetCharacters")]
        public IActionResult GetCharacters()
        {
            var result = new List<CharacterViewModel>();
            var characters = _characterService.GetCharactersList();

            foreach (var character in characters)
            {
                var model = new CharacterViewModel
                {
                    Name = character.Name,
                    Episodes = character.Episodes.Select(e => e.Name).ToList(),
                    Planet = character.Planet?.Name,
                    Friends = character.Friends.Select(f => f.Name).ToList()
                };

                result.Add(model);
            }

            var json = JsonHelper<List<CharacterViewModel>>.JsonConverter(result, "characters");

            return Content(json, "application/json");
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteCharacter(long characterId)
        {
            _characterService.DeleteCharacter(characterId);
            return Ok();
        }

        [HttpDelete("DeleteCascade")]
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

            if (character == null)
            {
                return Ok(new { Message = "Character doesn't exist" });
            }

            var model = new CharacterViewModel
            {
                Name = character.Name,
                Episodes = character.Episodes.Select(e => e.Name).ToList(),
                Planet = character.Planet?.Name,
                Friends = character.Friends.Select(f => f.Name).ToList()
            };

            var json = JsonHelper<CharacterViewModel>.JsonConverter(model, "character");

            return Content(json, "application/json");
        }

        [HttpGet("GetCharactersAsync")]
        public async Task<IActionResult> GetCharactersAsync()
        {
            var result = new List<CharacterViewModel>();
            var characters = await _characterService.GetCharactersListAsync();

            foreach (var character in characters)
            {
                var model = new CharacterViewModel
                {
                    Name = character.Name,
                    Episodes = character.Episodes.Select(e => e.Name).ToList(),
                    Planet = character.Planet?.Name,
                    Friends = character.Friends.Select(f => f.Name).ToList()
                };

                result.Add(model);
            }

            var json = JsonHelper<List<CharacterViewModel>>.JsonConverter(result, "characters");

            return Content(json, "application/json");
        }

        [HttpDelete("DeleteAsync")]
        public async Task<IActionResult> DeleteCharacterAsync(long characterId)
        {
            await _characterService.DeleteCharacterAsync(characterId);
            return Ok();
        }

        [HttpDelete("DeleteCascadeAsync")]
        public async Task<IActionResult> DeleteCharacterCascadeAsync(long characterId)
        {
            await _characterService.DeleteCharacterCascadeAsync(characterId);
            return Ok();
        }

        #endregion
    }
}
