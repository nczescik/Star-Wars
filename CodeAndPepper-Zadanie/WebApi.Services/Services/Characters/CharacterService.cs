using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebApi.DAL.Entities;
using WebApi.DAL.Extensions;
using WebApi.Services.Dto;
using WebAPI.DAL.Interfaces;

namespace WebApi.Services.Services.Characters
{
    public class CharacterService : ICharacterService
    {
        private readonly IRepository<Character> _repository;
        public CharacterService(
            IRepository<Character> repository)
        {
            _repository = repository;
        }

        public IList<CharacterDto> GetCharactersList()
        {
            var result = new List<CharacterDto>();
            var characters = _repository
                .GetDbSet()
                .Include(c => c.Episodes)
                .ThenInclude(e => e.Episode)
                .Include(c => c.Planet)
                .Include(c => c.Friends)
                .ToList();

            foreach (var character in characters)
            {
                var characterDto = new CharacterDto
                {
                    CharacterId = character.Id,
                    Name = character.GetName(),
                    Episodes = character.Episodes
                        .Select(e => new EpisodeDto
                        {
                            EpisodeId = e.EpisodeId,
                            Name = e.Episode.EpisodeName
                        }).ToList(),
                    Planet = character.Planet != null ? new PlanetDto
                    {
                        PlanetId = character.Planet.Id,
                        Name = character.Planet.Name
                    } : null,
                    Friends = character.Friends
                        .Select(f => new FriendDto
                        {
                            FriendId = f.FriendId,
                            Name = f.Friend.GetName()
                        }).ToList()
                };

                result.Add(characterDto);
            }

            return result;
        }
    }
}
