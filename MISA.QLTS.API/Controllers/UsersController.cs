using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Exceptions;
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
                return StatusCode(200, -1);
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
            // Hash mật khẩu trước khi so sánh
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
        private IActionResult HandleException(Exception ex)
        {
            var res = new
            {
                devMsg = ex.Message,
                userMsg = "Có lỗi xảy ra, vui lòng liên hệ MISA để được hỗ trợ.",
                errorCode = "001",
                data = ex.Data
            };
            // Nếu ex thuộc validate được viết ở trên
            if (ex is MISAValidateException)
            {
                return StatusCode(400, res);

            }
            else return StatusCode(500, res);
        }
    }
}
