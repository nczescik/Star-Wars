using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.DAL.Entities;
using WebApi.DAL.Extensions;
using WebApi.Services.Dto;
using WebAPI.DAL.Interfaces;

namespace WebApi.Services.Services.Machines
{
    public class MachineService : IMachineService
    {
        private readonly IRepository<Machine> _machineRepository;
        private readonly IRepository<Character> _characterRepository;
        private readonly IRepository<Episode> _episodeRepository;
        public MachineService(
            IRepository<Machine> machineRepository,
            IRepository<Character> characterRepository,
            IRepository<Episode> episodeRepository
            )
        {
            _machineRepository = machineRepository;
            _characterRepository = characterRepository;
            _episodeRepository = episodeRepository;
        }

        public long CreateMachine(MachineDto dto)
        {
            var machine = new Machine
            {
                Name = dto.Name
            };
            machine.Episodes = AddEpisodes(machine, dto);
            machine.Friends = AddFriends(machine, dto);


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
                throw new Exception("machine doesn't exist");
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
            machine.Episodes = UpdateEpisodes(machine, dto);
            machine.Friends = UpdateFriends(machine, dto);

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



        private List<CharacterEpisode> AddEpisodes(Machine machine, MachineDto dto)
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
                    Character = machine,
                    Episode = episode,
                });
            }

            return episodes;
        }

        private List<CharacterEpisode> UpdateEpisodes(Machine machine, MachineDto dto)
        {
            //Removing old episodes first
            var episodes = new List<CharacterEpisode>();

            var oldEposodeIds = machine.Episodes.Select(e => e.Episode.Id).ToList();
            foreach (var id in oldEposodeIds)
            {
                machine.Episodes.RemoveAll(e => e.Episode.Id == id);
            }

            return AddEpisodes(machine, dto);
        }

        private List<Friendship> AddFriends(Machine machine, MachineDto dto)
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
                    Character = machine,
                    Friend = friend,
                });

                //Adding machine to his friend's Friends list
                var machineFriend = _characterRepository
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
                        Friend = machine
                    });
                }

            }

            return friends;
        }

        private List<Friendship> UpdateFriends(Machine machine, MachineDto dto)
        {
            if (dto.FriendIds.Contains(machine.Id))
            {
                throw new Exception("Cannot add updating machine to his own friends list");
            }

            //Removing machine from his old friends Friends list first
            var oldFriendIds = machine.Friends.Select(e => e.Friend.Id).ToList();
            foreach (var id in oldFriendIds)
            {
                var characterFriends = _characterRepository
                    .GetDbSet()
                    .Include(c => c.Friends)
                    .ThenInclude(f => f.Friend)
                    .Where(c => c.Id == id)
                    .Single();

                characterFriends.Friends.RemoveAll(f => f.Friend.Id == machine.Id);
                _characterRepository.Update(characterFriends);

                machine.Friends.RemoveAll(e => e.Friend.Id == id);
            }

            return AddFriends(machine, dto);
        }

    }
}
