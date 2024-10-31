using Fall2024_Assignment3_cbpausina.Models.Entities;

namespace Fall2024_Assignment3_cbpausina.Models.ViewModels
{
    public class MovieActorViewModel
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public List<Actor> AvailableActors { get; set; }
        public List<int> SelectedActorIds { get; set; } = new List<int>();
    }
}
