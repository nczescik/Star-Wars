using System.Collections.Generic;

namespace WebApi.Models.Humans
{
    public class HumanViewModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Planet { get; set; }

        public List<string> Episodes { get; set; }
        public List<string> Friends { get; set; }
    }
}
