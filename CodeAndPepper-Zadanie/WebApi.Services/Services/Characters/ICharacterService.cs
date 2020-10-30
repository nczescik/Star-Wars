using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Services.Dto;

namespace WebApi.Services.Services.Characters
{
    public interface ICharacterService
    {
        CharacterDto GetCharacter(long characterId);
        IList<CharacterDto> GetCharactersList();
        void DeleteCharacter(long characterId);
        void DeleteCharacterCascade(long characterId);
        Task<CharacterDto> GetCharacterAsync(long characterId);
        Task<IList<CharacterDto>> GetCharactersListAsync();
        Task DeleteCharacterAsync(long characterId);
        Task DeleteCharacterCascadeAsync(long characterId);
    }
}
