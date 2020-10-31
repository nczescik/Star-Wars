using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebApi.DAL.Entities;
using WebApi.DAL.Extensions;
using WebApi.Services.Dto;
using WebAPI.DAL.Interfaces;

namespace WebApi.Services.Services.Humans
{
    public class HumanService : IHumanService
    {
        private readonly IRepository<Human> _humanRepository;
        private readonly IRepository<Character> _characterRepository;
        private readonly IRepository<Episode> _episodeRepository;
        public HumanService(
            IRepository<Human> humanRepository,
            IRepository<Character> characterRepository,
            IRepository<Episode> episodeRepository
            )
        {
            _humanRepository = humanRepository;
            _characterRepository = characterRepository;
            _episodeRepository = episodeRepository;
        }

        public long CreateHuman(HumanDto dto)
        {
            var human = new Human
            {

            };

            return _humanRepository.Add(human);
        }

        public HumanDto GetHuman(long id)
        {
            throw new System.NotImplementedException();
        }

        public IList<HumanDto> GetHumansList()
        {
            var list = _humanRepository.GetDbSet()
                .Include(h => h.Friends)
                .ThenInclude(f => f.Friend)
                .Include(h => h.Episodes)
                .ThenInclude(e => e.Episode)
                .ToList();

            var dtos = new List<HumanDto>();
            foreach (var human in list)
            {
                var episodes = human.Episodes
                    .Select(e => new EpisodeDto
                    {
                        EpisodeId = e.EpisodeId,
                        Name = e.Episode.EpisodeName
                    }).ToList();

                var friends = human.Friends
                    .Select(c => new CharacterDto
                    {
                        CharacterId = c.Friend.Id,
                        Name = c.Friend.GetName()
                    }).ToList();

                dtos.Add(new HumanDto
                {
                    HumanId = human.Id,
                    Firstname = human.Firstname,
                    Lastname = human.Lastname,
                    Episodes = episodes,
                    Friends = friends
                });
            }

            return dtos;
        }

        public long? UpdateHuman(HumanDto dto)
        {
            var human = _humanRepository.GetDbSet()
                .Include(h => h.Friends)
                .ThenInclude(f => f.Friend)
                .Include(h => h.Episodes)
                .ThenInclude(e => e.Episode)
                .FirstOrDefault();

            if (human == null)
            {
                return null;
            }

            human.Firstname = dto.Firstname;
            human.Lastname = dto.Lastname;

            human = UpdateEpisodes(human, dto);
            human = UpdateFriends(human, dto);

            _humanRepository.Update(human);

            return human.Id;
        }

        private Human UpdateEpisodes(Human human, HumanDto dto)
        {
            var episodes = new List<CharacterEpisode>();

            var oldEposodeIds = human.Episodes.Select(e => e.Episode.Id).ToList();
            foreach (var id in oldEposodeIds)
            {
                human.Episodes.RemoveAll(e => e.Episode.Id == id);
            }

            foreach (var episodeId in dto.EpisodeIds)
            {
                var episode = _episodeRepository.GetDbSet().Where(e => e.Id == episodeId).FirstOrDefault();
                if (episode != null)
                {
                    episodes.Add(new CharacterEpisode
                    {
                        Character = human,
                        Episode = episode,
                    });
                }
            }

            human.Episodes.AddRange(episodes);

            return human;
        }

        private Human UpdateFriends(Human human, HumanDto dto)
        {
            //Removing Human from his old friends Friends list first
            var oldFriendIds = human.Friends.Select(e => e.Friend.Id).ToList();
            foreach (var id in oldFriendIds)
            {
                var characterFriends = _characterRepository
                    .GetDbSet()
                    .Include(c => c.Friends)
                    .ThenInclude(f => f.Friend)
                    .Where(c => c.Id == id)
                    .Single();

                characterFriends.Friends.RemoveAll(f => f.Friend.Id == human.Id);
                _characterRepository.Update(characterFriends);

                human.Friends.RemoveAll(e => e.Friend.Id == id);
            }

            var friends = new List<Friendship>();
            foreach (var friendId in dto.FriendIds)
            {
                var character = _characterRepository
                    .GetDbSet()
                    .Include(c => c.Friends)
                    .ThenInclude(f => f.Friend)
                    .Where(e => e.Id == friendId)
                    .FirstOrDefault();

                if (character != null)
                {
                    //Adding Human to Friends list of his new friends
                    character.Friends.Add(new Friendship
                    {
                        Character = character,
                        CharacterId = character.Id,
                        Friend = human,
                        FriendId = human.Id
                    });
                    _characterRepository.Update(character);

                    friends.Add(new Friendship
                    {
                        Character = human,
                        CharacterId = human.Id,
                        Friend = character,
                        FriendId = character.Id
                    });
                }
            }

            human.Friends.AddRange(friends);

            return human;
        }

        public void DeleteHuman(long humanId)
        {
            var human = _humanRepository.GetById(humanId);
            human.IsDeleted = true;
            _humanRepository.Update(human);
        }

        public void DeleteHumanCascade(long humanId)
        {
            var human = _humanRepository.GetById(humanId);
            _humanRepository.Delete(human);
        }
    }
}
