using System.ComponentModel.DataAnnotations;

namespace Board.Models.Entities
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public string? Url { get; set; }
    }
}
