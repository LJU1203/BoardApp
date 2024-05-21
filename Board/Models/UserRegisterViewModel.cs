using System.ComponentModel.DataAnnotations;

namespace Board.Models
{
    public class UserRegisterViewModel
    {
        public string LoginId { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }

    }
}
