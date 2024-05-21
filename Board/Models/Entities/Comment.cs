using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Board.Models.Entities
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; } 

        [Required]
        public string Content { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;

        // Foreign key for Post
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }


        [ForeignKey("Author")]
        [Required]
        public int UserId { get; set; }
        

    }
}
