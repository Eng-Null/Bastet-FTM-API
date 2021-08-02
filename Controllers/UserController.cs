using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BastetAPI;
using BastetFTMAPI.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BastetFTMAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService repository;
        private readonly ILogger<UserController> logger;
        public UserController(IUserService repo, ILogger<UserController> log)
        {
            repository = repo;
            logger = log;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetClientsAsync()
        {
            var users = await repository.GetUsersAsync();

            return users.Select(x => x.UsersAsDto());
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetUserAsync(Guid id)
        {
            var user = await repository.GetUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Create New User
        /// </summary>
        /// <param name="createUser"> New User Data as Json</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUser createUser)
        {
            Random _random = new();

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = createUser.Username,
                DisplayName = createUser.DisplayName,
                UserNameHash = _random.Next(99999).ToString(),
                Email = createUser.Email,
                Password = createUser.Password,
                Roles = new List<Roles> { Roles.Guest },
                CreationDate = DateTimeOffset.UtcNow,
                LastLoginDate = DateTimeOffset.UtcNow,
                FailedAttempts = 0,
                IsLocked = false
            };

            await repository.CreateUserAsync(user);
            logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: User Created");
            return Ok();
        }

        /// <summary>
        /// Send Authenticate Token If User Found
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        [HttpPost("Authenticate")]
        public object LoginAsync([FromBody] LoginUser loginUser)
        {
            User user = repository.AuthenticateAsync(loginUser.Username, loginUser.UserNameHash, loginUser.Password).Result;
            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
              new Claim("Username", loginUser.Username),
              new Claim("DisplayName", user.DisplayName)
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var token = JwtHelper.GetJwtToken(loginUser.Username,
                                              "455578741455578741455578741455578741455578741",
                                              "Bastet FTM API",
                                              "RedWolf",
                                              TimeSpan.FromDays(10),
                                              claims.ToArray());
            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = token.ValidTo
            };
        }
    }
}
