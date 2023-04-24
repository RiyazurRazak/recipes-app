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

            // redirect to login page if cookie not present
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
                // generate new unique id to the user
                payload.UserId = Guid.NewGuid().ToString();

                // create user in db
                _context.Add(payload);
                await _context.SaveChangesAsync();

                // add cookie with secure option to access with server only
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
                // add cookie with secure option to access with server only
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

        public IActionResult Logout()
        {
            // remove cookie
            Response.Cookies.Delete("user");
            return RedirectToAction("Login", "Auth");
        }



    }
}
