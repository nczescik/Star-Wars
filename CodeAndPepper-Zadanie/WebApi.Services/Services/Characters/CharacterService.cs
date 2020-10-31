using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DAL;
using WebApi.DAL.Entities;
using WebApi.DAL.Extensions;
using WebApi.Services.Dto;
using WebAPI.DAL.Interfaces;

namespace WebApi.Services.Services.Characters
{
    public class CharacterService : ICharacterService
    {
        private readonly IRepository<Character> _repository;
        private readonly IRepository<Episode> _episodeRepository;
        private readonly StarWarsDbContext _dbContext;
        public CharacterService(
            IRepository<Character> repository,
            IRepository<Episode> episodeRepository,
            StarWarsDbContext dbContext)
        {
            _repository = repository;
            _episodeRepository = episodeRepository;
            _dbContext = dbContext;
        }

        public CharacterDto GetCharacter(long characterId)
        {
            var character = _repository
                .GetDbSet()
                .Include(c => c.Episodes)
                .ThenInclude(e => e.Episode)
                .Include(c => c.Planet)
                .Include(c => c.Friends)
                .ThenInclude(f => f.Friend)
                .Where(c => c.Id == characterId && !c.IsDeleted)
                .FirstOrDefault();

            CharacterDto dto = null;
            if (character != null)
            {
                dto = new CharacterDto
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
                        .Select(f => new CharacterDto
                        {
                            CharacterId = f.FriendId,
                            Name = f.Friend.GetName()
                        }).ToList()
                };
            }

            return dto;
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
                .ThenInclude(f => f.Friend)
                .Where(c => !c.IsDeleted)
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
                        .Select(f => new CharacterDto
                        {
                            CharacterId = f.FriendId,
                            Name = f.Friend.GetName()
                        }).ToList()
                };

                result.Add(characterDto);
            }

            return result;
        }

        public void DeleteCharacter(long characterId)
        {
            var character = _repository.GetById(characterId);
            if (character != null)
            {
                character.IsDeleted = true;
                _repository.Update(character);
            }
        }

        public void DeleteCharacterCascade(long characterId)
        {
            var character = _dbContext.Characters
                .Include(c => c.Friends)
                .ThenInclude(f => f.Friend)
                .Where(c => c.Id == characterId)
                .FirstOrDefault();

            if (character != null)
            {
                foreach (var child in character.Friends.ToList())
                {
                    _dbContext.Remove(child);
                    _dbContext.SaveChanges();
                }

                _repository.Delete(character);
            }
        }

        public List<CharacterEpisode> AssignEpisodes(Character character, CharacterCollectionsDto dto)
        {
            var episodes = new List<CharacterEpisode>();
            foreach (var episodeId in dto.EpisodeIds)
            {
                var episode = _episodeRepository.GetDbSet().Where(e => e.Id == episodeId).FirstOrDefault();
                if (episode == null)
                {
                    throw new Exception("Episode doesn't exist");
                }

                episodes.Add(new CharacterEpisode
                {
                    Character = character,
                    Episode = episode,
                });
            }

            return episodes;
        }

        public List<CharacterEpisode> UpdateEpisodes(Character character, CharacterCollectionsDto dto)
        {
            //Removing old episodes first
            var episodes = new List<CharacterEpisode>();

            var oldEposodeIds = character.Episodes.Select(e => e.Episode.Id).ToList();
            foreach (var id in oldEposodeIds)
            {
                character.Episodes.RemoveAll(e => e.Episode.Id == id);
            }

            return AssignEpisodes(character, dto);
        }

        public List<Friendship> AssignFriends(Character character, CharacterCollectionsDto dto)
        {
            var friends = new List<Friendship>();
            foreach (var friendId in dto.FriendIds)
            {
                var friend = _repository
                    .GetDbSet()
                    .Include(c => c.Friends)
                    .ThenInclude(f => f.Friend)
                    .Where(e => e.Id == friendId)
                    .FirstOrDefault();

                if (friend == null)
                {
                    throw new Exception("Cannot add character which doesn't exist to friends list");
                }

                friends.Add(new Friendship
                {
                    Character = character,
                    Friend = friend,
                });

                //Adding character to his friend's Friends list
                var machineFriend = _repository
                    .GetDbSet()
                    .Include(c => c.Friends)
                    .ThenInclude(f => f.Friend)
                    .Where(e => e.Id == friendId)
                    .FirstOrDefault();

                if (machineFriend != null)
                {
                    machineFriend.Friends.Add(new Friendship
                    {
                        Character = friend,
                        Friend = character
                    });
                }

            }

            return friends;
        }

        public List<Friendship> UpdateFriends(Character character, CharacterCollectionsDto dto)
        {
            if (dto.FriendIds.Contains(character.Id))
            {
                throw new Exception("Cannot add updating character to his own friends list");
            }

            //Removing character from his old friends Friends list first
            var oldFriendIds = character.Friends.Select(e => e.Friend.Id).ToList();
            foreach (var id in oldFriendIds)
            {
                var characterFriends = _repository
                    .GetDbSet()
                    .Include(c => c.Friends)
                    .ThenInclude(f => f.Friend)
                    .Where(c => c.Id == id)
                    .Single();

                characterFriends.Friends.RemoveAll(f => f.Friend.Id == character.Id);
                _repository.Update(characterFriends);

                character.Friends.RemoveAll(e => e.Friend.Id == id);
            }

            return AssignFriends(character, dto);
        }


        #region async methods

        public async Task<CharacterDto> GetCharacterAsync(long characterId)
        {
            var character = await _repository
                .GetDbSet()
                .Include(c => c.Episodes)
                .ThenInclude(e => e.Episode)
                .Include(c => c.Planet)
                .Include(c => c.Friends)
                .ThenInclude(f => f.Friend)
                .Where(c => c.Id == characterId && !c.IsDeleted)
                .FirstOrDefaultAsync();
            
            CharacterDto characterDto = null;
            if (character != null)
            {
                characterDto = new CharacterDto
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
                    .Select(f => new CharacterDto
                    {
                        CharacterId = f.FriendId,
                        Name = f.Friend.GetName()
                    }).ToList()
                };
            }
            return characterDto;
        }

        public async Task<IList<CharacterDto>> GetCharactersListAsync()
        {
            var result = new List<CharacterDto>();
            var characters = await _repository
                .GetDbSet()
                .Include(c => c.Episodes)
                .ThenInclude(e => e.Episode)
                .Include(c => c.Planet)
                .Include(c => c.Friends)
                .ThenInclude(f => f.Friend)
                .Where(c => !c.IsDeleted)
                .ToListAsync();

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
                        .Select(f => new CharacterDto
                        {
                            CharacterId = f.FriendId,
                            Name = f.Friend.GetName()
                        }).ToList()
                };

                result.Add(characterDto);
            }

            return result;
        }

        public async Task DeleteCharacterAsync(long characterId)
        {
            var character = await _repository.GetByIdAsync(characterId);
            if (character != null)
            {
                character.IsDeleted = true;
                await _repository.UpdateAsync(character);
            }
        }

        public async Task DeleteCharacterCascadeAsync(long characterId)
        {
            var character = _dbContext.Characters
                .Include(c => c.Friends)
                .ThenInclude(f => f.Friend)
                .Where(c => c.Id == characterId)
                .FirstOrDefault();

            if (character != null)
            {
                foreach (var child in character.Friends.ToList())
                {
                    _dbContext.Remove(child);
                    await _dbContext.SaveChangesAsync();
                }

                await _repository.DeleteAsync(character);
            }
        }

        #endregion
    }
}
