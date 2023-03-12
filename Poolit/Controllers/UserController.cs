using Microsoft.AspNetCore.Authorization;
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

    [Route("/register")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Register(string username, string password)
    {
        try
        {
            Response response;
            var user = new User { Username = username };
            if (_userService.CanSave(user) is false)
            {
                response = new Response { Error = "User with this name already exists." };
                return BadRequest(response);
            }
            _userService.AssignPasswordHash(user, password);
            _userService.SaveUser(user);
            Response.Headers.Add("token", _userService.CreateToken(user));

            var dataEntry = new DataEntry<User>()
            {
                Data = user,
                Type = "user"
            };
            response = new Response
            {
                Data = new DataEntry<User>[] { dataEntry }
            };
            return response;
        }
        catch
        {
            var response = new Response { Error = "Something went wrong. Please try again later. We are sorry." };
            return BadRequest(response);
        }
    }

    [Route("/login")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Login(string username, string password)
    {
        try
        {
            var user = new User { Username = username };

            if (_userService.CanSave(user))
            {
                return new Response
                {
                    Error = "Wrong username or password."
                };
            }
            
            user = _userService.GetUserByUsername(username);

            if (_userService.VerifyPassword(user, password) is false)
            {
                return new Response
                {
                    Error = "Wrong username or password."
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
        catch
        {
            var response = new Response { Error = "Something went wrong. Please try again later. We are sorry." };
            return BadRequest(response);
        }
    }

    [Route("/get-user-by-username")]
    [HttpPost, Authorize]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetUserByUsername(string username)
    {
        try
        {
            var user = new User { Username = username };
            if (_userService.CanSave(user))
            {
                var response = new Response { Error = "No user with this user name." };
                return BadRequest(response);
            }

            user = _userService.GetUserByUsername(username);

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
        catch
        {
            var response = new Response { Error = "Something went wrong. Please try again later. We are sorry." };
            return BadRequest(response);
        }
    }
}
