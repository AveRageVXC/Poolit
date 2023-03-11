using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Poolit.Models;
using Poolit.Services;

namespace Poolit.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// User signing up.
    /// </summary>
    /// <param name="login">User's login.</param>
    /// <param name="password">User's password.</param>
    /// <returns>User</returns>
    [Route("/register")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Register(string login, string password)
    {
        try
        {
            var user = new User { Login = login };
            var hashedPassword = _userService.HashPassword(user, password);
            user.HashedPassword = hashedPassword;
            user.Id = 0;
            user.Token = _userService.CreateToken(user);
            var dataEntry = new DataEntry<User>()
            {
                Data = user,
                Type = "user"
            };
            var response = new Response
            {
                Data = new DataEntry<User>[] { dataEntry }
            };
            return response;
        }
        catch (Exception)
        {
            var response = new Response { Error = "Something went wrong. Please try again later. We are sorry." };
            return BadRequest(response);
        }
    }

    /// <summary>
    /// User signing in.
    /// </summary>
    /// <param name="login">User's login.</param>
    /// <param name="password">User's password.</param>
    /// <param name="token">User's token.</param>
    /// <returns>User</returns>
    [Route("/login")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Login(string login, string password)
    {
        try
        {
            var id = 0;

            // login: w, password: w
            var hashedPassword = "AQAAAAIAAYagAAAAENBPS1G889jxdh2gdddLCvhEA7gbyF2Jb7MsxOXKkiXWGzcYj9/Z4bfzQi/FTXrv6A==";
            var user = new User { Login = login, HashedPassword = hashedPassword };
            user.Id = id;

            if (_userService.VerifyPassword(user, hashedPassword, password) is false)
            {
                return new Response
                {
                    Error = "Wrong login or password"
                };
            }

            user.Token = _userService.CreateToken(user);

            var dataEntry = new DataEntry<User>()
            {
                Data = user,
                Type = "user"
            };

            return new Response
            {
                Data = new DataEntry<User>[] { dataEntry },
            };
        }
        catch (Exception)
        {
            var response = new Response { Error = "Something went wrong. Please try again later. We are sorry." };
            return BadRequest(response);
        }
    }

    /// <summary>
    /// Getting user by login
    /// </summary>
    /// <param name="login">User's login</param>
    /// <returns>User</returns>
    [Route("/get-user-by-login")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetUserByLogin(string login)
    {
        try
        {
            var id = 0;
            var user = _userService.GetUserByLogin(login);
            user.Token = Request.Headers.Authorization.ToString().Split(' ')[1];
            user.Id = id;

            var dataEntry = new DataEntry<User>()
            {
                Data = user,
                Type = "user"
            };

            return new Response
            {
                Data = new DataEntry<User>[] { dataEntry },
            };
        }
        catch (Exception e)
        {
            var response = new Response { Error = "Something went wrong. Please try again later. We are sorry." };
            return BadRequest(response);
        }
    }
}
