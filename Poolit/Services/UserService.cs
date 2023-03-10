using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Poolit.Models;
using Poolit.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Poolit.Services;

public class UserService : IUserService
{
    private IOptions<TokensConfiguration> _tokenConfiguration;

    public UserService(IOptions<TokensConfiguration> tokenConfiguration)
    {
        _tokenConfiguration = tokenConfiguration;
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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfiguration.Value.JWT));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);

        var handler = new JwtSecurityTokenHandler().WriteToken(token);
        return handler;
    }
}
