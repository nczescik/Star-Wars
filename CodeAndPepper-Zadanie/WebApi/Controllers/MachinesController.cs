using Microsoft.AspNetCore.Mvc;
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
                FriendIds = model.FriendIds,
                EpisodeIds = model.EpisodeIds
            };

            var id = _machineService.CreateMachine(dto);

            return Ok(new { machineId = id });
        }

        [HttpGet("Getmachine/{userId}")]
        public IActionResult Getmachine(long userId)
        {
            var dto = _machineService.GetMachine(userId);
            var model = new MachineViewModel
            {
                Name = dto.Name,
                Episodes = dto.Episodes.Select(e => e.Name).ToList(),
                Friends = dto.Friends.Select(f => f.Name).ToList()
            };

            return Ok(new { machine = model });
        }

        [HttpGet("Getmachines")]
        public IActionResult Getmachines()
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
                return BadRequest(new { Message = "Incorrect value of machine Id" });
            }

            var dto = new MachineDto
            {
                MachineId = model.MachineId.Value,
                Name = model.Name,
                FriendIds = model.FriendIds,
                EpisodeIds = model.EpisodeIds
            };

            var id = _machineService.UpdateMachine(dto);

            return Ok(new { machineId = id });
        }
    }
}
