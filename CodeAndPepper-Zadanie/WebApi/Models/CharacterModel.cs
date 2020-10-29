using System.Collections.Generic;

namespace WebApi.Models
{
    public class CharacterModel
    {
        public string Name { get; set; }
        public string Planet { get; set; }
        public IList<string> Episodes { get; set; }
        public IList<string> Friends { get; set; }
    }
}
