using Fall2024_Assignment3_cbpausina.Models.Entities;

namespace Fall2024_Assignment3_cbpausina.Models.ViewModels;
public class MovieDetailsViewModel
{
    public Movie Movie { get; set; }

    public IEnumerable<Actor> Actors { get; set; }

    public IList<string> Reviews { get; set; }

    public IList<double> ReviewSentiment { get; set; }

    public MovieDetailsViewModel(Movie movie, IEnumerable<Actor> actors, IList<string> reviews, IList<double> reviewsentiment)
    {
        Movie = movie;
        Actors = actors;
        Reviews = reviews;
        ReviewSentiment = reviewsentiment;
    }
}

