using Fall2024_Assignment3_cbpausina.Models.Entities;

namespace Fall2024_Assignment3_cbpausina.Models.ViewModels;
public class ActorDetailsViewModel
{
    public Actor Actor { get; set; }

    public IEnumerable<Movie> Movies { get; set; }

    public IList<Tweet> Tweets { get; set; }

    public IList<double>? TweetSentiment { get; set; }

    public ActorDetailsViewModel(Actor actor, IEnumerable<Movie> movies, IList<Tweet> tweets, IList<double> tweetsentiment)
    {
        Actor = actor;
        Movies = movies;
        Tweets = tweets;
        TweetSentiment = tweetsentiment;
    }
}
