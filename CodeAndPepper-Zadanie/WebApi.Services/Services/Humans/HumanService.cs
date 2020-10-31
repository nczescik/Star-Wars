using Microsoft.EntityFrameworkCore;
using System;
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
                Firstname = dto.Firstname,
                Lastname = dto.Lastname
            };
            human.Episodes = AddEpisodes(human, dto);
            human.Friends = AddFriends(human, dto);


            var humanId = _humanRepository.Add(human);

            return humanId;
        }

        public HumanDto GetHuman(long id)
        {
            var human = _humanRepository.GetDbSet()
                .Include(h => h.Friends)
                .ThenInclude(f => f.Friend)
                .Include(h => h.Episodes)
                .ThenInclude(e => e.Episode)
                .Where(h => h.Id == id)
                .FirstOrDefault();

            if (human == null)
            {
                throw new Exception("Human doesn't exist");
            }

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

            var dto = new HumanDto
            {
                HumanId = human.Id,
                Firstname = human.Firstname,
                Lastname = human.Lastname,
                Episodes = episodes,
                Friends = friends
            };

            return dto;
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

        public long UpdateHuman(HumanDto dto)
        {
            var human = _humanRepository.GetDbSet()
                .Include(h => h.Friends)
                .ThenInclude(f => f.Friend)
                .Include(h => h.Episodes)
                .ThenInclude(e => e.Episode)
                .Where(h => h.Id == dto.HumanId)
                .FirstOrDefault();

            if (human == null)
            {
                throw new Exception("User doesn't exist");
            }

            human.Firstname = dto.Firstname;
            human.Lastname = dto.Lastname;
            human.Episodes = UpdateEpisodes(human, dto);
            human.Friends = UpdateFriends(human, dto);

            _humanRepository.Update(human);

            return human.Id;
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



        private List<CharacterEpisode> AddEpisodes(Human human, HumanDto dto)
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
                    Character = human,
                    Episode = episode,
                });
            }

            return episodes;
        }

        private List<CharacterEpisode> UpdateEpisodes(Human human, HumanDto dto)
        {
            var episodes = new List<CharacterEpisode>();

            var oldEposodeIds = human.Episodes.Select(e => e.Episode.Id).ToList();
            foreach (var id in oldEposodeIds)
            {
                human.Episodes.RemoveAll(e => e.Episode.Id == id);
            }

            return AddEpisodes(human, dto);
        }

        private List<Friendship> AddFriends(Human human, HumanDto dto)
        {
            var friends = new List<Friendship>();
            foreach (var friendId in dto.FriendIds)
            {
                var friend = _characterRepository
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
                    Character = human,
                    Friend = friend,
                });

                //Adding Human to his friend's Friends list
                var humanFriend = _characterRepository
                    .GetDbSet()
                    .Include(c => c.Friends)
                    .ThenInclude(f => f.Friend)
                    .Where(e => e.Id == friendId)
                    .FirstOrDefault();

                if (humanFriend != null)
                {
                    humanFriend.Friends.Add(new Friendship
                    {
                        Character = friend,
                        Friend = human
                    });
                }

            }

            return friends;
        }

        private List<Friendship> UpdateFriends(Human human, HumanDto dto)
        {
            if (dto.FriendIds.Contains(human.Id))
            {
                throw new Exception("Cannot add updating human to his own friends list");
            }

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

            return AddFriends(human, dto);
        }

    }
}
