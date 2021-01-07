using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebApi.Helpers;
using WebApi.Models.Episodes;
using WebApi.Services.Dto;
using WebApi.Services.Services.Episodes;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EpisodeController : ControllerBase
    {
        private readonly IEpisodeService _episodeService;
        public EpisodeController(
            IEpisodeService episodeService)
        {
            _episodeService = episodeService;
        }

        [HttpPost("Create")]
        public IActionResult Create(EpisodeModel model)
        {
            var dto = new EpisodeDto
            {
                Name = model.Name,
                CharacterIds = model.CharacterIds
            };

            var id = _episodeService.CreateEpisode(dto);

            return Ok(new { EpisodeId = id });
        }

        [HttpGet("GetEpisode")]
        public IActionResult GetEpisode(long episodeId)
        {
            var episode = _episodeService.GetEpisode(episodeId);
            if (episode == null)
            {
                return Ok(new { Message = "Episode doesn't exists" });
            }

            var episodeModel = new EpisodeModel
            {
                Name = episode.Name,
                CharacterIds = episode.CharacterIds
            };

            var json = JsonHelper<EpisodeModel>.JsonConverter(episodeModel, "episode");

            return Content(json, "application/json");
        }

        [HttpGet("GetEpisodes")]
        public IActionResult GetEpisodes()
        {
            var episodeList = new List<EpisodeModel>();
            var episodes = _episodeService.GetEpisodesList();

            foreach (var episode in episodes)
            {
                episodeList.Add(new EpisodeModel
                {
                    Name = episode.Name,
                    CharacterIds = episode.CharacterIds
                });
            }
            var json = JsonHelper<List<EpisodeModel>>.JsonConverter(episodeList, "episodes");

            return Content(json, "application/json");
        }


        [HttpPut("Update")]
        public IActionResult UpdateEpisode(EpisodeModel episodeModel)
        {
            if (!episodeModel.EpisodeId.HasValue || episodeModel.EpisodeId.Value == 0)
            {
                throw new Exception("Incorrect value of episode Id");
            }

            var episodeDto = new EpisodeDto
            {
                EpisodeId = episodeModel.EpisodeId.Value,
                Name = episodeModel.Name,
                CharacterIds = episodeModel.CharacterIds
            };

            var id = _episodeService.UpdateEpisode(episodeDto);

            return Ok(new { EpisodeId = id });
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteEpisode(long episodeId)
        {
            _episodeService.DeleteEpisode(episodeId);
            return Ok();
        }

        [HttpDelete("DeleteCascade")]
        public IActionResult DeleteCharacterCascade(long episodeId)
        {
            _episodeService.DeleteEpisodeCascade(episodeId);
            return Ok();
        }
    }
}
