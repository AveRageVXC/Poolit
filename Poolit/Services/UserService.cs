using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Poolit.Configurations;
using Poolit.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Poolit.Services;

public class UserService : IUserService
{
    private IOptions<JwtConfiguration> _jwtConfiguration;

    public UserService(IOptions<JwtConfiguration> jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration;
    }

    public string HashPassword(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        return passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string hashedPassword, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        return passwordHasher.VerifyHashedPassword(user, hashedPassword, password) == PasswordVerificationResult.Success;
    }

    public string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Login)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Value.Token));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        var handler = new JwtSecurityTokenHandler().WriteToken(token);
        return handler;
    }

    public User GetUserByLogin(string login)
    {
        return new User { Login = login };
    }

    public User GetUserById(int id)
    {
        return new User { Id = id, Login = "" };
    }
}
