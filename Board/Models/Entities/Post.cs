using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Board.Models.Entities
{
    public class Post
    {
        [Key]
        public int PostId { get; set; } 

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;

        [ForeignKey("Author")]
        public int UserId { get; set; }

        // Navigation property for related comments
        public ICollection<Comment> Comments { get; set; }
    }
}
