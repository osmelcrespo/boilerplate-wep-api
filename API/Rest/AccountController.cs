using Core.Models;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Rest
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _contextApp;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, AppDbContext contextApp)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _contextApp = contextApp;
        }

        [HttpPost("Login")]
        public async Task<ResponseData> Login([FromBody] BasicUser user)
        {
            var messageResult = new ResponseData();

            var loginResults = new JObject();

            try
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);

                if (result.Succeeded)
                {
                    var appUser = await _userManager.Users.SingleOrDefaultAsync(r => r.Email == user.Email || r.UserName == user.Email);
                    if (appUser == null)
                    {
                        messageResult.Error = true;
                        messageResult.Description = "User not found!";
                        return messageResult;
                    }

                    messageResult.Data = await GenerateJwtToken(user.Email, appUser);

                    return messageResult;
                }
                else
                {
                    messageResult.Error = true;
                    messageResult.Description = "Invalid Credentials!";
                    return messageResult;
                }
            }
            catch (Exception e)
            {
                messageResult.Error = true;
                messageResult.Description = e.Message;
                messageResult.Data = e;
                return messageResult;
            }
        }

        //[Authorize]
        [HttpPost("Register")]
        public async Task<ResponseData> Register([FromBody] BasicUser model)
        {
            var messageResult = new ResponseData();

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                messageResult.Error = true;
                messageResult.Description = "Bad Request";
                messageResult.Data = result;
                return messageResult;
            }

            return messageResult;
        }

        private async Task<string> GenerateJwtToken(string email, IdentityUser user)
        {
            var userdb = await _contextApp.AspNetUsers.FirstOrDefaultAsync(x => x.Id == user.Id);

            var claims = new List<Claim>
            {
                new Claim("Email", email),
                new Claim("Id", user.Id),
                new Claim("Name", userdb.Name),
                new Claim("LastName", userdb.LastName),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
