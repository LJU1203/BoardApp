using System.ComponentModel.DataAnnotations;

namespace Board.Models.Entities
{
    public class User
    {
        
        [Key]
        public int UserId { get; set; } // Primary key

        [Required]
        [StringLength(50)]
        public string LoginId { get; set; }


        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
