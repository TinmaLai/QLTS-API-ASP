using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.CORE.Entities;
using System.Security.Claims;

namespace MISA.QLTS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Hàm xử lý login, tạo cookie lưu vào database
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns>Ok hoặc lỗi sai tài khoản mật khẩu</returns>
        [HttpPost]
        public async Task<IActionResult> OnPostAsync([FromBody] User userLogin)
        {
            
            var user = AuthenticateUser(userLogin.Username, userLogin.Password);
            if(user == null)
            {
                return BadRequest();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                null);
            return Ok();
        }
        /// <summary>
        /// Check username, password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>User đúng tài khoản mật khẩu hoặc null</returns>
        private User AuthenticateUser(string username, string password)
        {
            
            if (username == "tinmalai" && password == "123")
            {
                return new User()
                {
                    Username = "tinmalai",
                };
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Hàm logout, xóa cookie
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            //SignOutAsync is Extension method for SignOut    
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Redirect to home page    
            return Ok();
        }
    }
}
