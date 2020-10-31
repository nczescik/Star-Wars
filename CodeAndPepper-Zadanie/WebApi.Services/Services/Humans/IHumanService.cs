using System.Collections;
using System.Collections.Generic;
using WebApi.DAL.Entities;
using WebApi.Services.Dto;

namespace WebApi.Services.Services.Humans
{
    public interface IHumanService
    {
        long CreateHuman(HumanDto dto);
        HumanDto GetHuman(long id);
        IList<HumanDto> GetHumansList();
        long? UpdateHuman(HumanDto dto);
        void DeleteHuman(long humanId);
        void DeleteHumanCascade(long humanId);
    }
}
