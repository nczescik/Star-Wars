using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.DAL.Entities
{
    public class Planet : Entity
    {
        public string Name { get; set; }
    }
}
