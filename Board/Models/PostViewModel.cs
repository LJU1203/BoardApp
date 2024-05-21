using Board.Models.Entities;

namespace Board.Models
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public string Username { get; set; }
        public DateTime DateTime { get; set; }

        public bool IsAuthor { get; set; }
    }

    public class PostViewModel
    {
        public Post Post { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }

        public bool IsAuthor { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
