namespace WebApplication1.Models
{
    public class MovieByName
    {
        public class Result
        {
            public string[] genre { get; set; }
            public string[] imageurl { get; set; }
            public string imdbid { get; set; }
            public float imdbrating { get; set; }
            public int released { get; set; }
            public string synopsis { get; set; }
            public string title { get; set; }
            public string type { get; set; }
        }
    }
}
