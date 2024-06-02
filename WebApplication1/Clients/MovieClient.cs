using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.Clients
{
    public class MovieClient
    {
        private static string _address;
        private static string _apikey;
        private static string _apihost;
        public MovieClient()
        {
            _address = Constants.Address;
            _apikey = Constants.ApiKey;
            _apihost = Constants.ApiHost;
        }
        public async Task<MovieList> GetMovieListIMDBRating(int start_year, int end_year, double min_imdb, double max_imdb)
        {
            Database db = new Database();
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_address + "/advancedsearch?start_year=" + start_year + "&end_year=" + end_year + "&min_imdb=" + min_imdb + "&max_imdb=" + max_imdb + "&genre=adventure&language=english&type=movie&sort=oldest&page=1"),
                Headers =
                {
                    { "X-RapidAPI-Key", _apikey },
                    { "X-RapidAPI-Host", _apihost },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<MovieList>(body);
                //db.InsertMovieListAsync(result);
                return result;
            }
        }
    }
}
