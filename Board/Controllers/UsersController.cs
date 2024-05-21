using Microsoft.AspNetCore.Mvc;
using Board.Models;
using Board.Data;
using Board.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Board.Services;

namespace Board.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserService _userService;

        public UsersController(ApplicationDbContext dbContext)
        {
            this.dbContext=dbContext;
            _userService = new UserService();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {

            var user = new User
            {
                LoginId = model.LoginId,
                Username = model.Username
            };
            user.Password = _userService.HashPassword(user, model.Password);

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List", "Posts");
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string loginId, string password)
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.LoginId == loginId);
            

            if (user != null && _userService.VerifyPassword(user, password))
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                return RedirectToAction("List", "Posts");
            }

            ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.SetInt32("UserId", 0);
            return RedirectToAction("List", "Posts");
        }

        [HttpGet]
        [Route("api/checkLoginId")]
        public async Task<IActionResult> CheckLoginId(string loginId)
        {
            var exists = await dbContext.Users.AnyAsync(u => u.LoginId == loginId);
            return Ok(new { exists });
        }


        [HttpGet]
        [Route("api/checkUsername")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var exists = await dbContext.Users.AnyAsync(u => u.Username == username);
            return Ok(new { exists });
        }
    }
}
