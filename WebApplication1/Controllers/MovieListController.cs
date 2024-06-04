using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebApplication1.Clients;
using WebApplication1.Models;

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
        public MovieList MovieTitles(int start_year, int end_year, double min_imdb, double max_imdb)
        {
            MovieClient client = new MovieClient();
            var movieList = client.GetMovieListIMDBRating(start_year, end_year, min_imdb, max_imdb).Result;
            _cache.Set("MovieList", movieList);
            return movieList;
        }

        [HttpGet("GetMovieByTitle")]
        public Result GetMovieByTitle(string title)
        {
            var movieList = _cache.Get<MovieList>("MovieList");
            if (movieList?.results != null)
            {
                foreach (var result in movieList.results)
                {
                    if (result.title == title)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }
}
