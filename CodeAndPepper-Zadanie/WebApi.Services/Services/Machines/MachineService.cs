using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.DAL.Entities;
using WebApi.DAL.Extensions;
using WebApi.Services.Dto;
using WebApi.Services.Services.Characters;
using WebAPI.DAL.Interfaces;

namespace WebApi.Services.Services.Machines
{
    public class MachineService : IMachineService
    {
        private readonly IRepository<Machine> _machineRepository;
        private readonly ICharacterService _characterService;
        public MachineService(
            IRepository<Machine> machineRepository,
            ICharacterService characterService
            )
        {
            _machineRepository = machineRepository;
            _characterService = characterService;
        }

        public long CreateMachine(MachineDto dto)
        {
            var machine = new Machine
            {
                Name = dto.Name
            };
            machine.Episodes = _characterService.AssignEpisodes(machine, dto);
            machine.Friends = _characterService.AssignFriends(machine, dto);


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
            machine.Episodes = _characterService.UpdateEpisodes(machine, dto);
            machine.Friends = _characterService.UpdateFriends(machine, dto);

            _machineRepository.Update(machine);

            return machine.Id;
        }

        public void DeleteMachine(long machineId)
        {
            var machine = _machineRepository.GetById(machineId);
            machine.IsDeleted = true;
            _machineRepository.Update(machine);
        }

        public void DeleteMachineCascade(long machineId)
        {
            var machine = _machineRepository.GetById(machineId);
            _machineRepository.Delete(machine);
        }
    }
}
