using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.DAL.Entities;
using WebApi.Services.Dto;

namespace WebApi.Services.Services.Episodes
{
    public interface IEpisodeService
    {
        long CreateEpisode(EpisodeDto episodeDto);
        EpisodeDto GetEpisode(long EpisodeId);
        IList<EpisodeDto> GetEpisodesList();
        long UpdateEpisode(EpisodeDto episodeDto);
        void DeleteEpisode(long EpisodeId);
        void DeleteEpisodeCascade(long EpisodeId);
        void AssignEpisodes(Character character, IList<long> ids);
        void UpdateEpisodes(Character character, IList<long> ids);
        Task<EpisodeDto> GetEpisodeAsync(long EpisodeId);
        Task<IList<EpisodeDto>> GetEpisodesListAsync();
        Task DeleteEpisodeAsync(long EpisodeId);
        Task DeleteEpisodeCascadeAsync(long EpisodeId);
    }
}
