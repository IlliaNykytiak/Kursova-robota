using Npgsql;
using WebApplication1.Models;
namespace WebApplication1
{
    public class Database
    {
        NpgsqlConnection connection = new NpgsqlConnection(Constants.Connect);
        public async Task ToWatchList(int ID, string title, string type, string genre, string imageurl, int released, string imdbid, double imdbrating, string synopsis)
        {
            Random random = new Random();
            int i = random.Next(-999999999, 999999999);
            var sql = "insert into public.\"ToWatchList\" (\"chat_ID\",\"title\", \"type\"" +
                ", \"genre\", \"imageurl\", \"released\", \"imdbid\", \"imdbrating\", \"synopsis\", \"key\")" +
                $"values (@chat_ID, @title, @type, @genre, @imageurl, @released, @imdbid, @imdbrating, @synopsis, @key)";
            await connection.OpenAsync();
            NpgsqlCommand comm = new NpgsqlCommand(sql, connection);
            comm.Parameters.AddWithValue("chat_ID", ID);
            comm.Parameters.AddWithValue("title", title);
            comm.Parameters.AddWithValue("type", type);
            comm.Parameters.AddWithValue("genre", genre);
            comm.Parameters.AddWithValue("imageurl", imageurl);
            comm.Parameters.AddWithValue("released", released);
            comm.Parameters.AddWithValue("imdbid", imdbid);
            comm.Parameters.AddWithValue("imdbrating", imdbrating);
            comm.Parameters.AddWithValue("synopsis", synopsis);
            comm.Parameters.AddWithValue("key", i);
            await comm.ExecuteNonQueryAsync();
            await connection.CloseAsync();
            return;
        }
        public async Task UpdateToWatch(int ID, MovieByName movie, int i)
        {
            var sql = "UPDATE public.\"ToWatchList\" SET \"chat_ID\" = @chat_ID, \"title\" = @title, \"type\" = @type, \"genre\" = @genre, \"imageurl\" = @imageurl, \"released\" = @released, \"imdbid\" = @imdbid, \"imdbrating\" = @imdbrating, \"synopsis\" = @synopsis WHERE \"key\" = @key";
            await connection.OpenAsync();
            NpgsqlCommand comm = new NpgsqlCommand(sql, connection);
            comm.Parameters.AddWithValue("chat_ID", ID);
            comm.Parameters.AddWithValue("title", movie.title);
            comm.Parameters.AddWithValue("type", movie.type);
            comm.Parameters.AddWithValue("genre", movie.genre);
            comm.Parameters.AddWithValue("imageurl", movie.imageurl);
            comm.Parameters.AddWithValue("released", movie.released);
            comm.Parameters.AddWithValue("imdbid", movie.imdbid);
            comm.Parameters.AddWithValue("imdbrating", movie.imdbrating);
            comm.Parameters.AddWithValue("synopsis", movie.synopsis);
            comm.Parameters.AddWithValue("key", i);
            await comm.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
        public async Task<int> GetKeyByTitle(string title, int chat_ID)
        {
            var sql = "SELECT \"key\" FROM \"ToWatchList\" WHERE \"title\" = @title";
            await connection.OpenAsync();
            NpgsqlCommand comm = new NpgsqlCommand(sql, connection);
            comm.Parameters.AddWithValue("chat_ID", chat_ID);
            comm.Parameters.AddWithValue("title", title);
            var result = await comm.ExecuteScalarAsync();
            await connection.CloseAsync();
            if (result != null)
            {
                return (int)result;
            }
            else
            {
                throw new Exception("No key found with the specified title");
            }
        }
        public async Task<int> DeleteFromToWatchList(int key)
        {
            var sql = "DELETE FROM \"ToWatchList\" WHERE \"key\" = @key";
            await connection.OpenAsync();
            NpgsqlCommand comm = new NpgsqlCommand(sql, connection);
            comm.Parameters.AddWithValue("key", key);
            var result = await comm.ExecuteScalarAsync();
            await connection.CloseAsync();
            if (result != null)
            {
                return (int)result;
            }
            else
            {
                throw new Exception("No key found with the specified title");
            }
        }
        public async Task<List<ToWatchList>> GetAllToWatch(int chat_ID)
        {
            var sql = "SELECT \"title\" FROM \"ToWatchList\" WHERE \"chat_ID\" = @chat_ID";
            await connection.OpenAsync();
            NpgsqlCommand comm = new NpgsqlCommand(sql, connection);
            comm.Parameters.AddWithValue("chat_ID", chat_ID);
            var reader = await comm.ExecuteReaderAsync();
            var toWatchLists = new List<ToWatchList>();
            while (await reader.ReadAsync())
            {
                var toWatchList = new ToWatchList
                {
                    title = reader.GetString(0),
                };
                toWatchLists.Add(toWatchList);
            }
            await connection.CloseAsync();
            return toWatchLists;
        }
        public async Task<bool> ClearToWatch(int chat_ID)
        {
            var sql = "DELETE FROM \"ToWatchList\" WHERE \"chat_ID\" = @chat_ID";
            await connection.OpenAsync();
            NpgsqlCommand comm = new NpgsqlCommand(sql, connection);
            comm.Parameters.AddWithValue("chat_ID", chat_ID);
            var reader = await comm.ExecuteNonQueryAsync();
            await connection.CloseAsync();
            return true;
        }
    }
}