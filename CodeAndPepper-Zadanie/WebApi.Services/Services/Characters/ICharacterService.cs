using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.DAL.Entities;
using WebApi.Services.Dto;

namespace WebApi.Services.Services.Characters
{
    public interface ICharacterService
    {
        CharacterDto GetCharacter(long characterId);
        IList<CharacterDto> GetCharactersList();
        void DeleteCharacter(long characterId);
        void DeleteCharacterCascade(long characterId);
        List<CharacterEpisode> AssignEpisodes(Character character, CharacterCollectionsDto dto);
        List<CharacterEpisode> UpdateEpisodes(Character character, CharacterCollectionsDto dto);
        List<Friendship> AssignFriends(Character character, CharacterCollectionsDto dto);
        List<Friendship> UpdateFriends(Character character, CharacterCollectionsDto dto);
        Task<CharacterDto> GetCharacterAsync(long characterId);
        Task<IList<CharacterDto>> GetCharactersListAsync();
        Task DeleteCharacterAsync(long characterId);
        Task DeleteCharacterCascadeAsync(long characterId);
    }
}
