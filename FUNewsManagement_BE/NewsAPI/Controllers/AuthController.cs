using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAPI;
using NewsAPI.Services;
using System.Security.Claims;

namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly FunewsManagementContext _context;
        private readonly JwtService _jwtService;

        public AuthController(IConfiguration config, FunewsManagementContext context, JwtService jwtService)
        {
            _config = config;
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Check if admin
            var adminEmail = _config["AdminAccountConfig:Email"];
            var adminPassword = _config["AdminAccountConfig:Password"];

            if (dto.Email == adminEmail && dto.Password == adminPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, dto.Email),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("AccountName", "Admin Account"),
                    new Claim(ClaimTypes.NameIdentifier, "0") // Admin ID mặc định là 0
                };

                var token = _jwtService.GenerateToken(claims);
                return Ok(new { token });
            }

            // Check in DB for Staff/Lecturer
            var user = await _context.SystemAccounts
                .FirstOrDefaultAsync(u => u.AccountEmail == dto.Email && u.AccountPassword == dto.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var userRole = user.AccountRole switch
            {
                1 => "Staff",
                2 => "Lecturer",
                _ => "Unknown"
            };

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.AccountEmail),
                new Claim(ClaimTypes.Role, userRole),
                new Claim("AccountName", user.AccountName),
                new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString())
            };

            var userToken = _jwtService.GenerateToken(userClaims);
            return Ok(new { token = userToken });
        }
    }
}
