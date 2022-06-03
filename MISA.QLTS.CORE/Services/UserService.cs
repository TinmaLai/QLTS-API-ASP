using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MISA.QLTS.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Services
{
    public class UserService
    {
        
        private async Task<User> AuthenticateUser(string email, string password)
        {
            // For demonstration purposes, authenticate a user
            // with a static email address. Ignore the password.
            // Assume that checking the database takes 500ms

            await Task.Delay(500);

            if (email == "tinmalai")
            {
                return new User()
                {
                    Username = "tinmalai",
                    Password = "123"
                };
            }
            else
            {
                return null;
            }
        }
    }
}
