using Microsoft.AspNetCore.Mvc;
using WebApplication1.Clients;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("controller")]
    public class MovieListController : ControllerBase
    {
        private readonly ILogger<MovieListController> _logger;
        public MovieListController (ILogger<MovieListController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public List<string> MovieTitles(int start_year, int end_year, double min_imdb, double max_imdb)
        {
            MovieClient client = new MovieClient();
            MovieList movieList = client.GetMovieListIMDBRating(start_year, end_year, min_imdb, max_imdb).Result;
            List<string> titles = new List<string>();
            List<double> rating = new List<double>();

            if (movieList.results != null)
            {
                foreach (var result in movieList.results)
                {
                    titles.Add(result.title);
                }
                //foreach (var result in movieList.results)
                //{
                //    rating.Add(result.imdbrating);
                //}
            }
            return titles;
        }
    }
}
