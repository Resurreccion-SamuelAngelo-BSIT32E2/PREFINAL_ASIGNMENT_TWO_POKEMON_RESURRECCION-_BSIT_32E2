using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PokemonMVC.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PokemonMVC.Controllers
{
    public class PokemonController : Controller
    {
        private readonly HttpClient _httpClient;

        public PokemonController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index(int offset = 0, int limit = 20)
        {
            var pokemons = new List<Pokemon>();

            try
            {
                var response = await _httpClient.GetStringAsync($"https://pokeapi.co/api/v2/pokemon?offset={offset}&limit={limit}");
                Debug.WriteLine("API Response: " + response); // Log the response

                if (!string.IsNullOrEmpty(response))
                {
                    var json = JObject.Parse(response);
                    var results = json["results"];
                    if (results != null)
                    {
                        foreach (var result in results)
                        {
                            var pokemonUrl = result["url"]?.ToString();
                            if (!string.IsNullOrEmpty(pokemonUrl))
                            {
                                var pokemonResponse = await _httpClient.GetStringAsync(pokemonUrl);
                                Debug.WriteLine("Pokemon Response: " + pokemonResponse); // Log the individual pokemon response

                                if (!string.IsNullOrEmpty(pokemonResponse))
                                {
                                    var pokemonJson = JObject.Parse(pokemonResponse);
                                    var moves = new List<string>();
                                    foreach (var move in pokemonJson["moves"])
                                    {
                                        moves.Add(move["move"]["name"]?.ToString());
                                    }

                                    var abilities = new List<string>();
                                    foreach (var ability in pokemonJson["abilities"])
                                    {
                                        abilities.Add(ability["ability"]["name"]?.ToString());
                                    }

                                    var pokemon = new Pokemon
                                    {
                                        Name = pokemonJson["name"]?.ToString(),
                                        Moves = moves,
                                        Abilities = abilities
                                    };

                                    pokemons.Add(pokemon);
                                }
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Request error: " + e.Message); // Log any request errors
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message); // Log any other errors
            }

            ViewBag.Offset = offset;
            ViewBag.Limit = limit;

            return View(pokemons);
        }

        public async Task<IActionResult> Details(string name)
        {
            var response = await _httpClient.GetStringAsync($"https://pokeapi.co/api/v2/pokemon/{name}");
            if (string.IsNullOrEmpty(response))
            {
                return NotFound();
            }

            var json = JObject.Parse(response);
            var moves = new List<string>();
            foreach (var move in json["moves"])
            {
                moves.Add(move["move"]["name"]?.ToString());
            }

            var abilities = new List<string>();
            foreach (var ability in json["abilities"])
            {
                abilities.Add(ability["ability"]["name"]?.ToString());
            }

            var pokemon = new Pokemon
            {
                Name = json["name"]?.ToString(),
                Moves = moves,
                Abilities = abilities
            };

            return View(pokemon);
        }
    }
}
