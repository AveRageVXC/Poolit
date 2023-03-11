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

    public void AssignPasswordHash(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        user.HashedPassword = passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        return passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password) is PasswordVerificationResult.Success;
    }

    public User GetUserByUsername(string username)
    {
        return new User { Username = username };
    }

    public User GetUserById(int id)
    {
        return new User { Id = id, Username = "" };
    }
    
    public string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim> {
            new Claim(ClaimTypes.UserData, user.Id.ToString())
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

    public int GetIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        return int.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.UserData).Value);
    }
}
