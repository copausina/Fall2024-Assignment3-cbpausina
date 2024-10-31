using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Fall2024_Assignment3_cbpausina.Models.Entities
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Gender { get; set; } // enum?

        [Range(0, 130)]
        public int Age { get; set; }

        [Url] // Ensures link is valid URL format
        public string IMDbLink { get; set; }

        public byte[]? Photo { get; set; } // For storing the photo as a byte array

        public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();

        // JSON serialized column to store tweets as JSON string
        public string TweetsJson { get; set; } = "";

        [NotMapped] // Tweets will be ignored by EF
        public IList<Tweet> Tweets // Use this in code, not TweetsJson
        {
            get => string.IsNullOrEmpty(TweetsJson) ? new List<Tweet>() : JsonSerializer.Deserialize<List<Tweet>>(TweetsJson) ?? new List<Tweet>();
            set => TweetsJson = JsonSerializer.Serialize(value);
        }

        public IList<double>? TweetSentiment { get; set; }
    }

    public class Tweet
    {
        public string Username { get; set; }
        public string Text { get; set; }
    }
}
