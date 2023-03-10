using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Poolit.Models;
using Poolit.Models.Requests;
using Poolit.Models.Responses;
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

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        var user = new User { Login = request.Login };
        var hashedPassword = HashPassword(user, request.Password);
        user.HashedPassword = hashedPassword;
        user.Id = 0;
        user.Token = CreateToken(user);

        return new RegisterResponse
        {
            User = user
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        if (request.Token?.Length > 0)
        {
            // check if token is correct
        }

        var id = 0;
        // login: w, password: w
        var hashedPassword = "AQAAAAIAAYagAAAAENBPS1G889jxdh2gdddLCvhEA7gbyF2Jb7MsxOXKkiXWGzcYj9/Z4bfzQi/FTXrv6A==";
        var user = new User { Login = request.Login, HashedPassword = hashedPassword };
        user.Id = id;

        if (VerifyPassword(user, hashedPassword, request.Password) != PasswordVerificationResult.Success)
        {
            return new LoginResponse
            {
                Error = "Wrong login or password",
                HasError = true,
            };
        }

        user.Token = CreateToken(user);

        return new LoginResponse
        {
            User = user,
        };
    }

    private string HashPassword(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        return passwordHasher.HashPassword(user, password);
    }

    private PasswordVerificationResult VerifyPassword(User user, string hashedPassword, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        return passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
    }

    private string CreateToken(User user)
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
