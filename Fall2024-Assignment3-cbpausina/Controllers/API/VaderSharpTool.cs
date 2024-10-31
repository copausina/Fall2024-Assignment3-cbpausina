using Fall2024_Assignment3_cbpausina.Models.Entities;
using VaderSharp2;

namespace Fall2024_Assignment3_cbpausina.Controllers.API
{
    public class VaderSharpTool
    {
        private readonly SentimentIntensityAnalyzer analyzer; // for VaderSharp2

        public VaderSharpTool()
        {
            analyzer = new SentimentIntensityAnalyzer();
        }

        public IList<double> AnalyzeReviews(IList<string> reviews) // await/task?
        {
            IList<double> sentimentScores = new List<double>();
            
            double sentimentTotal = 0;
            for (int i = 0; i < reviews.Count; i++)
            {
                string review = reviews[i]; // get next review
                SentimentAnalysisResults sentiment = analyzer.PolarityScores(review); // analyze it
                sentimentTotal += sentiment.Compound; // add its score to total score
                sentimentScores.Add(sentiment.Compound); // add its score to list
            }
            sentimentScores.Add(sentimentTotal / reviews.Count); // add average sentiment to end of list

            return sentimentScores;
        }

        public IList<double> AnalyzeTweets(IList<Tweet> tweets)
        {
            IList<double> sentimentScores = new List<double>();

            double sentimentTotal = 0;
            foreach (var tweet in tweets)
            {
                SentimentAnalysisResults sentiment = analyzer.PolarityScores(tweet.Text);
                sentimentTotal += sentiment.Compound;
                sentimentScores.Add(sentiment.Compound);
            }
            sentimentScores.Add(sentimentTotal / tweets.Count); // add average sentiment to end of list

            return sentimentScores;
        }
    }
}
