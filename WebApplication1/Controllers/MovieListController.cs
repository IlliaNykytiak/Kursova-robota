using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
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

        [HttpGet("GetAllToWatch")]
        public async Task<IActionResult> GetAllToWatch(int chat_ID)
        {
            Database db = new Database();
            try
            {
                return Ok(await db.GetAllToWatch(chat_ID));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error key");
                return StatusCode(500, "An error occurred while searching the key");
            }

        }

        [HttpPost("AddMovie")]
        public async Task<IActionResult> AddMovie(int ID, [FromBody] MovieByName movie)
        {
            Database db = new Database();
            if (movie == null)
            {
                return BadRequest("Movie is null");
            }

            try
            {
                await db.ToWatchList(ID, movie.title, movie.type, movie.genre[0], movie.imageurl[0], movie.released, movie.imdbid, movie.imdbrating, movie.synopsis);
                return Ok("Movie added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding movie");
                return StatusCode(500, "An error occurred while adding the movie");
            }
        }

        [HttpPut("UpdateMovie/{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieByName movie)
        {
            Database db = new Database();
            if (movie == null)
            {
                return BadRequest("Movie is null");
            }

            try
            {
                int key = await db.GetKeyByTitle(movie.title, id);
                await db.UpdateToWatch(id, movie, key);
                return Ok("Movie updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie");
                return StatusCode(500, "An error occurred while updating the movie");
            }
        }

        [HttpDelete("DeleteMovie/{id}")]
        public async Task<IActionResult> DeleteMovie(int id, string title)
        {
            Database db = new Database();
            if (title == null)
            {
                return BadRequest("Title is null");
            }
            try
            {
                int key = await db.GetKeyByTitle(title, id);
                Console.WriteLine(key);
                await db.DeleteFromToWatchList(key);
                return Ok("Movie deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie");
                return StatusCode(500, "An error occurred while deleting the movie");
            }
        }
        [HttpDelete("ClearToWatchList/{id}")]
        public async Task<IActionResult> ClearToWatchList(int id)
        {
            Database db = new Database();
            try
            {
                await db.ClearToWatch(id);
                return Ok("Movie deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie");
                return StatusCode(500, "An error occurred while deleting the movie");
            }
        }

    }
}
