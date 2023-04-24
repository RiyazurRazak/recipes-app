using Microsoft.AspNetCore.Mvc;
using recipes_app.Data;
using recipes_app.Models;

namespace recipes_app.Controllers
{
    public class AuthController : Controller
    {

        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Register()
        {

            if (Request.Cookies["user"] != null)
            {
                return RedirectToAction("Index", "Recipes");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Email,Password,Name")] UsersModel payload)
        {
            try
            {
                payload.UserId = Guid.NewGuid().ToString();
                _context.Add(payload);
                await _context.SaveChangesAsync();
                Response.Cookies.Append("user", payload.UserId, new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true

                });
                return RedirectToAction("Index", "Recipes");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        public IActionResult Login()
        {
            if (Request.Cookies["user"] != null)
            {
                return RedirectToAction("Index", "Recipes");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Email,Password")] UsersModel userModel)
        {
            var validUser = _context.Users.Where(user => user.Password == userModel.Password && user.Email == userModel.Email).FirstOrDefault();
            if (validUser != null)
            {
                // add the user id to the cookies
                Response.Cookies.Append("user", validUser.UserId, new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true

                });
                return RedirectToAction("Index", "Recipes");
            }

            return RedirectToAction(nameof(Register));
        }


    }
}
