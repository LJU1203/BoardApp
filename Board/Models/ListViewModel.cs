using Board.Models.Entities;

namespace Board.Models
{
    public class ListItem
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public DateTime DatePosted { get; set; }
    }

    public class ListViewModel
    {
        public List<ListItem> PostItems { get; set; }
        public int TotalPosts { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public bool IsLoggedIn { get; set; }
    }
}
