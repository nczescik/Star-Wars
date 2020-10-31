using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.DAL.Entities;
using WebApi.DAL.Extensions;
using WebApi.Services.Dto;
using WebApi.Services.Services.Characters;
using WebAPI.DAL.Interfaces;

namespace WebApi.Services.Services.Humans
{
    public class HumanService : IHumanService
    {
        private readonly IRepository<Human> _humanRepository;
        private readonly ICharacterService _characterService;
        public HumanService(
            IRepository<Human> humanRepository,
            ICharacterService characterService
            )
        {
            _humanRepository = humanRepository;
            _characterService = characterService;
        }

        public long CreateHuman(HumanDto dto)
        {
            var human = new Human
            {
                Firstname = dto.Firstname,
                Lastname = dto.Lastname
            };
            human.Episodes = _characterService.AssignEpisodes(human, dto);
            human.Friends = _characterService.AssignFriends(human, dto);


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
            human.Episodes = _characterService.UpdateEpisodes(human, dto);
            human.Friends = _characterService.UpdateFriends(human, dto);

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
