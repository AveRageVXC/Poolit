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
    /// <param name="username">User's username.</param>
    /// <param name="password">User's password.</param>
    /// <returns>User</returns>
    [Route("/register")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Register(string username, string password)
    {
        try
        {
            var user = new User { Username = username };
            _userService.AssignPasswordHash(user, password);
            user.Id = 0;
            Response.Headers.Add("token", _userService.CreateToken(user));
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
    /// <param name="username">User's username.</param>
    /// <param name="password">User's password.</param>
    /// <param name="token">User's token.</param>
    /// <returns>User</returns>
    [Route("/login")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Login(string username, string password)
    {
        try
        {
            var id = 0;

            // username: w, password: w
            var hashedPassword = "AQAAAAIAAYagAAAAENBPS1G889jxdh2gdddLCvhEA7gbyF2Jb7MsxOXKkiXWGzcYj9/Z4bfzQi/FTXrv6A==";
            var user = new User { Username = username, HashedPassword = hashedPassword };
            user.Id = id;

            if (_userService.VerifyPassword(user, password) is false)
            {
                return new Response
                {
                    Error = "Wrong username or password"
                };
            }

            Response.Headers.Add("token", _userService.CreateToken(user));

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
    /// Getting user by username
    /// </summary>
    /// <param name="username">User's username</param>
    /// <returns>User</returns>
    [Route("/get-user-by-username")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetUserByUsername(string username)
    {
        try
        {
            var user = _userService.GetUserByUsername(username);

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
