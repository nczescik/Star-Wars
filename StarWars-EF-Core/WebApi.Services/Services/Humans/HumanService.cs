using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.DAL.Entities;
using WebApi.DAL.Extensions;
using WebApi.Services.Dto;
using WebApi.Services.Services.Characters;
using WebApi.Services.Services.Episodes;
using WebApi.Services.Services.Planets;
using WebAPI.DAL.Interfaces;

namespace WebApi.Services.Services.Humans
{
    public class HumanService : IHumanService
    {
        private readonly IRepository<Human> _humanRepository;
        private readonly ICharacterService _characterService;
        private readonly IEpisodeService _episodeService;
        private readonly IPlanetService _planetService;

        public HumanService(
            IRepository<Human> humanRepository,
            ICharacterService characterService,
            IEpisodeService episodeService,
            IPlanetService planetService
            )
        {
            _humanRepository = humanRepository;
            _characterService = characterService;
            _episodeService = episodeService;
            _planetService = planetService;
        }

        public long CreateHuman(HumanDto dto)
        {
            var human = new Human
            {
                Firstname = dto.Firstname,
                Lastname = dto.Lastname
            };

            if (dto.PlanetId.HasValue)
            {
                _planetService.AssignPlanet(human, dto.PlanetId.Value);
            }
            _episodeService.AssignEpisodes(human, dto.EpisodeIds);
            _characterService.AssignFriends(human, dto.FriendIds);


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
            if (dto.PlanetId.HasValue)
            {
                _planetService.AssignPlanet(human, dto.PlanetId.Value);
            }
            _episodeService.UpdateEpisodes(human, dto.EpisodeIds);
            _characterService.UpdateFriends(human, dto.FriendIds);

            _humanRepository.Update(human);

            return human.Id;
        }

        public void DeleteHuman(long humanId)
        {
            _characterService.DeleteCharacter(humanId);
        }

        public void DeleteHumanCascade(long humanId)
        {
            _characterService.DeleteCharacterCascade(humanId);
        }
    }
}
