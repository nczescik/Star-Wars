using System.Collections.Generic;
using WebApi.Services.Dto;

namespace WebApi.Services.Services.Characters
{
    public interface ICharacterService
    {
        IList<CharacterDto> GetCharactersList();
    }
}
