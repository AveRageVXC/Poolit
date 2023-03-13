using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poolit.Models;
using Poolit.Models.Requests;
using Poolit.Services;
using System.Text.RegularExpressions;

namespace Poolit.Controllers;

[Route("api/[controller]")]
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

    [Route("register")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Register([FromBody] RegisterRequest request)
    {
        var response = new Response();
        try
        {
            var userName = request.UserName.Trim();

            if (userName.Length < 4)
            {
                response.Error = "Username must be at least 4 symbols";
                return BadRequest(response);
            }

            if (userName.Length > 32)
            {
                response.Error = "Username's length can't be over 32 symbols";
                return BadRequest(response);
            }

            var password = request.Password.Trim();

            if (password.Length < 8)
            {
                response.Error = "Password's length must be at least 8 symbols";
                return BadRequest(response);
            }

            if (password.Length > 32)
            {
                response.Error = "Password's length can't be over 32 symbols";
                return BadRequest(response);
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");

            if (!hasNumber.IsMatch(password))
            {
                response.Error = "Password must contain at least 1 digit";
                return BadRequest(response);
            }

            if (!hasUpperChar.IsMatch(password))
            {
                response.Error = "Password must contain at least 1 capital letter";
                return BadRequest(response);
            }

            var user = new User { Username = userName };

            // CanSave == don't have users's username in db
            if (_userService.CanSave(user) is false)
            {
                response.Error = "User with this name already exists";
                return BadRequest(response);
            }

            _userService.AssignPasswordHash(user, password );

            _userService.SaveUser(user);

            //Response.Headers.Add("token", token);

            var dataEntry = new DataEntry<User>()
            {
                Data = user,
                Type = "user"
            };
            response.Data = new[] { dataEntry };

            return Ok(response);
        }
        catch
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            return BadRequest(response);
        }
    }

    [Route("login")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> Login([FromBody] LoginRequest request)
    {
        var response = new Response();
        try
        {
            var username = request.UserName.Trim();
            var password = request.Password.Trim();
            var user = new User { Username = username };

            // CanSave user = user doesn't exists
            if (_userService.CanSave(user))
            {
                response.Error = "Wrong username or password.";
                return BadRequest(response);
            }

            user = _userService.GetUserByUsername(username);

            if (_userService.VerifyPassword(user, password) is false)
            {
                response.Error = "Wrong username or password.";
                return BadRequest(response);
            }

            //Response.Headers.Add("token", token);

            var dataEntry = new DataEntry<User>()
            {
                Data = user,
                Type = "user"
            };
            response.Data = new[] { dataEntry };

            return Ok(response);
        }
        catch
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            return BadRequest(response);
        }
    }

    [Route("get-user-by-username")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetUserByUsername([FromBody] string username)
    {
        var response = new Response();
        try
        {
            username = username.Trim();
            var user = new User { Username = username };

            // CanSave user = user doesn't exist
            if (_userService.CanSave(user))
            {
                response.Error = "No user with this user name";
                return BadRequest(response);
            }

            user = _userService.GetUserByUsername(username);

            var dataEntry = new DataEntry<User>()
            {
                Data = user,
                Type = "user"
            };
            response.Data = new[] { dataEntry };

            return Ok(response);
        }
        catch
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            return BadRequest(response);
        }
    }

    [Route("get-users-by-username")]
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response>> GetUsersByUsername([FromBody] string username)
    {
        var response = new Response();
        try
        {
            username = username.Trim();

            var users = _userService.GetUsersByUsername(username).Select(u => new DataEntry<User> { Type = "user", Data = u }).ToArray();

            response.Data = users;

            return Ok(response);
        }
        catch
        {
            response.Error = "Something went wrong. Please try again later. We are sorry";
            return BadRequest(response);
        }
    }
}
