using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_cbpausina.Models.Entities
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Url] // Ensures link is valid URL format
        public string IMDbLink { get; set; }

        public string Genre { get; set; }

        [Range(1880, 2100)]
        public int YearOfRelease { get; set; }

        public byte[]? Poster { get; set; }

        public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();

        public IList<string>? Reviews { get; set; }

        public IList<double>? ReviewSentiment { get; set; }
    }
}
