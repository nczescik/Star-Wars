using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebApi.Helpers;
using WebApi.Models.Humans;
using WebApi.Services.Dto;
using WebApi.Services.Services.Humans;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HumansController : ControllerBase
    {
        private readonly IHumanService _humanService;
        public HumansController(
            IHumanService humanService)
        {
            _humanService = humanService;
        }

        [HttpPost("Create")]
        public IActionResult Create(HumanModel model)
        {
            var dto = new HumanDto
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                FriendIds = model.FriendIds,
                EpisodeIds = model.EpisodeIds
            };

            var id = _humanService.CreateHuman(dto);

            return Ok(new { HumanId = id });
        }

        [HttpGet("GetHuman/{userId}")]
        public IActionResult GetHuman(long userId)
        {
            var dto = _humanService.GetHuman(userId);
            var model = new HumanViewModel
            {
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                Episodes = dto.Episodes.Select(e => e.Name).ToList(),
                Friends = dto.Friends.Select(f => f.Name).ToList()
            };

            return Ok(new { Human = model });
        }

        [HttpGet("GetHumans")]
        public IActionResult GetHumans()
        {
            var result = new List<HumanViewModel>();
            var humans = _humanService.GetHumansList();

            foreach (var human in humans)
            {
                var model = new HumanViewModel
                {
                    Firstname = human.Firstname,
                    Lastname = human.Lastname,
                    Episodes = human.Episodes.Select(e => e.Name).ToList(),
                    Planet = human.Planet?.Name,
                    Friends = human.Friends.Select(f => f.Name).ToList()
                };

                result.Add(model);
            }

            var json = JsonHelper<List<HumanViewModel>>.JsonConverter(result, "humans");

            return Content(json, "application/json");
        }

        [HttpPut("Update")]
        public IActionResult Update(HumanModel model)
        {
            if (!model.HumanId.HasValue || model.HumanId.Value == 0)
            {
                return BadRequest(new { Message = "Incorrect value of human Id" });
            }

            var dto = new HumanDto
            {
                HumanId = model.HumanId.Value,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                FriendIds = model.FriendIds,
                EpisodeIds = model.EpisodeIds
            };

            var id = _humanService.UpdateHuman(dto);

            return Ok(new { HumanId = id });
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteHuman(long humanId)
        {
            _humanService.DeleteHuman(humanId);
            return Ok();
        }

        [HttpDelete("DeleteCascade")]
        public IActionResult DeleteHumanCascade(long humanId)
        {
            _humanService.DeleteHumanCascade(humanId);
            return Ok();
        }
    }
}
