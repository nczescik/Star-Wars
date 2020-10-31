using System.Collections.Generic;

namespace WebApi.Models.Machines
{
    public class MachineViewModel
    {
        public string Name { get; set; }

        public string Planet { get; set; }

        public List<string> Episodes { get; set; }
        public List<string> Friends { get; set; }
    }
}
