using Board.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Board.Services
{
    public class UserService
    {
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService()
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string password)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return verificationResult == PasswordVerificationResult.Success;
        }
    }
}
