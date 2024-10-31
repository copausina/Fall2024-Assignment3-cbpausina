using Azure.AI.OpenAI;
using Fall2024_Assignment3_cbpausina.Models.Entities; // To use Tweet
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using System.ClientModel;
using System.Collections.Generic;
using System.Text.Json.Nodes;
//using Azure;
//using OpenAI;
//using System.Collections.Generic;
//using System.Threading.Tasks;

namespace Fall2024_Assignment3_cbpausina.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIAPIController : ControllerBase
    {
        private readonly ChatClient _client;
        private readonly string _aiDeployment;

        public OpenAIAPIController(IConfiguration configuration)
        {
            string apiKey = configuration["AzureOpenAI:ApiKey"];
            string apiEndpoint = configuration["AzureOpenAI:ApiEndpoint"];
            _aiDeployment = configuration["AzureOpenAI:AiDeployment"];
            _client = new AzureOpenAIClient(new Uri(apiEndpoint), new ApiKeyCredential(apiKey)).GetChatClient(_aiDeployment);
        }

        public async Task<IList<string>> GenerateTenMovieReviews(string movieTitle, int yearOfRelease)
        {
            string[] personas = {"is very postive", "is harsh", "is balanced", "loves classic movies", "prefers movies with action",
            "prefers deep movies", "discusses cinematography", "dicusses thematic elements", "likes to show off their film knowledge",
            "is pretentious"};

            var messages = new ChatMessage[]
            {
                new SystemChatMessage($"You represent a group of {personas.Length} film critics who have the following personalities: {string.Join(",", personas)}. When you receive a question, respond as each member of the group."),
                new UserChatMessage($"How would you rate the movie {movieTitle} released in {yearOfRelease} out of 10 in less than 175 words? Don't directly reference your personality, end each response with a '|', and don't indicate the order of the reviews.")
                //new SystemChatMessage($"You represent a group of {personas.Length} film critics who have the following personalities: {string.Join(",", personas)}. When you receive a question, respond as each member of the group."),
                //new UserChatMessage($"Don't directly reference your personality, and seperate all the responses like this: review | review | review. How would you rate the movie {movieTitle} released in {yearOfRelease} in less than 200 words?")
            };
            ClientResult<ChatCompletion> result = await _client.CompleteChatAsync(messages);

            var reviews = result.Value.Content[0].Text.Split('|').Select(s => s.Trim()).ToList(); // split string returned using | as delimiter


            //multiple calls
            //foreach (string persona in personas)
            //{
            //    var messages = new ChatMessage[]
            //    {
            //    new SystemChatMessage($"You are a film critic who {persona}."),
            //    new UserChatMessage($"How would you rate {movieTitle} released in {yearOfRelease} out of 10 in less than 175 words?")
            //    };
            //    var chatCompletionOptions = new ChatCompletionOptions
            //    {
            //        MaxOutputTokenCount = 200,
            //    };
            //    ClientResult<ChatCompletion> result = await _client.CompleteChatAsync(messages, chatCompletionOptions);

            //    reviews.Add(result.Value.Content[0].Text);
            //    await Task.Delay(10000); // Request throttle due to rate limit
            //}
            if(reviews.Count > 10)
                reviews.RemoveRange(10, reviews.Count - 10);	// keep only first ten elems in list

            return reviews;
        }

        public async Task<IList<Tweet>> TwitterApiSim(string actorName)
        {
            var messages = new ChatMessage[]
            {
            new SystemChatMessage($"You represent the Twitter social media platform."),
            new UserChatMessage($"Generate an answer with a valid JSON formatted array of objects containing the tweet and username. Your response should consist exclusively of the json, starting with [, do not include a message like 'here's a json array' before the json. Generate 20 tweets from a variety of users about the actor {actorName}.")
            };
            ClientResult<ChatCompletion> result = await _client.CompleteChatAsync(messages);

            string tweetsJsonString = result.Value.Content.FirstOrDefault()?.Text ?? "[]";
            JsonArray json = JsonNode.Parse(tweetsJsonString)!.AsArray();

            return json.Select(t => new Tweet { Username = t!["username"]?.ToString() ?? "", Text = t!["tweet"]?.ToString() ?? "" }).ToList();

            //var analyzer = new SentimentIntensityAnalyzer();
            //double sentimentTotal = 0;
            //foreach (var tweet in tweets)
            //{
            //    SentimentAnalysisResults sentiment = analyzer.PolarityScores(tweet.Text);
            //    sentimentTotal += sentiment.Compound;

            //    Console.WriteLine($"{tweet.Username}: \"{tweet.Text}\" (sentiment {sentiment.Compound})\n");
            //}

            //double sentimentAverage = sentimentTotal / tweets.Length;
        }
    }
}
