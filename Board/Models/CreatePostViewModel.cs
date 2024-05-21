using System.ComponentModel.DataAnnotations;

namespace Board.Models
{
    public class CreatePostViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
