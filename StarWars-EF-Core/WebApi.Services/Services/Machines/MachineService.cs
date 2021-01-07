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

namespace WebApi.Services.Services.Machines
{
    public class MachineService : IMachineService
    {
        private readonly IRepository<Machine> _machineRepository;
        private readonly ICharacterService _characterService;
        private readonly IEpisodeService _episodeService;
        private readonly IPlanetService _planetService;
        public MachineService(
            IRepository<Machine> machineRepository,
            ICharacterService characterService,
            IEpisodeService episodeService,
            IPlanetService planetService
            )
        {
            _machineRepository = machineRepository;
            _characterService = characterService;
            _episodeService = episodeService;
            _planetService = planetService;
        }

        public long CreateMachine(MachineDto dto)
        {
            var machine = new Machine
            {
                Name = dto.Name
            };

            if (dto.PlanetId.HasValue)
            {
                _planetService.AssignPlanet(machine, dto.PlanetId.Value);
            }
            _episodeService.AssignEpisodes(machine, dto.EpisodeIds);
            _characterService.AssignFriends(machine, dto.FriendIds);


            var machineId = _machineRepository.Add(machine);

            return machineId;
        }

        public MachineDto GetMachine(long id)
        {
            var machine = _machineRepository.GetDbSet()
                .Include(h => h.Friends)
                .ThenInclude(f => f.Friend)
                .Include(h => h.Episodes)
                .ThenInclude(e => e.Episode)
                .Where(h => h.Id == id)
                .FirstOrDefault();

            if (machine == null)
            {
                throw new Exception("Machine doesn't exist");
            }

            var episodes = machine.Episodes
                .Select(e => new EpisodeDto
                {
                    EpisodeId = e.EpisodeId,
                    Name = e.Episode.EpisodeName
                }).ToList();

            var friends = machine.Friends
                .Select(c => new CharacterDto
                {
                    CharacterId = c.Friend.Id,
                    Name = c.Friend.GetName()
                }).ToList();

            var dto = new MachineDto
            {
                MachineId = machine.Id,
                Name = machine.GetName(),
                Episodes = episodes,
                Friends = friends
            };

            return dto;
        }

        public IList<MachineDto> GetMachinesList()
        {
            var list = _machineRepository.GetDbSet()
                .Include(h => h.Friends)
                .ThenInclude(f => f.Friend)
                .Include(h => h.Episodes)
                .ThenInclude(e => e.Episode)
                .ToList();

            var dtos = new List<MachineDto>();
            foreach (var machine in list)
            {
                var episodes = machine.Episodes
                    .Select(e => new EpisodeDto
                    {
                        EpisodeId = e.EpisodeId,
                        Name = e.Episode.EpisodeName
                    }).ToList();

                var friends = machine.Friends
                    .Select(c => new CharacterDto
                    {
                        CharacterId = c.Friend.Id,
                        Name = c.Friend.GetName()
                    }).ToList();

                dtos.Add(new MachineDto
                {
                    MachineId = machine.Id,
                    Name = machine.GetName(),
                    Episodes = episodes,
                    Friends = friends
                });
            }

            return dtos;
        }

        public long UpdateMachine(MachineDto dto)
        {
            var machine = _machineRepository.GetDbSet()
                .Include(h => h.Friends)
                .ThenInclude(f => f.Friend)
                .Include(h => h.Episodes)
                .ThenInclude(e => e.Episode)
                .Where(h => h.Id == dto.MachineId)
                .FirstOrDefault();

            if (machine == null)
            {
                throw new Exception("User doesn't exist");
            }

            machine.Name = dto.Name;
            if (dto.PlanetId.HasValue)
            {
                _planetService.AssignPlanet(machine, dto.PlanetId.Value);
            }
            _episodeService.UpdateEpisodes(machine, dto.EpisodeIds);
            _characterService.UpdateFriends(machine, dto.FriendIds);

            _machineRepository.Update(machine);

            return machine.Id;
        }

        public void DeleteMachine(long machineId)
        {
            _characterService.DeleteCharacter(machineId);
        }

        public void DeleteMachineCascade(long machineId)
        {
            _characterService.DeleteCharacterCascade(machineId);
        }
    }
}
