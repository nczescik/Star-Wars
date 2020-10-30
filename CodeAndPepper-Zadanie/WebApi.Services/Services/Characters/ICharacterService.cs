using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Services.Dto;

namespace WebApi.Services.Services.Characters
{
    public interface ICharacterService
    {
        Task<IList<CharacterDto>> GetCharactersListAsync();
        IList<CharacterDto> GetCharactersList();
        CharacterDto GetCharacter(long characterId);
        Task<CharacterDto> GetCharacterAsync(long characterId);
        void DeleteCharacter(long characterId);
        void DeleteCharacterCascade(long characterId);
        Task DeleteCharacterAsync(long characterId);
        Task DeleteCharacterCascadeAsync(long characterId);
    }
}
