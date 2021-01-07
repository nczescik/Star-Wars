using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DAL.Entities;
using WebApi.Services.Dto;
using WebAPI.DAL.Interfaces;

namespace WebApi.Services.Services.Episodes
{
    public class EpisodeService : IEpisodeService
    {
        private readonly IRepository<Episode> _episodeRepository;
        private readonly IRepository<Character> _characterRepository;
        public EpisodeService(
            IRepository<Episode> episodeRepository,
            IRepository<Character> characterRepository)
        {
            _episodeRepository = episodeRepository;
            _characterRepository = characterRepository;
        }

        public long CreateEpisode(EpisodeDto episodeDto)
        {
            var episode = new Episode
            {
                EpisodeName = episodeDto.Name
            };
            AssignCharacters(episode, episodeDto);

            var episodeId = _episodeRepository.Add(episode);
            return episodeId;
        }

        public EpisodeDto GetEpisode(long EpisodeId)
        {
            var episode = _episodeRepository
                .GetDbSet()
                .Include(e => e.Characters)
                .Where(c => c.Id == EpisodeId && !c.IsDeleted)
                .FirstOrDefault();

            if (episode == null)
            {
                throw new Exception("Episode doesn't exist");
            }

            var dto = new EpisodeDto
            {
                EpisodeId = episode.Id,
                Name = episode.EpisodeName,
                CharacterIds = episode.Characters.Select(c => c.CharacterId).ToList()
            };

            return dto;
        }

        public IList<EpisodeDto> GetEpisodesList()
        {
            var result = new List<EpisodeDto>();
            var episodes = _episodeRepository
                .GetDbSet()
                .Include(e => e.Characters)
                .Where(c => !c.IsDeleted)
                .ToList();

            foreach (var episode in episodes)
            {
                var episodeDto = new EpisodeDto
                {
                    EpisodeId = episode.Id,
                    Name = episode.EpisodeName,
                    CharacterIds = episode.Characters.Select(c => c.CharacterId).ToList()
                };

                result.Add(episodeDto);
            }

            return result;
        }

        public long UpdateEpisode(EpisodeDto episodeDto)
        {
            var episode = _episodeRepository.GetById(episodeDto.EpisodeId);
            if (episode == null)
            {
                throw new Exception("Episode doesn't exist");
            }

            episode.EpisodeName = episodeDto.Name;
            UpdateCharacters(episode, episodeDto);

            var episodeId = _episodeRepository.Update(episode);

            return episodeId;
        }

        public void DeleteEpisode(long episodeId)
        {
            var episode = _episodeRepository.GetById(episodeId);
            if (episode != null)
            {
                episode.IsDeleted = true;
                _episodeRepository.Update(episode);
            }
        }

        public void DeleteEpisodeCascade(long episodeId)
        {
            var episode = _episodeRepository.GetById(episodeId);
            if (episode != null)
            {
                _episodeRepository.Delete(episode);
            }
        }


        #region async methods

        public async Task<EpisodeDto> GetEpisodeAsync(long episodeId)
        {
            var episode = await _episodeRepository
                .GetDbSet()
                .Include(e => e.Characters)
                .Where(c => c.Id == episodeId && !c.IsDeleted)
                .FirstOrDefaultAsync();

            if (episode == null)
            {
                throw new Exception("Episode doesn't exist");
            }

            var episodeDto = new EpisodeDto
            {
                EpisodeId = episode.Id,
                Name = episode.EpisodeName,
                CharacterIds = episode.Characters.Select(c => c.CharacterId).ToList()
            };

            return episodeDto;
        }

        public async Task<IList<EpisodeDto>> GetEpisodesListAsync()
        {
            var result = new List<EpisodeDto>();
            var episodes = await _episodeRepository
                .GetDbSet()
                .Include(e => e.Characters)
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            foreach (var episode in episodes)
            {
                var episodeDto = new EpisodeDto
                {
                    EpisodeId = episode.Id,
                    Name = episode.EpisodeName,
                    CharacterIds = episode.Characters.Select(c => c.CharacterId).ToList()
                };

                result.Add(episodeDto);
            }

            return result;
        }

        public async Task DeleteEpisodeAsync(long episodeId)
        {
            var episode = await _episodeRepository.GetByIdAsync(episodeId);
            if (episode != null)
            {
                episode.IsDeleted = true;
                await _episodeRepository.UpdateAsync(episode);
            }
        }

        public async Task DeleteEpisodeCascadeAsync(long episodeId)
        {
            var episode = await _episodeRepository.GetByIdAsync(episodeId);
            if (episode != null)
            {
                await _episodeRepository.DeleteAsync(episode);
            }
        }

        #endregion



        public void AssignEpisodes(Character character, IList<long> ids)
        {
            var episodes = new List<CharacterEpisode>();
            foreach (var episodeId in ids)
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

            character.Episodes = episodes;
        }

        public void UpdateEpisodes(Character character, IList<long> ids)
        {
            //Removing old episodes first
            var episodes = new List<CharacterEpisode>();

            var oldEposodeIds = character.Episodes.Select(e => e.Episode.Id).ToList();
            foreach (var id in oldEposodeIds)
            {
                character.Episodes.RemoveAll(e => e.Episode.Id == id);
            }

            AssignEpisodes(character, ids);
        }


        private void AssignCharacters(Episode episode, EpisodeDto episodeDto)
        {
            var characters = new List<CharacterEpisode>();
            foreach (var id in episodeDto.CharacterIds)
            {
                var character = _characterRepository.GetDbSet().Where(c => c.Id == id).FirstOrDefault();
                if (character == null)
                {
                    throw new Exception("Cannot assing character which doesn't exist to episode");
                }

                characters.Add(new CharacterEpisode
                {
                    Character = character,
                    Episode = episode
                });
            }

            episode.Characters = characters;
        }

        private void UpdateCharacters(Episode episode, EpisodeDto dto)
        {
            //Removing old characters first
            var characters = new List<CharacterEpisode>();

            var oldCharacterIds = episode.Characters.Select(e => e.Character.Id).ToList();
            foreach (var id in oldCharacterIds)
            {
                episode.Characters.RemoveAll(e => e.Character.Id == id);
            }

            AssignCharacters(episode, dto);
        }

    }
}
