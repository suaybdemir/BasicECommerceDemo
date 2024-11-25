using ECommerceDemo.Data;
using ECommerceDemo.Dtos;
using ECommerceDemo.Models.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

[AllowAnonymous, Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthenticationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    // Kullanıcı kaydetme işlemi
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterDTO model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.FullName,
                Email = model.Email
            };

            

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Başarılı kayıt durumunda, kullanıcıyı otomatik giriş yapma (isteğe bağlı)
                await _signInManager.SignInAsync(user, isPersistent: false);

                // JWT Token oluştur
                var token = GenerateJwtToken(user);

                return Ok(new { Token = token });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return BadRequest(ModelState);
    }

    // Kullanıcı girişi işlemi (JWT Token ile)
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                {
                    var token = GenerateJwtToken(user);
                    return Ok(new { Token = token });
                }
            }

            return Unauthorized("Invalid login attempt.");
        }

        return BadRequest(ModelState);
    }

    private object GenerateJwtToken(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

        // Check if the key size is at least 256 bits (32 bytes)
        if (key.Length < 32)
        {
            throw new ArgumentException("The encryption key must be at least 256 bits (32 bytes).");
        }

        var userRoles = _userManager.GetRolesAsync(user).Result;

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expireSeconds = Convert.ToInt32(_configuration["Jwt:Expire"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expireSeconds),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var expirationDate = token.ValidTo;

        return new
        {
            tokenstr = tokenHandler.WriteToken(token),
            expiration = expirationDate
        };
    }


    // Şifre sıfırlama işlemi - Reset Password
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("No user found with this email.");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password reset successful.");
            }

            return BadRequest("Failed to reset password.");
        }

        return BadRequest(ModelState);
    }

    // Logout (Çıkış) işlemi
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("Logged out successfully.");
    }
}
