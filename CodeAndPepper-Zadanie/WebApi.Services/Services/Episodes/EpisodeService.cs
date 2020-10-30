using Microsoft.EntityFrameworkCore;
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

            var episodeId = _episodeRepository.Add(episode);

            var characters = new List<CharacterEpisode>();
            episode = _episodeRepository.GetDbSet().Where(e => e.Id == episodeId).FirstOrDefault();
            foreach (var id in episodeDto.CharacterIds)
            {
                var character = _characterRepository.GetDbSet().Where(c => c.Id == id).FirstOrDefault();
                if (character != null)
                {
                    characters.Add(new CharacterEpisode
                    {
                        Character = character,
                        CharacterId = id,
                        Episode = episode,
                        EpisodeId = episodeId
                    });
                }
            }

            episode.Characters.AddRange(characters);
            _episodeRepository.Update(episode);

            return episodeId;
        }

        public EpisodeDto GetEpisode(long EpisodeId)
        {
            var episode = _episodeRepository
                .GetDbSet()
                .Include(e => e.Characters)
                .Where(c => c.Id == EpisodeId && !c.IsDeleted)
                .FirstOrDefault();

            EpisodeDto dto = null;
            if (episode != null)
            {
                dto = new EpisodeDto
                {
                    EpisodeId = episode.Id,
                    Name = episode.EpisodeName,
                    CharacterIds = episode.Characters.Select(c => c.CharacterId).ToList()
                };
            }

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

        public long? UpdateEpisode(EpisodeDto episodeDto)
        {
            var episode = _episodeRepository.GetById(episodeDto.EpisodeId);
            if (episode == null)
            {
                return null;
            }

            episode.EpisodeName = episodeDto.Name;

            var characters = new List<CharacterEpisode>();
            foreach (var id in episodeDto.CharacterIds)
            {
                var character = _characterRepository.GetDbSet().Where(c => c.Id == id).FirstOrDefault();
                if (character != null)
                {
                    episode = _episodeRepository.GetDbSet().Where(e => e.Id == episodeDto.EpisodeId).FirstOrDefault();
                    characters.Add(new CharacterEpisode
                    {
                        Character = character,
                        CharacterId = id,
                        Episode = episode,
                        EpisodeId = episodeDto.EpisodeId
                    });
                }
            }

            episode.Characters.AddRange(characters);
            _episodeRepository.Update(episode);

            return episodeDto.EpisodeId;
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
                .Where(c => c.Id == episodeId && !c.IsDeleted)
                .FirstOrDefaultAsync();

            EpisodeDto EpisodeDto = null;
            if (episode != null)
            {
                EpisodeDto = new EpisodeDto
                {
                    EpisodeId = episode.Id,
                    Name = episode.EpisodeName
                };
            }
            return EpisodeDto;
        }

        public async Task<IList<EpisodeDto>> GetEpisodesListAsync()
        {
            var result = new List<EpisodeDto>();
            var episodes = await _episodeRepository
                .GetDbSet()
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            foreach (var episode in episodes)
            {
                var episodeDto = new EpisodeDto
                {
                    EpisodeId = episode.Id,
                    Name = episode.EpisodeName
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
    }
}
