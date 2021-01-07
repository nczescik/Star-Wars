using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Helpers;
using WebApi.Models.Machines;
using WebApi.Services.Dto;
using WebApi.Services.Services.Machines;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MachinesController : ControllerBase
    {
        private readonly IMachineService _machineService;
        public MachinesController(
            IMachineService machineService)
        {
            _machineService = machineService;
        }

        [HttpPost("Create")]
        public IActionResult Create(MachineModel model)
        {
            var dto = new MachineDto
            {
                Name = model.Name,
                PlanetId = model.PlanetId,
                FriendIds = model.FriendIds,
                EpisodeIds = model.EpisodeIds
            };

            var id = _machineService.CreateMachine(dto);

            return Ok(new { machineId = id });
        }

        [HttpGet("GetMachine/{machineId}")]
        public IActionResult GetMachine(long machineId)
        {
            var dto = _machineService.GetMachine(machineId);
            var model = new MachineViewModel
            {
                Name = dto.Name,
                Episodes = dto.Episodes.Select(e => e.Name).ToList(),
                Friends = dto.Friends.Select(f => f.Name).ToList()
            };

            return Ok(new { Machine = model });
        }

        [HttpGet("GetMachines")]
        public IActionResult GetMachines()
        {
            var result = new List<MachineViewModel>();
            var machines = _machineService.GetMachinesList();

            foreach (var machine in machines)
            {
                var model = new MachineViewModel
                {
                    Name = machine.Name,
                    Episodes = machine.Episodes.Select(e => e.Name).ToList(),
                    Planet = machine.Planet?.Name,
                    Friends = machine.Friends.Select(f => f.Name).ToList()
                };

                result.Add(model);
            }

            var json = JsonHelper<List<MachineViewModel>>.JsonConverter(result, "machines");

            return Content(json, "application/json");
        }

        [HttpPut("Update")]
        public IActionResult Update(MachineModel model)
        {
            if (!model.MachineId.HasValue || model.MachineId.Value == 0)
            {
                throw new Exception("Incorrect value of machine Id");
            }

            var dto = new MachineDto
            {
                MachineId = model.MachineId.Value,
                Name = model.Name,
                PlanetId = model.PlanetId,
                FriendIds = model.FriendIds,
                EpisodeIds = model.EpisodeIds
            };

            var id = _machineService.UpdateMachine(dto);

            return Ok(new { machineId = id });
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteMachine(long machineId)
        {
            _machineService.DeleteMachine(machineId);
            return Ok();
        }

        [HttpDelete("DeleteCascade")]
        public IActionResult DeleteMachineCascade(long machineId)
        {
            _machineService.DeleteMachineCascade(machineId);
            return Ok();
        }
    }
}
