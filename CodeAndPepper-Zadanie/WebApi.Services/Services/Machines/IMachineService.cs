using System.Collections.Generic;
using WebApi.Services.Dto;

namespace WebApi.Services.Services.Machines
{
    public interface IMachineService
    {
        long CreateMachine(MachineDto dto);
        MachineDto GetMachine(long id);
        IList<MachineDto> GetMachinesList();
        long UpdateMachine(MachineDto dto);
        void DeleteMachine(long MachineId);
        void DeleteMachineCascade(long MachineId);
    }
}
