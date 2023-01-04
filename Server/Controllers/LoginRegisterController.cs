using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text;
using System.Security.Cryptography;
using DocsWASM.Pages.LoginRegister;
namespace DocsWASM.Controllers
{
    public class LoginRegisterController : Controller
    {
        [HttpGet]
        [Route("/login")]
        public IActionResult Login(bool badCredentials)
        {
            if (User.Identity !=null && User.Identity.IsAuthenticated)
                return Redirect("/");
            return View("../LoginRegister/Login", new LoginRegisterModels.LoginView() { badCredentials = badCredentials });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/login")]
        public async Task<IActionResult> LoginUser(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || Request.HttpContext.Connection.RemoteIpAddress == null)
                return Redirect("/login?badCredentials=true");

            var result = await (new LoginRegisterSQL()).Login(email, password, Request.HttpContext.Connection.RemoteIpAddress.ToString());

            if (result.success)
            {
                await SignInUser(result);
                return Redirect("/");
            }
            return Redirect("/login?badCredentials=true");

        }

        [HttpGet]
        [Route("/register")]
        public IActionResult Register()
        {
            
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return Redirect("/");
            return View("../LoginRegister/register", new LoginRegisterModels.RegisterView() { ipAdress = Request.HttpContext.Connection.RemoteIpAddress.ToString(), userAgent = Request.Headers["User-Agent"].ToString() });
        }

        [HttpGet]
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            return View("../LoginRegister/logout");
        }

        private async Task SignInUser(LoginRegisterModels.LoginResult user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Hash, user.hash),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }
    }
}
