using System.Collections.Generic;

namespace PokemonMVC.Models
{
    public class Pokemon
    {
        public string Name { get; set; }
        public List<string> Moves { get; set; }
        public List<string> Abilities { get; set; }
    }
}
