using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using KitanoUserService.API.Models.ExecuteModels;

namespace KitanoUserService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenTokenController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IDatabase _iDb;
        public GenTokenController(IConfiguration config, IDatabase redisDb)
        {
            _config = config;
            _iDb = redisDb;
        }
        private string GenerateJwtToken(string user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("id", user),
                new Claim(JwtRegisteredClaimNames.Sub, user),
                new Claim(JwtRegisteredClaimNames.Email, $"{user}@gmail.com"),
                new Claim("roles", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }

        [HttpPost]
        public IActionResult GenToken(LoginModel login)
        {
            var jwtToken = GenerateJwtToken(login.user_name);

            var user = new CurrentUserModel()
            {
                Id = 0,
                UserName = login.user_name,
                FullName = "hoang",
                //IsActive = true,
                //IsDeleted = false,
                RoleId = 1
            };

            //_iDb.StringSet($"Auth.UserInfo.{login.user_name}", JsonSerializer.Serialize(user));

            return Ok(new AutResult()
            {
                Result = true,
                Token = jwtToken
            });
        }
    }
}
