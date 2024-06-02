using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebApplication1.Clients;
using WebApplication1.Models;
using WebApplication1.Telegram;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("controller")]
    public class MovieListController : ControllerBase
    {
        private readonly ILogger<MovieListController> _logger;
        private readonly IMemoryCache _cache;

        public MovieListController(ILogger<MovieListController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet("GetMovieListIMDBRating")]
        public List<string> MovieTitles(int start_year, int end_year, double min_imdb, double max_imdb)
        {
            MovieClient client = new MovieClient();
            var movieList = client.GetMovieListIMDBRating(start_year, end_year, min_imdb, max_imdb).Result;
            _cache.Set("MovieList", movieList);
            List<string> titles = new List<string>();
            if (movieList.results != null)
            {
                foreach (var result in movieList.results)
                {
                    titles.Add(result.title);
                }
            }
            return titles;
        }

        [HttpGet("GetMovieByTitle")]
        public string GetMovieByTitle(string title)
        {
            var movieList = _cache.Get<MovieList>("MovieList");
            foreach (var result in movieList.results)
            {
                if (result.title == title)
                {
                    return "1";
                }
                return "Movie not found";
            }
            return "Movie not found";
        }
    }
}
