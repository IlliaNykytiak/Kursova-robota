using Microsoft.VisualBasic;
using Npgsql;
using System.Text;
using WebApplication1.Models;
namespace WebApplication1
{
    public class Database
    {
        //        NpgsqlConnection connection = new NpgsqlConnection(Constants.Connect);
        //        public async Task InsertMovieListAsync(MovieList movieList)
        //        {
        //            var sql = "insert into public.\"MovieList\" (\"imageurl\")" +
        //$"values (@imageurl)";
        //            NpgsqlCommand comm = new NpgsqlCommand(sql.ToString(), connection);
        //            await connection.OpenAsync();
        //            foreach (var result in movieList.results)
        //            {
        //                comm.Parameters.AddWithValue("@imageurl", result.imageurl);
        //                await comm.ExecuteNonQueryAsync();
        //                await Task.Delay(1000);
        //            }
        //            await connection.CloseAsync();
        //            //var sql = "insert into public.\"MovieList\" (\"genre\", \"imageurl\"" +
        //            //    ", \"imdbid\", \"imdbrating\", \"released\", \"synopsis\", \"title\", \"type\")" +
        //            //    $"values (@genre, @imageurl, @imdbid, @imdbrating, @released, @synopsis, @title, @type)";
        //            //NpgsqlCommand comm = new NpgsqlCommand(sql, connection);
        //            //foreach (var result in movieList.results)
        //            //{
        //            //    comm.Parameters.AddWithValue("genre", result.genre);
        //            //    comm.Parameters.AddWithValue("@imageurl", result.imageurl);
        //            //    comm.Parameters.AddWithValue("@imdbid", result.imdbid);
        //            //    comm.Parameters.AddWithValue("@imdbrating", result.imdbrating);
        //            //    comm.Parameters.AddWithValue("@released", result.released);
        //            //    comm.Parameters.AddWithValue("@synopsis", result.synopsis);
        //            //    comm.Parameters.AddWithValue("@title", result.title);
        //            //    comm.Parameters.AddWithValue("@type", result.type);
        //            //}
        //        }
        NpgsqlConnection connection = new NpgsqlConnection(Constants.Connect);
        public async Task InsertMovieListAsync(MovieList movieList)
        {
            var sql = new StringBuilder();
            sql.Append("insert into public.\"MovieList\" (\"imageurl\") values ");
            var parameters = new List<NpgsqlParameter>();

            for (int i = 0; i < 50; i++)
            {
                sql.Append($"(@imageurl{i}),");
                parameters.Add(new NpgsqlParameter($"@imageurl{i}", movieList.results[i].imageurl));
            }

            sql.Length--; // Видалити останню кому
            NpgsqlCommand comm = new NpgsqlCommand(sql.ToString(), connection);
            await connection.OpenAsync();
            comm.Parameters.AddRange(parameters.ToArray());
            await comm.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
    }
}
