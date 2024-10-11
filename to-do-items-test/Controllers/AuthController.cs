using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TodoItemsTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Check for null login model
            if (login == null)
            {
                return BadRequest("Login model is null.");
            }

            // Check if required fields are present
            if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
            {
                return BadRequest("Username and Password are required.");
            }

            // Basic user validation (replace with actual validation logic)
            if (login.Username == "test" && login.Password == "password")
            {
                // Generate the JWT token
                var tokenString = GenerateJwtToken(login.Username);

                // Ensure token generation was successful
                if (string.IsNullOrEmpty(tokenString))
                {
                    return StatusCode(500, "Error generating JWT token.");
                }

                // Check if Response or Response.Cookies is null
                if (Response == null || Response.Cookies == null)
                {
                    return StatusCode(500, "Response or Cookies is null.");
                }

                // Append the cookie
                Response.Cookies.Append("access_token", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Ensure you're using HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                });

                // Return the token in the response
                return Ok(new { Token = tokenString });
            }

            // If credentials are invalid, return Unauthorized (401)
            return Unauthorized();
        }

        // Helper method to generate JWT token
        private string GenerateJwtToken(string username)
        {
            // Retrieve JWT configuration and ensure values are not null
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT settings are missing in configuration.");
            }

            // Create the JWT token claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            // Create the key and credentials using HMACSHA256
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            // Return the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // Model to capture login request details
    public class LoginModel
    {
        /// <summary>
        /// Username is required and must be between 3 and 50 characters.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string? Username { get; set; }

        /// <summary>
        /// Password is required and must be between 6 and 100 characters.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }
    }
}
